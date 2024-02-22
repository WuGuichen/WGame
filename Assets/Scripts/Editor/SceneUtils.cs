using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Text;
using UnityEngine;
using WGame.Util;

public class SceneUtils
{
    [MenuItem("Utils/Open BootStrap")]
    static void OpenBootStrap()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/BootStrap.unity");
    }
    [MenuItem("Utils/Open SandBox")]
    static void OpenSandBox()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/SandBox.unity");
    }

    [MenuItem("Utils/清除PlayerPrefs")]
    static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteKey("TestInput");    
    }
    [MenuItem("Utils/Open TestScene")]
    static void OpenTestScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/TestScene.unity");
    }

    [MenuItem("WLang/更新WLang脚本")]
    static void RefreshWLang()
    {
        WLangMgr.Inst.HotUpdate();
    }
    
    [MenuItem("WLang/执行MyHotAction")]
    static void DoMyHotAction()
    {
        WLangMgr.Inst.CallCode("MyHotAction");
    }

    [MenuItem("Utils/代码生成/生成命令数据")]
    static void OpenTest()
    {
        var list = ReflectionHelper.GetStaticFunctionList(typeof(ActionHelper), new List<string>(){"DoAction", "Do"});
        string path = "Assets/Scripts/ECS/Trigger/Action/AutoGen/ActionHelper.AutoGen.cs";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("/** This is an automatically generated . Please do not modify it. **/");
        sb.AppendLine();
        sb.AppendLine("public partial class ActionHelper");
        sb.AppendLine("{");
        StringBuilder sbName = new StringBuilder();
        StringBuilder sbParam1 = new StringBuilder();
        StringBuilder sbParam2 = new StringBuilder();
        sbName.AppendLine("\tpublic static string[] ActionListName = new string["+list.Count+"]");
        sbName.AppendLine("\t{");
        sbParam1.AppendLine("\tpublic static string[] ActionListParam1 = new string["+list.Count+"]");
        sbParam1.AppendLine("\t{");
        sbParam2.AppendLine("\tpublic static string[] ActionListParam2 = new string["+list.Count+"]");
        sbParam2.AppendLine("\t{");
        var paramList1 = new string[list.Count];
        var paramList2 = new string[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            var info = list[i];
            sbName.Append("\t\t\"" + info.Name + "\"");
            var parameters = info.GetParameters();
            if (parameters.Length > 0)
            {
                paramList1[i] = parameters[0].ParameterType.FullName;
                sbParam1.Append("\t\t\"{0}\"".Fmt(paramList1[i]));
            }
            else
            {
                paramList1[i] = null;
                sbParam1.Append("\t\tnull");
            }

            if (parameters.Length > 1)
            {
                paramList2[i] = parameters[1].ParameterType.FullName;
                sbParam2.Append("\t\t\"{0}\"".Fmt(paramList2[i]));
            }
            else
            {
                paramList2[i] = null;
                sbParam2.Append("\t\tnull");
            }

            if (parameters.Length > 2)
                WLogger.Error(info.Name + "有三个参数的函数没有做处理");

            if (i != list.Count - 1)
            {
                sbName.Append(",");
                sbParam1.Append(",");
                sbParam2.Append(",");
            }

            sbName.AppendLine();
            sbParam1.AppendLine();
            sbParam2.AppendLine();
        }

        sbName.AppendLine("\t};");
        sbParam1.AppendLine("\t};");
        sbParam2.AppendLine("\t};");
        sb.Append(sbName);
        sb.Append(sbParam1);
        sb.Append(sbParam2);
        sb.AppendLine("\tpublic static void Do(string name, object param1, object param2)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\ttry");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tswitch (name)");
        sb.AppendLine("\t\t\t{");
        for (int i = 0; i < list.Count; i++)
        {
            var info = list[i];
            sb.AppendLine("\t\t\t\tcase \"{0}\":".Fmt(info.Name));
            if(paramList2[i] != null && paramList1[i] != null)
                sb.AppendLine("\t\t\t\t\t{0}(({1})param1, ({2})param2);".Fmt(info.Name, paramList1[i], paramList2[i]));
            else if(paramList1[i] != null)
                sb.AppendLine("\t\t\t\t\t{0}(({1})param1);".Fmt(info.Name, paramList1[i]));
            else
                sb.AppendLine("\t\t\t\t\t{0}();".Fmt(info.Name));
            sb.AppendLine("\t\t\t\t\tbreak;");
        }
        sb.AppendLine("\t\t\t}");
        sb.AppendLine("\t\t}");
        sb.AppendLine("\t\tcatch (System.Exception e)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tWLogger.Exception(e);");
        sb.AppendLine("\t\t\tthrow;");
        sb.AppendLine("\t\t}");
        sb.AppendLine("\t}");
        sb.AppendLine("}");
        using FileStream fs = new FileStream(path, FileMode.Create);
        using StreamWriter sw = new StreamWriter(fs);
        sw.Write(sb.ToString());
        WLogger.Info("已生成，数量:"+list.Count);
    }
}
