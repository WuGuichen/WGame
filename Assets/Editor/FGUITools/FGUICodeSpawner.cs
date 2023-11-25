using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WGame.Util;
using System.IO;
using System.Linq;
using FairyGUI.Utils;
using UnityEditor;

namespace WGame.UIEditor
{
    public static class FGUICodeSpawner
    {
        // 名字空间
        public static string NameSpace = "WGame.UI";

        // 类名前缀
        public static string ClassNamePrefix = "FUI_";
        
        // 初始化函数名
        public static string InitFunctionName = "InitSelf";

        // 代码生成路径
        public const string FUIAutoGenDir = "Assets/Scripts/ECS/Module/ModelView/FGUIAutoGen";
        public const string ModelViewCodeDir = "Assets/Scripts/ECS/Module/ModelView/FGUI";
        public const string ModelCodeDir = "Assets/Scripts/ECS/Module/Model";
        public const string DefineCodeDir = "Assets/Scripts/ECS/Module/Define";
        public const string HotfixViewCodeDir = "Assets/Scripts/ECS/Module/HotfixView/FGUI";

        // 不生成使用默认名称的成员
        public static readonly bool IgnoreDefaultVariableName = true;

        public static readonly Dictionary<string, PackageInfo> PackageInfos = new Dictionary<string, PackageInfo>();

        public static readonly Dictionary<string, ComponentInfo> ComponentInfos =
            new Dictionary<string, ComponentInfo>();

        public static readonly List<ComponentInfo> MainPanelComponentInfos = new List<ComponentInfo>();

        public static readonly MultiDictionary<string, string, ComponentInfo> ExportedComponentInfos =
            new MultiDictionary<string, string, ComponentInfo>();

        private static readonly HashSet<string> ExtralExportURLs = new HashSet<string>();

        public static bool Localize(string xmlPath)
        {
            if (string.IsNullOrEmpty(xmlPath) || !File.Exists(xmlPath))
            {
                Debug.LogError("没有提供语言文件！可查看此文档来生成：https://www.fairygui.com/docs/editor/i18n");
                return false;
            }

            FGUILocalizeHandler.Localize(xmlPath);
            return true;
        }
        
        public static void FUICodeSpawn()
        {
            ParseAndSpawnCode();

            AssetDatabase.Refresh();
        }

        private static void ParseAndSpawnCode()
        {
            ParseAllPackages();
            AfterParseAllPackages();
            SpawnCode();
        }

        private static void ParseAllPackages()
        {
            PackageInfos.Clear();
            ComponentInfos.Clear();
            MainPanelComponentInfos.Clear();
            ExportedComponentInfos.Clear();
            ExtralExportURLs.Clear();

            string fuiAssetsDir = Application.dataPath + "/../FGUIProject/assets";
            string[] packageDirs = Directory.GetDirectories(fuiAssetsDir);
            foreach (var packageDir in packageDirs)
            {
                PackageInfo packageInfo = ParsePackage(packageDir);
                if (packageInfo == null)
                {
                    continue;
                }
                
                PackageInfos.Add(packageInfo.Id, packageInfo);
            }
        }

        private static PackageInfo ParsePackage(string packageDir)
        {
            PackageInfo packageInfo = new PackageInfo();

            packageInfo.Path = packageDir;
            packageInfo.Name = Path.GetFileName(packageDir);

            string packageXmlPath = $"{packageDir}/package.xml";
            if (!File.Exists(packageXmlPath))
            {
                Debug.LogWarning($"{packageXmlPath} 不存在！");
                return null;
            }
            
            XML xml = new XML(File.ReadAllText(packageDir + "/package.xml"));
            packageInfo.Id = xml.GetAttribute("id");

            if (xml.elements[0].name != "resources")
            {
                throw new System.Exception("package.xml 格式不对！");
            }
            
            foreach (XML element in xml.elements[0].elements)
            {
                if (element.name != "component")
                {
                    continue;
                }
                
                PackageComponentInfo packageComponentInfo = new PackageComponentInfo();
                packageComponentInfo.Id = element.GetAttribute("id");
                packageComponentInfo.Name = element.GetAttribute("name");
                packageComponentInfo.Path = "{0}{1}{2}".Fmt(packageDir, element.GetAttribute("path"), packageComponentInfo.Name);
                packageComponentInfo.Exported = element.GetAttribute("exported") == "true";
                
                packageInfo.PackageComponentInfos.Add(packageComponentInfo.Name, packageComponentInfo);

                ComponentInfo componentInfo = ParseComponent(packageInfo, packageComponentInfo);
                string key = "{0}/{1}".Fmt(componentInfo.PackageId, componentInfo.Id);
                ComponentInfos.Add(key, componentInfo);

                if (componentInfo.PanelType == PanelType.Main)
                {
                    MainPanelComponentInfos.Add(componentInfo);    
                }
            }

            return packageInfo;
        }

