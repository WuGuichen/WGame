using UnityEditor;
using UnityEngine;
using WGame.UIEditor;

public class FGUIEditorTool : EditorWindow
{
    [MenuItem("FairyGUI/FairyGUI编辑器", false, 100)]
    public static void OpenFairyGUINew()
    {
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.FileName = $"{System.Environment.CurrentDirectory}\\..\\FGUIProject\\FairyGUI-Editor\\FairyGUI-Editor.exe";
        //Debug.Log(p.StartInfo.FileName);
        p.StartInfo.Arguments = $"\"{System.Environment.CurrentDirectory}\\..\\FGUIProject\\FGUIProject.fairy\"";
        Debug.Log(p.StartInfo.Arguments);
        p.EnableRaisingEvents = true;
        p.Start();
    }
    [MenuItem("FairyGUI/生成UI代码", false, 100)]
    public static void GenFairyGUICodes()
    {
        FGUICodeSpawner.FUICodeSpawn();
    }
    
    [MenuItem("FairyGUI/Generate Package Mapping")]
    public static void Generate()
    {
        FairyGUI.Dynamic.Editor.UIPackageMappingUtility.GenerateMappingFile("Assets/Bundles/FGUI", "Assets/Bundles/Gen/UIPackageMapping.asset");
    }
}
