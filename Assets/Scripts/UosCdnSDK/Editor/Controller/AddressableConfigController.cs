using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

#if ADDRESSABLES_EXISTS
using UnityEditor.AddressableAssets;
#endif

namespace UosCdn
{
    public class AddressableConfigController
    {
        public static ParametersBucket pb = ParametersBucket.GetParametersBucket();
        public static ParametersAddressableConfig pac = ParametersAddressableConfig.GetParametersAddressableConfig();

        public static string GetRemoteLoadPath(string badgeName)
        {
            if (string.IsNullOrEmpty(pb.selectedBucketUuid))
            {
                return "";
            }

            if (string.IsNullOrEmpty(badgeName) || UOSCdnSettings.Settings.useLatest)
            {
                badgeName = "latest";
            }

            string host = Parameters.proxyHost[Parameters.backend];
            string remoteLoadPath = host + "client_api/v1/buckets/" + pb.selectedBucketUuid + "/release_by_badge/" + badgeName + "/entry_by_path/content/?path=";
            return remoteLoadPath;
        }
        
        public static void SetAddressablesProfile(string badgename)
        {
#if ADDRESSABLES_EXISTS
            try
            {
                if (AddressableAssetSettingsDefaultObject.Settings != null)
                {
                    var remoteLoadUrl = "{UOS.cdn_url}/?path=";
                    string profileId = AddressableAssetSettingsDefaultObject.Settings.activeProfileId;
                    AddressableAssetSettingsDefaultObject.Settings.profileSettings.SetValue(profileId, UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.kRemoteLoadPath, remoteLoadUrl);
                    WriteCdsUrlFile(badgename);
                }
                else
                {
                    EditorUtility.DisplayDialog("Addressables Init Warning", "Please init Addressables package first. (Window -> Asset Management -> Addressables -> Groups)", "OK");
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log("Set addressables profile error. Maybe the addressables package version is incompatible");
            }
#endif
        }

        public static void WriteCdsUrlFile(string badgeName)
        {
#if ADDRESSABLES_EXISTS
            if (string.IsNullOrEmpty(pb.selectedBucketUuid))
            {
                return;
            }
            if (string.IsNullOrEmpty(badgeName) || UOSCdnSettings.Settings.useLatest)
            {
                badgeName = "latest";
            }
            
            var host = Parameters.proxyHost[Parameters.backend];
            var cdn_url = host + "client_api/v1/buckets/" + pb.selectedBucketUuid + "/release_by_badge/" + badgeName + "/entry_by_path/content";
            
            var kSourcePath = AddressableAssetSettingsDefaultObject.kDefaultConfigFolder + "/cdnurl.cs";
            var codePath = Path.Combine(Application.dataPath, "../" + kSourcePath);
            FileUtil.DeleteFileOrDirectory(codePath);

            string code = string.Format("class UOS {{ public static string cdn_url = \"{0}\"; }}", cdn_url);
            System.IO.File.WriteAllText(codePath, code);
            AssetDatabase.ImportAsset(kSourcePath);
#endif
        }
    }
}