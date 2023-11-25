using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class GenMotionIDList
{
    private static readonly string genMotionIDListPath = Application.dataPath + "/Scripts/ECS/Nodes/AutoGen/";
    private static readonly string fileName = "MotionIDs";

    [MenuItem("Tools/Motion Auto Gen/生成MotionID列表")]
    public static void GenMotionID()
    {
        var motionNames = PrefabModel.GetMotionList();
        StringBuilder sb = new StringBuilder();
        if (!System.IO.Directory.Exists(genMotionIDListPath))
        {
            System.IO.Directory.CreateDirectory(genMotionIDListPath);
        }

        var filePath = genMotionIDListPath + fileName + ".cs";
        System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, false);
        file.Write("// auto generated code, don't edit\n\n");
        file.Write("namespace Motion\n{\n");
        file.Write("\tpublic class MotionIDs\n\t{\n");

        for (int i = 0; i < motionNames.Length; i++)
        {
            var idName = motionNames[i];
            if (idName == "Test") continue;
            var cfg = PrefabModel.GetMotionAssetByName(idName);
            var id = cfg.UID;
            sb.Append("\t\tpublic const int ");
            sb.Append(idName);
            sb.Append(" ");
            sb.Append("=");
            sb.Append(" ");
            sb.Append(id.ToString());
            sb.Append(";\n");
        }

        sb.AppendLine("\t\tpublic static readonly int[] IDList = new int[]{");
        for (int i = 0; i < motionNames.Length; i++)
        {
            var idName = motionNames[i];
            if (idName == "Test") continue;
            var cfg = PrefabModel.GetMotionAssetByName(idName);
            var id = cfg.UID;
            sb.Append("\t\t\t");
            sb.Append(idName +",\n");
        }

        sb.AppendLine("\t\t};");
        
        sb.AppendLine("\t\tpublic static readonly string[] NameList = new string[]{");
        for (int i = 0; i < motionNames.Length; i++)
        {
            var idName = motionNames[i];
            if (idName == "Test") continue;
            var cfg = PrefabModel.GetMotionAssetByName(idName);
            var id = cfg.UID;
            sb.Append("\t\t\t");
            sb.Append("\""+idName +"\",\n");
        }

        sb.AppendLine("\t\t};");
        
        file.Write(sb.ToString());

        file.Write("\t}\n");
        file.Write("}");
        file.Close();
        Debug.Log("文件生成成功：" + filePath);
        AssetDatabase.Refresh();
    }
}
