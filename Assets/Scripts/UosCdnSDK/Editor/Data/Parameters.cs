using System;
using System.Collections.Generic;
using UnityEngine;

namespace UosCdn
{
    [Serializable]
    public class Parameters
    {
        //constant parameters
        public static string apiHost = "https://a-dev.unity.cn/";
        public static Dictionary<string, string> proxyHost = new Dictionary<string, string> {
            { "BlueCloud","https://a.unity.cn/"}
        };
        public static string contentType = "application/json";

        public static string k_oldUosInfoDirectory_1 = "Assets/UnityOnlineServiceData"; 
        public static string k_oldUosInfoDirectory_2 = "Assets/Editor/UnityOnlineServiceData"; 
        
        public static string k_UosCdnSettingsPathPrefix = "Assets/Editor/UnityOnlineServicesData/";
        public static string k_UosSettingsPath = "Assets/Editor/UnityOnlineServicesData/UOSSettings.asset";
        public static string k_UosCdnSettingsPath = "Assets/Editor/UnityOnlineServicesData/UOSCdnSettings.asset";
        public static string k_UploadPartStatusPathPrefix = "Assets/Editor/UnityOnlineServicesData/";
        public static string k_UploadPartStatusFile = "Assets/Editor/UnityOnlineServicesData/unfinishedUploads.json";
        
        public static string CosAppId = "1301029430";
        public static string CosRegion = "ap-shanghai";
        public static string CosBucket = "asset-streaming-1301029430";
        public static int maxRetries = 1;
        public static int createMultiCount = 20;
        public static int deleteMultiCount = 20;

        //static parameters
        public static List<string> ignoreFiles = new List<string>()
        {
            ".DS_Store"
        };
        
        public static string uosAppId = UOSAppInfo.getUosAppId();
        public static string uosAppServiceSecret = UOSAppInfo.getUosAppServiceSecret();
        public static string backend = "BlueCloud";
        
        public static int countPerpage = 10;
    }
}