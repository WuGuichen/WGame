using System;
using System.IO;
using System.Net;
using System.Text;
using COSXML.Utils;
using UnityEditor;
using UnityEngine;

namespace UosCdn
{
    [Serializable]
    public class TemporaryAuth
    {
        public static string secretId = "";
        public static string secretKey = "";
        public static string token = "";
        public static long startTime = 0;
        public static long expiredTime = 0;

        public static string ToMsg()
        {
            return string.Format("Id: {0}, Key: {1}, Token: {2}, startTime: {3}, expiredTime: {4}",
                secretId, secretKey, token, startTime, expiredTime);
        }

        public static bool isNearExpired()
        {
            return TimeUtils.GetCurrentTime(TimeUnit.SECONDS) >= expiredTime - 120;
        }

        public static void refresh()
        {
            try
            {
                string respJson = HttpUtil.getHttpResponse(Parameters.apiHost + "api/v1/users/me/temporaryAuth", "GET");
                TemporaryAuthResponse tmpAuth = JsonUtility.FromJson<TemporaryAuthResponse>(respJson);
                secretId = tmpAuth.tmpId;
                secretKey = tmpAuth.tmpKey;
                token = tmpAuth.tmpToken;
                startTime = tmpAuth.startTime;
                expiredTime = tmpAuth.expiredTime;
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Refresh temporary auth error : {0}", e.Message));
            }            
        }
    }
}
