

namespace WGame.Editor
{
    using UnityEngine;
    using WGame.Runtime;
    using System.IO;
    using System.Collections.Generic;

    public class GameAssetsMgr : Singleton<GameAssetsMgr>
    {
        private Dictionary<string, string> _fileMap = new Dictionary<string, string>();
        public const string ResRoot = "Assets/Res";
        
        public static string ResDataPath { get; private set; }

        public static string AbilityDataPath { get; private set; }

        private void BuildResMap()
        {
            string abRoot = Path.Combine(Directory.GetCurrentDirectory(), ResRoot);
            if (!Directory.Exists(abRoot))
                WLogger.Error("The directory of resource is not exist!");

            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Texture t:TextAsset t:GameObject t:Shader t:Font", new string[] { ResRoot });
            for (int i=0; i<guids.Length; ++i)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                if (File.Exists(assetPath))
                {
                    string fileName = assetPath.Replace(ResRoot, "");
                    _fileMap[fileName] = assetPath;
                }
            }
        }

        private void InitResFolderPath()
        {
            ResDataPath = Application.dataPath.Replace("Assets", ResRoot);
            
            AbilityDataPath = $"{ResDataPath}/AbilityData/";
        }
        
        public override void InitInstance()
        {
            InitResFolderPath();
            BuildResMap();
        }

        private bool FindResource(string fileName, out string asset)
        {
            if (_fileMap.TryGetValue(fileName, out asset))
            {
                return true;
            }
            
            return false;
        }
        
        public string FormatResourceName(string name)
        {
            return name.Replace(ResRoot, "");
        }
        
        public Object LoadObject(string fileName, System.Type type)
        {
            string asset = string.Empty;
            if (FindResource(fileName, out asset))
            {
                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(asset, type);
                if (null == obj)
                {
                    WLogger.Error($"GameAssetsMgr Failed to load asset: {fileName}");
                }
                return obj;
            }
            else
            {
                WLogger.Error($"The resource '{fileName}' is not exist! ");
                return null;
            }
        }

        public T LoadObject<T>(string fileName) where T : UnityEngine.Object
        {
            string asset = string.Empty;
            if (FindResource(fileName, out asset))
            {
                T obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(asset);
                if (null == obj)
                {
                    WLogger.Error($"GameAssetsMgr Failed to load asset: {fileName}");
                }
                return obj;
            }
            else
            {
                WLogger.Error($"The resource '{fileName}' is not exist! ");
                return null;
            }
        }
        
        public string[] GetFiles(string path)
        {
            List<string> fileList = new List<string>();

            using (var itr = _fileMap.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    if (itr.Current.Key.Contains(path))
                    {
                        fileList.Add(itr.Current.Key);
                    }
                }
            }

            return fileList.ToArray();
        }
    }
}