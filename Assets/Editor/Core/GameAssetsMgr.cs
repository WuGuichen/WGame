namespace WGame.Editor
{
    using UnityEngine;
    using WGame.Runtime;
    using System.Linq;
    using System.IO;
    using System.Collections.Generic;

    public class GameAssetsMgr : Singleton<GameAssetsMgr>
    {
        private Dictionary<string, string> _prefabMap = new Dictionary<string, string>();

        private Dictionary<string, string> _animClipMap = new();
        private Dictionary<string, AnimationClip> _animClipCache = new();
        public const string AnimClipFolder = "Assets/Res/Animations";
        // public const string ResRoot = "Assets/Res";
        public const string PrefabRoot = "Assets/Prefabs";
        
        public static string PrefabDataPath { get; private set; }
        public static string PrefabEffectPath { get; private set; }

        public static string AbilityDataPath { get; private set; }

        private void BuildResMap()
        {
            string abRoot = Path.Combine(Directory.GetCurrentDirectory(), PrefabRoot);
            if (!Directory.Exists(abRoot))
                WLogger.Error("The directory of resource is not exist!");

            var guids = UnityEditor.AssetDatabase.FindAssets("t:Texture t:TextAsset t:GameObject t:Shader t:Font", new string[] { PrefabRoot });
            for (int i=0; i<guids.Length; ++i)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                if (File.Exists(assetPath))
                {
                    string fileName = assetPath.Replace(PrefabRoot, "");
                    _prefabMap.Add(fileName, assetPath);
                }
            }
        }

        private void InitResFolderPath()
        {
            PrefabDataPath = Application.dataPath.Replace("Assets", PrefabRoot);
            
            AbilityDataPath = $"{PrefabDataPath}/AbilityData/";
            PrefabEffectPath = $"{PrefabDataPath}/Effects/";
        }

        public override void InitInstance()
        {
            Dispose();
            InitResFolderPath();
            BuildResMap();
            
            var guids = UnityEditor.AssetDatabase.FindAssets("t:AnimationClip", new string[] { AnimClipFolder });
            for (int i=0; i<guids.Length; ++i)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                if (File.Exists(assetPath))
                {
                    var clipName = Path.GetFileNameWithoutExtension(assetPath);
                    _animClipMap.Add(clipName, assetPath);
                }
            }
        }

        private bool FindResource(string fileName, out string asset)
        {
            if (_prefabMap.TryGetValue(fileName, out asset))
            {
                return true;
            }
            
            return false;
        }
        
        public string FormatResourceName(string name)
        {
            return name.Replace(PrefabRoot, "");
        }
        
        public AnimationClip LoadAnimClip(string fileName)
        {
            if (_animClipCache.TryGetValue(fileName, out var clip))
            {
                return clip;
            }
            if (_animClipMap.TryGetValue(fileName, out var assetPath))
            {
                clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
                if (null == clip)
                {
                    WLogger.Error($"无法加载{assetPath}");
                }
                _animClipCache.Add(fileName, clip);
                return clip;
            }
            WLogger.Error($"Res/Animations/文件夹下没有{fileName}");
            return null;
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

            using (var itr = _prefabMap.GetEnumerator())
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

        public List<string> GetAllAnimationClips()
        {
            return _animClipMap.Keys.ToList();
        }

        public void Dispose()
        {
            _animClipMap.Clear();
            _animClipCache.Clear();
            _prefabMap.Clear();
        }
    }
}