        private static ComponentInfo ParseComponent(PackageInfo packageInfo, PackageComponentInfo packageComponentInfo)
        {
            ComponentInfo componentInfo = new ComponentInfo();
            componentInfo.PackageId = packageInfo.Id;
            componentInfo.Id = packageComponentInfo.Id;
            componentInfo.Name = packageComponentInfo.Name;
            componentInfo.NameWithoutExtension = Path.GetFileNameWithoutExtension(packageComponentInfo.Name);
            componentInfo.Url = "ui://{0}{1}".Fmt(packageInfo.Id, packageComponentInfo.Id);
            componentInfo.Exported = packageComponentInfo.Exported;
            componentInfo.ComponentType = ComponentType.Component;

            XML xml = new XML(File.ReadAllText(packageComponentInfo.Path));

            if (xml.attributes.TryGetValue("extention", out string typeName))
            {
                ComponentType type = EnumHelper.FromString<ComponentType>(typeName);
                if (type == ComponentType.None)
                {
                    Debug.LogError("{0}类型没有处理！".Fmt(typeName));
                }
                else
                {
                    componentInfo.ComponentType = type;
                }
            }
            else if (xml.attributes.TryGetValue("remark", out string remark))
            {
                if (Enum.TryParse(remark, out PanelType panelType))
                {
                    componentInfo.PanelType = panelType;
                }
            }

            foreach (XML element in xml.elements)
            {
                if (element.name == "displayList")
                {
                    componentInfo.DisplayList = element.elements;
                }
                else if (element.name == "controller")
                {
                    componentInfo.ControllerList.Add(element);
                }
                else if (element.name == "transition")
                {
                    componentInfo.TransitionList.Add(element);
                }
                else if (element.name == "relation")
                { 
                    
                }
                else if (element.name == "customProperty")
                { 
                    
                }
                else
                {
                    if (element.name == "ComboBox" && componentInfo.ComponentType == ComponentType.ComboBox)
                    {
                        ExtralExportURLs.Add(element.GetAttribute("dropdown"));
                    }
                }
            }

            return componentInfo;
        }
        
        // 检查哪些组件可以导出。需要在 ParseAllPackages 后执行，因为需要有全部 package 的信息。
        private static void AfterParseAllPackages()
        {
            foreach (ComponentInfo componentInfo in ComponentInfos.Values)
            {
                componentInfo.CheckCanExport(ExtralExportURLs, IgnoreDefaultVariableName);
            }
            
            foreach (ComponentInfo componentInfo in ComponentInfos.Values)
            {
                componentInfo.SetVariableInfoTypeName();
            }
        }
        
        private static void SpawnCode()
        {
            if (Directory.Exists(FUIAutoGenDir)) 
            {
                Directory.Delete(FUIAutoGenDir, true);
            }
            
            foreach (ComponentInfo componentInfo in ComponentInfos.Values)
            {
                FGUIComponentSpawner.SpawnComponent(componentInfo);
            }
            
            List<PackageInfo> ExportedPackageInfos = new List<PackageInfo>();
            foreach (var kv in ExportedComponentInfos)
            {
                FGUIBinderSpawner.SpawnCodeForPanelBinder(PackageInfos[kv.Key], kv.Value);
                
                ExportedPackageInfos.Add(PackageInfos[kv.Key]);
            }

            FGUIPackageHelperSpawner.GenerateMappingFile();
            FGUIPanelIdSpawner.SpawnPanelId();
            FGUIBinderSpawner.SpawnFUIBinder(ExportedPackageInfos);

            foreach (var kv in ComponentInfos)
            {
                ComponentInfo componentInfo = kv.Value;
                if (componentInfo.PanelType == PanelType.Main)
                {
                    PackageInfo packageInfo = PackageInfos[componentInfo.PackageId];
                    
                    SpawnSubPanelCode(componentInfo);

                    FGUIPanelSpawner.SpawnPanel(packageInfo.Name, componentInfo);
                        
                    // FGUIPanelSystemSpawner.SpawnPanelSystem(packageInfo.Name, componentInfo);
                    //     
                    // FGUIEventHandlerSpawner.SpawnEventHandler(packageInfo.Name, componentInfo);
                    
                    FGUIModelDefineSpawner.SpawnModeAndDefineCode(packageInfo.Name);
                }
            }
            FGUIViewDBSpawner.SpawnViewDB(ComponentInfos.Values.ToList());
        }

        private static void SpawnSubPanelCode(ComponentInfo componentInfo)
        {
            componentInfo.VariableInfos.ForEach(variableInfo =>
            {
                if (!variableInfo.IsExported || variableInfo.ComponentInfo == null)
                {
                    return;
                }
                
                string subPackageName = PackageInfos[variableInfo.PackageId].Name;

                FGUIPanelSpawner.SpawnSubPanel(subPackageName, variableInfo.ComponentInfo);
                // FGUIPanelSystemSpawner.SpawnPanelSystem(subPackageName, variableInfo.ComponentInfo, variableInfo);
                
                SpawnSubPanelCode(variableInfo.ComponentInfo);
            });
        }
    }
}