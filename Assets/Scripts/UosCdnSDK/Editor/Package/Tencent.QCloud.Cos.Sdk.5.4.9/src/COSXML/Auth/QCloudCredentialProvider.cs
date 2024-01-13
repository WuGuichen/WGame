﻿﻿using System;

using System.Text;
using COSXML.Utils;
using COSXML.CosException;
using COSXML.Common;
using COSXML.Log;
using System.IO;
/**
* Copyright (c) 2018 Tencent Cloud. All rights reserved.
* 11/9/2018 12:16:20 PM
* bradyxiao
*/
namespace COSXML.Auth
{

    public abstract class QCloudCredentialProvider
    {
        public abstract QCloudCredentials GetQCloudCredentials();

        public abstract void Refresh();
    }

    /// <summary>
    /// 通过 云 api
    /// </summary>
    public class DefaultQCloudCredentialProvider : QCloudCredentialProvider
    {
        private string secretId;

        private string secretKey;

        private long keyTimDuration;

        public DefaultQCloudCredentialProvider(string secretId, string secretKey, long keyDurationSecond)
        {
            this.secretId = secretId;
            this.secretKey = secretKey;
            this.keyTimDuration = keyDurationSecond;
        }

        public override QCloudCredentials GetQCloudCredentials()
        {
            long keyStartTime = TimeUtils.GetCurrentTime(TimeUnit.SECONDS);
            long keyEndTime = keyStartTime + keyTimDuration;
            string keyTime = String.Format("{0};{1}", keyStartTime, keyEndTime);
            if (secretId == null) throw new CosClientException((int)CosClientError.INVALID_CREDENTIALS, "secretId == null");
            if (secretKey == null) throw new CosClientException((int)CosClientError.INVALID_CREDENTIALS, "secretKey == null");
            string signKey = DigestUtils.GetHamcSha1ToHexString(keyTime, Encoding.UTF8, secretKey, Encoding.UTF8);
            return new QCloudCredentials(secretId, signKey, keyTime);
        }

        public override void Refresh()
        {
            //TODO update value
            QLog.D("DefaultQCloudCredentialProvider", "need to update QCloudCredentials");
            //invoke SetSetQCloudCredential(string secretId, string secretKey, string keyTime)
        }
    }

    /// <summary>
    /// 通过 临时密钥
    /// </summary>
    public class DefaultSessionQCloudCredentialProvider : QCloudCredentialProvider
    {
        private string tmpSecretId;
        private string tmpSecretKey;
        private string keyTime;
        private string token;

        public DefaultSessionQCloudCredentialProvider(string tmpSecretId, string tmpSecretKey, long tmpExpiredTime, string sessionToken)
            :this(tmpSecretId, tmpSecretKey, TimeUtils.GetCurrentTime(TimeUnit.SECONDS),tmpExpiredTime, sessionToken)
        {
        }

        public DefaultSessionQCloudCredentialProvider(string tmpSecretId, string tmpSecretKey, long keyStartTimeSecond, long tmpExpiredTime, string sessionToken)
        {
            this.tmpSecretId = tmpSecretId;
            this.tmpSecretKey = tmpSecretKey;
            this.keyTime = String.Format("{0};{1}", keyStartTimeSecond, tmpExpiredTime);
            this.token = sessionToken;
        }

        public override QCloudCredentials GetQCloudCredentials()
        {
            if (IsNeedUpdateNow()) Refresh();
            if (tmpSecretId == null) throw new CosClientException((int)CosClientError.INVALID_CREDENTIALS, "secretId == null");
            if (tmpSecretKey == null) throw new CosClientException((int)CosClientError.INVALID_CREDENTIALS, "secretKey == null");
            if (keyTime == null) throw new CosClientException((int)CosClientError.INVALID_CREDENTIALS, "keyTime == null");
            string signKey = DigestUtils.GetHamcSha1ToHexString(keyTime, Encoding.UTF8, tmpSecretKey, Encoding.UTF8);
            return new SessionQCloudCredentials(tmpSecretId, signKey, token, keyTime);
        }

        public override void Refresh()
        {
            //TODO update value
            QLog.D("DefaultSessionQCloudCredentialProvider", "need to update QCloudCredentials");
            //invoke SetQCloudCredential(string tmpSecretId, string tmpSecretKey, string tmpkeyTime, string sessionToken)
        }

        public bool IsNeedUpdateNow()
        {
            if (String.IsNullOrEmpty(keyTime) || String.IsNullOrEmpty(tmpSecretId) || String.IsNullOrEmpty(tmpSecretKey) || String.IsNullOrEmpty(token))
            {
                return true;
            }
            int index = keyTime.IndexOf(';');
            long endTime = -1L;
            long.TryParse(keyTime.Substring(index + 1), out endTime);
            long nowTime = TimeUtils.GetCurrentTime(TimeUnit.SECONDS);
            if (endTime <= nowTime) return true;
            return false;
        }

        /// <summary>
        /// 直接复制临时密钥信息
        /// </summary>
        /// <param name="tmpSecretId">临时安全证书 Id</param>
        /// <param name="tmpSecretKey">临时安全证书 Key</param>
        /// <param name="tmpkeyTime">证书有效的期间</param>
        /// <param name="sessionToken">token 值</param>
        public void SetQCloudCredential(string tmpSecretId, string tmpSecretKey, string tmpkeyTime, string sessionToken)
        {
            this.tmpSecretId = tmpSecretId;
            this.tmpSecretKey = tmpSecretKey;
            this.token = sessionToken;
            this.keyTime = tmpkeyTime;
        }   
    }


}
