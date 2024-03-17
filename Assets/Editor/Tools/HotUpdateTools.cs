using System.IO;
using UnityEditor;
using UnityEngine;

public class HotUpdateTools
{
    private static readonly string _basePath = Application.dataPath + "/../HybridCLRData/HotUpdateDlls/";
    private static readonly string _targetPath = Application.dataPath + "/Res/HotUpdateDll/";
    private static readonly string _targetMetaDLLPath = Application.dataPath + "/Res/AOTMetaDll/";
    
    [MenuItem("Utils/HotUpdate/更新AOT MetaDLL", false, 100)]
    private static void CopyBuildAOTMetaDllToHotUpdate()
    {
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        var sourcePath = _basePath + buildTarget;
        var list = AOTGenericReferences.PatchedAOTAssemblyList;
        if (!Directory.Exists(_targetMetaDLLPath))
        {
            Directory.CreateDirectory(_targetMetaDLLPath);
        }
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
        if (!Directory.Exists(_targetPath))
        {
            Directory.CreateDirectory(_targetPath);
        }
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
