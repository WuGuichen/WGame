using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using WGame.Util;

namespace WGame.UIEditor
{
    public static class FGUIModelDefineSpawner
    {
        public static void SpawnModeAndDefineCode(string packageName)
        {
            string fileDir = FGUICodeSpawner.ModelCodeDir;
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            string className = packageName + "Model";
            string filePath = "{0}/{1}.cs".Fmt(fileDir, className);
            if (!File.Exists(filePath))
            {
                Debug.Log("Spawn Code For Model {0}".Fmt(filePath));
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("// 数据缓存，数据逻辑操作");
                sb.AppendLine();
                // sb.AppendLine();
                sb.AppendFormat("namespace {0}", FGUICodeSpawner.NameSpace);
                sb.AppendLine();
                sb.AppendLine("{");
                sb.AppendFormat("\tpublic class {0} : Runtime.Singleton<{0}>", className);
                sb.AppendLine();
                sb.AppendLine("\t{");
                sb.AppendLine();
                sb.AppendLine("\t}");
                sb.AppendLine("}");

                using FileStream fs = new FileStream(filePath, FileMode.Create);
                using StreamWriter sw = new StreamWriter(fs);
                sw.Write(sb.ToString());
                sb.Clear();
                // Debug.Log("{0} 已经存在".Fmt(filePath));
            }
            fileDir = FGUICodeSpawner.DefineCodeDir;
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            className = packageName + "Define";
            filePath = "{0}/{1}.cs".Fmt(fileDir, className);
            if (!File.Exists(filePath))
            {
                Debug.Log("Spawn Code For Model {0}".Fmt(filePath));
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("// 数据定义");
                sb.AppendLine();
                // sb.AppendLine();
                sb.AppendFormat("namespace {0}", FGUICodeSpawner.NameSpace);
                sb.AppendLine();
                sb.AppendLine("{");
                sb.AppendFormat("\tpublic class {0} : Runtime.Singleton<{0}>", className);
                sb.AppendLine();
                sb.AppendLine("\t{");
                sb.AppendLine();
                sb.AppendLine("\t}");
                sb.AppendLine("}");

                using FileStream fs = new FileStream(filePath, FileMode.Create);
                using StreamWriter sw = new StreamWriter(fs);
                sw.Write(sb.ToString());
                sb.Clear();
                // Debug.Log("{0} 已经存在".Fmt(filePath));
            }
        }
    }
}
