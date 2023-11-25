using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using WGame.Util;
using System.Text;

namespace WGame.UIEditor
{
    public static class FGUIViewDBSpawner
    {
        public static void SpawnViewDB(List<ComponentInfo> componentInfos)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FGUIViewDBSpawner. Please do not modify it. **/");
            sb.AppendLine();
            sb.AppendFormat("namespace {0}", FGUICodeSpawner.NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendLine("\tpublic class VDB");
            sb.AppendLine("\t{");
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            sb2.AppendLine("\t\tpublic static readonly System.Collections.Generic.Dictionary<string, System.Type> viewList = new()");
            sb2.AppendLine("\t\t{");
            foreach (var info in componentInfos)
            {
                if (info.PanelType == PanelType.Main)
                {
                    sb1.AppendFormat("\t\tpublic const string {0} = \"{0}\";\n", info.NameWithoutExtension);
                    sb2.AppendFormat("\t\t\t{{{0}, typeof({0})}},\n", info.NameWithoutExtension);
                }
            }
            sb2.AppendLine("\t\t};");
            sb.AppendLine(sb1.ToString());
            sb.AppendLine(sb2.ToString());
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            string filePath = $"{FGUICodeSpawner.FUIAutoGenDir}/VDB.cs";
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }
    }
}