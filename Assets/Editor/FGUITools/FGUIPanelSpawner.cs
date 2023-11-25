using System;
using System.IO;
using System.Text;
using WGame.Util;
using UnityEngine;

namespace WGame.UIEditor
{
    public static class FGUIPanelSpawner
    {
        public static void SpawnSubPanel(string packageName, ComponentInfo componentInfo)
        {
            string fileDir = "{0}/{1}".Fmt(FGUICodeSpawner.ModelViewCodeDir, packageName);
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }
            
            string componentName = componentInfo.NameWithoutExtension;
            string nameSpace = componentInfo.NameSpace;
            string filePath = "{0}/{1}.cs".Fmt(fileDir, componentName);
            if (File.Exists(filePath))
            {
                // Debug.Log("{0} 已经存在".Fmt(filePath));
                return;
            }
            
            Debug.Log("Spawn Code For SubComponent {0}".Fmt(filePath));
            
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"using {nameSpace};");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"namespace {FGUICodeSpawner.NameSpace}");
            sb.AppendLine("{");
            sb.AppendLine("\t[ChildOf]");
            sb.AppendLine($"\tpublic class {componentName}: Entity, IAwake<FUI_{componentName}>");
            sb.AppendLine("\t{");
            
            // 子组件
            componentInfo.VariableInfos.ForEach(variableInfo =>
            {
                if (!variableInfo.IsExported)
                {
                    return;
                }

                if (variableInfo.ComponentInfo?.PanelType != PanelType.Common)
                {
                    return;
                }

                int index = variableInfo.TypeName.IndexOf("FUI_", StringComparison.Ordinal);
                string comName = variableInfo.TypeName.Substring(index + 4);
                sb.AppendLine($"\t\tpublic {comName} {variableInfo.VariableName} {{get; set;}}");
            });
            
            sb.AppendLine($"\t\tpublic FUI_{componentName} FUI{componentName} {{ get; set; }}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            
            File.WriteAllText(filePath, sb.ToString());
        }
        
        public static void SpawnPanel(string packageName, ComponentInfo componentInfo)
        {
            string nameSpace = componentInfo.NameSpace;
            string panelName = componentInfo.NameWithoutExtension;
            
            string fileDir = "{0}/{1}".Fmt(FGUICodeSpawner.ModelViewCodeDir, packageName);
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }
            
            string filePath = "{0}/{1}.cs".Fmt(fileDir, panelName);
            if (File.Exists(filePath))
            {
                // Debug.Log("{0} 已经存在".Fmt(filePath));
                return;
            }
            
            Debug.Log("Spawn Code For Panel {0}".Fmt(filePath));
            
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"using {nameSpace};");
            sb.AppendFormat($"using FairyGUI;");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendFormat("namespace {0}", FGUICodeSpawner.NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
            // sb.AppendLine("\t[ComponentOf(typeof(FUIEntity))]");
            sb.AppendFormat("\tpublic class {0}: BaseView", panelName);
            sb.AppendLine();
            sb.AppendLine("\t{");
            sb.AppendFormat("\t\tpublic override string ViewName => \"{0}\";\n", panelName);
            
            // 子组件
            componentInfo.VariableInfos.ForEach(variableInfo =>
            {
                if (!variableInfo.IsExported)
                {
                    return;
                }

                if (variableInfo.ComponentInfo?.PanelType != PanelType.Common)
                {
                    return;
                }

                int index = variableInfo.TypeName.IndexOf("FUI_", StringComparison.Ordinal);
                string comName = variableInfo.TypeName.Substring(index + 4);
                sb.AppendLine($"\t\tpublic {comName} {variableInfo.VariableName} {{get; set;}}\n");
            });
            
            sb.AppendLine("\t\tprivate readonly FUI_{0} ui = FUI_{0}.CreateInstance();".Fmt(panelName));
            sb.AppendLine("\t\tprotected override GObject uiObj => ui;");
            sb.AppendLine();
            
            sb.AppendLine("\t\tprotected override void CustomInit()");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\t");
            sb.AppendLine("\t\t}");
            
            sb.AppendLine("\t\tprotected override void AfterOpen()");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\t");
            sb.AppendLine("\t\t}");
            
            sb.AppendLine("\t\tprotected override void BeforeClose()");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\t");
            sb.AppendLine("\t\t}");
            
            sb.AppendLine("\t\tprotected override void OnDestroy()");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\t");
            sb.AppendLine("\t\t}");
            
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }

    }
}