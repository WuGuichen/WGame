using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace UosCdn
{
    public class UOSCdnSettings : ScriptableObject
    {
        [SerializeField]
        public bool useLatest;
        
        [SerializeField]
        public bool syncWithDelete;

        private static UOSCdnSettings m_settings;

        public static UOSCdnSettings Settings
        {
            get
            {
                if (m_settings != null)
                {
                    return m_settings;
                }

                if (false == Directory.Exists(Parameters.k_UosCdnSettingsPathPrefix))
                {
                    Directory.CreateDirectory(Parameters.k_UosCdnSettingsPathPrefix);
                }

                m_settings = AssetDatabase.LoadAssetAtPath<UOSCdnSettings>(Parameters.k_UosCdnSettingsPath);
                if (m_settings == null)
                {
                    m_settings = ScriptableObject.CreateInstance<UOSCdnSettings>();
                    m_settings.useLatest = true;
                    m_settings.syncWithDelete = false;
                    AssetDatabase.CreateAsset(m_settings, Parameters.k_UosCdnSettingsPath);
                    AssetDatabase.SaveAssets();
                }

                return m_settings;

            }
        }
    }
}
