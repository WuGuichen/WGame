using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using COSXML;
using COSXML.Auth;
using COSXML.Model.Object;
using COSXML.Utils;
using UnityEditor;
using UnityEngine;

namespace UosCdn
{
    public partial class Util
    {
        [InitializeOnLoadMethod]
        public static void RemoveOldData()
        {
            if (Directory.Exists(Parameters.k_oldUosInfoDirectory_1))
            {
                Directory.Delete(Parameters.k_oldUosInfoDirectory_1, true);
            }
            
            if (File.Exists(Parameters.k_oldUosInfoDirectory_1 + ".meta"))
            {
                Debug.Log("Delete " + Parameters.k_oldUosInfoDirectory_1 + ".meta");
                File.Delete(Parameters.k_oldUosInfoDirectory_1 + ".meta");
            }
            
            if (Directory.Exists(Parameters.k_oldUosInfoDirectory_2))
            {
                Directory.Delete(Parameters.k_oldUosInfoDirectory_2, true);
            }
            
            if (File.Exists(Parameters.k_oldUosInfoDirectory_2 + ".meta"))
            {
                Debug.Log("Delete " + Parameters.k_oldUosInfoDirectory_2 + ".meta");
                File.Delete(Parameters.k_oldUosInfoDirectory_2 + ".meta");
            }
        }
        
        public static Dictionary<string, EntryInfo> getLocalFiles(string rootPath)
        {
            Dictionary<string, EntryInfo> localFiles = new Dictionary<string, EntryInfo>();
            DirectoryInfo root = new DirectoryInfo(rootPath);
            rootPath = root.FullName.Replace("\\", "/");
            FileInfo[] files = root.GetFiles("*", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                string fullPath = file.FullName.Replace("\\", "/");
                string path = getRelativePath(rootPath, fullPath);
                long size = file.Length;
                string contentType = getContentTypeFromExtension(path);
                string hash = Util.getFiletHash(file.FullName);

                if (Parameters.ignoreFiles.Contains(file.Name))
                {
                    continue;
                }

                EntryInfo entry = new EntryInfo(fullPath, path, hash, size, contentType);
                localFiles.Add(path, entry);
            }

            return localFiles;
        }

        public static EntryInfo getEntryInfoFromLocalFile(string filepath)
        {
            FileInfo file = new FileInfo(filepath);

            string fullPath = file.FullName.Replace("\\", "/");
            string path = file.Name;
            long size = file.Length;
            string contentType = getContentTypeFromExtension(path);
            string hash = getFiletHash(file.FullName);

            return new EntryInfo(fullPath, path, hash, size, contentType);

        }

        public static string getRelativePath(string rootPath, string fullName)
        {
            return fullName.StartsWith(rootPath, StringComparison.Ordinal) ? fullName.Substring(rootPath.Length + 1) : "";
        }


        public static string getContentTypeFromExtension(string path)
        {
            var suffix = getSuffix(path);
            if (MimeTypes.ContainsKey("*" + suffix))
            {
                return MimeTypes["*" + suffix];
            } 
            if (MimeTypes.ContainsKey(suffix))
            {
                return MimeTypes[suffix];
            }
            return DefaultMimeType;
        }
        
        public static string getFiletHash(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public static ProjectInfo getProjectInfo()
        {
            if (string.IsNullOrEmpty(Parameters.uosAppId) || string.IsNullOrEmpty(Parameters.uosAppServiceSecret))
            {
                return null;
            }

            try
            {
                string responseBody = HttpUtil.getHttpResponse(Parameters.apiHost + "api/v1/users/me/uos/", "GET");
                ProjectInfo projectInfo = JsonUtility.FromJson<ProjectInfo>(responseBody);
                Debug.Log(string.Format("Refresh project info successfully : {0}", projectInfo.UnityProjectGuid));
                return projectInfo;
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Refresh project info failed : {0}", e.Message));
                return null;
            }
        }
        
        public static bool checkUosAuth()
        {
            if (string.IsNullOrEmpty(Parameters.uosAppId) || string.IsNullOrEmpty(Parameters.uosAppServiceSecret))
            {
                EditorUtility.DisplayDialog("Warning", "Please Set UOS Auth at Edit -> Project Settings -> Unity Online Services!", "OK");
                return false;
            }

            return true;
        }

        public static string getHeader(WebHeaderCollection headers, string key)
        {
            for (int i = 0; i < headers.Keys.Count; i++)
            {
                if (headers.Keys[i].Equals(key))
                {
                    return headers[i];
                }
            }
            return "";
        }
        
        private static string getSuffix(string path)
        {
            var words = path.Split('.');
            if (words.Length > 0)
            {
                return words[words.Length - 1];
            }

            return "";
        }
    }
}
