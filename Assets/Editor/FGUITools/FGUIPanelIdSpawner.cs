using System.IO;
using System.Text;
using WGame.Util;

namespace WGame.UIEditor
{
    public static class FGUIPanelIdSpawner
    {
        public static void SpawnPanelId()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            sb.AppendFormat("namespace {0}", FGUICodeSpawner.NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendLine("\tpublic enum PanelId");
            sb.AppendLine("\t{");

            sb.AppendLine("\t\tInvalid = 0,");

            foreach (ComponentInfo componentInfo in FGUICodeSpawner.MainPanelComponentInfos)
            {
                sb.AppendLine($"\t\t{componentInfo.NameWithoutExtension},");
            }
            
            sb.AppendLine("\t}"); 
            sb.AppendLine("}");
            
            string filePath = "{0}/PanelId.cs".Fmt(FGUICodeSpawner.FUIAutoGenDir);
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }
    }
}