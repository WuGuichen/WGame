using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace UosCdn
{
    public class UOSAppInfo : ScriptableObject
    {
        [SerializeField] 
        private string UOSAppId;
        
        [SerializeField] 
        private string UOSAppServiceSecret;

        internal static UOSAppInfo GetOrCreateSetting()
        {
            if (false == Directory.Exists(Parameters.k_UosCdnSettingsPathPrefix))
            {
                Directory.CreateDirectory(Parameters.k_UosCdnSettingsPathPrefix);
            }
            var setting = AssetDatabase.LoadAssetAtPath<UOSAppInfo>(Parameters.k_UosSettingsPath);
            if (setting == null)
            {
                setting = ScriptableObject.CreateInstance<UOSAppInfo>();
                setting.UOSAppId = "";
                setting.UOSAppServiceSecret = "";
                AssetDatabase.CreateAsset(setting, Parameters.k_UosSettingsPath);
                AssetDatabase.SaveAssets();
            }

            return setting;
        }

        internal static void SaveSetting(string uosAppId, string uosAppServiceSecret)
        {
            var setting = AssetDatabase.LoadAssetAtPath<UOSAppInfo>(Parameters.k_UosSettingsPath);
            setting.UOSAppId = uosAppId;
            setting.UOSAppServiceSecret = uosAppServiceSecret;
            EditorUtility.SetDirty(setting);
        }

        public static string getUosAppId()
        {
            return GetOrCreateSetting().UOSAppId;
        }

        public static string getUosAppServiceSecret()
        {
            return GetOrCreateSetting().UOSAppServiceSecret;
        }
        
        static class UosCdnSettingsIMGUIRegister
        {
            [SettingsProvider]
            public static SettingsProvider CreateUosCdnSettingsProvider()
            {
                var provider = new SettingsProvider("Project/Unity Online Services", SettingsScope.Project)
                {
                    label = "Unity Online Services",
                    guiHandler = (searchContext) =>
                    {
                        var setting = UOSAppInfo.GetOrCreateSetting();
                        setting.UOSAppId = EditorGUILayout.TextField("App Id", setting.UOSAppId);
                        setting.UOSAppServiceSecret = EditorGUILayout.TextField("App Service Secret", setting.UOSAppServiceSecret);
                        if (!setting.UOSAppId.Equals(Parameters.uosAppId) || !setting.UOSAppServiceSecret.Equals(Parameters.uosAppServiceSecret))
                        {
                            UOSAppInfo.SaveSetting(setting.UOSAppId, setting.UOSAppServiceSecret);
                            Parameters.uosAppId = setting.UOSAppId;
                            Parameters.uosAppServiceSecret = setting.UOSAppServiceSecret;
                        }
                        
                        if (GUILayout.Button("Go To Unity Online Services Dashboard."))
                        {
                            Application.OpenURL("https://uos.unity.cn/");
                        }
                    },
                    keywords = new HashSet<string>(new[] { "UOS" })
                };
                
                AssetDatabase.SaveAssets();
                return provider;
            }
        }
    }
}