using System.IO;
using UnityEditor;
using UnityEngine;

public class HotUpdateTools
{
    private static readonly string _basePath = Application.dataPath + "/../HybridCLRData/HotUpdateDlls/";
    private static readonly string _baseMetaPath = Application.dataPath + "/../HybridCLRData/AssembliesPostIl2CppStrip/";
    private static readonly string _targetPath = Application.dataPath + "/Res/HotUpdateDll/";
    private static readonly string _targetMetaDLLPath = Application.dataPath + "/Res/AOTMetaDll/";

    private static void ResetDictionary(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        else
        {
            var direction = new DirectoryInfo(path);
            var files = direction.GetFiles("*", SearchOption.AllDirectories);

            Debug.Log(files.Length);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }

                string FilePath = path + "/" + files[i].Name;
                File.Delete(FilePath);
            }
        }
    }
    
    [MenuItem("Utils/HotUpdate/更新AOT MetaDLL", false, 100)]
    private static void CopyBuildAOTMetaDllToHotUpdate()
    {
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        var sourcePath = _baseMetaPath + buildTarget;
        var list = AOTGenericReferences.PatchedAOTAssemblyList;
        ResetDictionary(_targetMetaDLLPath);
        foreach (var name in list)
        {
            var source = Path.Combine(sourcePath, name);
            var target = Path.Combine(_targetMetaDLLPath, name + ".bytes");
            if (File.Exists(source))
            {
                File.Copy(source, target, true);
            }
        }
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Utils/HotUpdate/更新热更DLL", false, 100)]
    private static void CopyBuildDllToHotUpdate()
    {
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        var sourcePath = _basePath + buildTarget;
        var list = WGame.Runtime.HotUpdateList.HotList;
        ResetDictionary(_targetPath);
        foreach (var name in list)
        {
            var source = Path.Combine(sourcePath, name);
            var target = Path.Combine(_targetPath, name + ".bytes");
            if (File.Exists(source))
            {
                File.Copy(source, target, true);
            }
        }
        AssetDatabase.Refresh();
    }
}
