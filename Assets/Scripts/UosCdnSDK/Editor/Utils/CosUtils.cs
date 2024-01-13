using System;
using System.Collections.Generic;
using COSXML;
using COSXML.Auth;
using COSXML.Model.Object;
using COSXML.Utils;
using COSXML.Model.Tag;
using UnityEngine;

namespace UosCdn
{
    public class CosUtils
    {

        public static CosXml cosXml = getCosXml();

        public static void uploadSingle(string objectKey, string srcPath)
        {
            try
            {
                PutObjectRequest request = new PutObjectRequest(Parameters.CosBucket, objectKey, srcPath);
                request.SetSign(TimeUtils.GetCurrentTime(TimeUnit.SECONDS), 600);
                request.SetCosProgressCallback(delegate (long completed, long total)
                {
                    ParametersUpload.alreadyUploadSize4Current = completed;
                });

                checkAuth();
                PutObjectResult result = getCosXml().PutObject(request);
            }
            catch (COSXML.CosException.CosClientException clientEx)
            {
                Debug.LogError("CosClientException: " + clientEx);
                throw clientEx;
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                Debug.LogError("CosServerException: " + serverEx.GetInfo());
                throw serverEx;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void uploadMulti(EntryInfo entryInfo)
        {
            string objectKey = entryInfo.objectKey;

            int index = -1;
            if (ParametersUpload.unfinishedIndexObjetkeyMapping.ContainsKey(objectKey))
            {
                index = ParametersUpload.unfinishedIndexObjetkeyMapping[objectKey];
            }
            UploadParts partsToUpload = SliceContent(entryInfo, index == -1 ? null : ParametersUpload.unfinishedUploadList.unfinishedList[index].uploadId);

            partsToUpload = CheckUploadParts(partsToUpload);

            if (index == -1)
            {
                UploadPartsStatus ups = new UploadPartsStatus();
                ups.uploadId = partsToUpload.uploadId;
                ups.objectKey = objectKey;
                ParametersUpload.unfinishedUploadList.unfinishedList.Add(ups);
                ParametersUpload.unfinishedIndexObjetkeyMapping.Add(objectKey, ParametersUpload.unfinishedUploadList.unfinishedList.Count - 1);
                EntryController.WriteUnfinishedUploadsIntoFile();
            }

            uploadParts(partsToUpload);
        }

        private static void uploadParts(UploadParts uploadParts)
        {

            for (int i = 0; i < uploadParts.totalParts; i++)
            {
                PartStructure part = uploadParts.parts[i];
                if (part.hasUploaded)
                {
                    ParametersUpload.alreadyUploadPartsSize4Current += part.partLength;
                    continue;
                }
                string etag = uploadPart(uploadParts.objectKey, part.partId, uploadParts.uploadId,
                    uploadParts.fullPath, part.partStart, part.partLength);

                part.hasUploaded = true;
                part.eTag = etag;
            }

            completeUpload(uploadParts);
        }

        private static UploadParts CheckUploadParts(UploadParts uploadParts)
        {
            if (uploadParts.uploadId != null)
            {
                return ListMultiParts(uploadParts);
            }
            else
            {
                return InitMultiUploadPart(uploadParts);
            }
        }

        private static UploadParts ListMultiParts(UploadParts uploadParts)
        {
            List<ListParts.Part> alreadyUploadParts = getUploadsAlready(uploadParts.objectKey, uploadParts.uploadId);
            if (alreadyUploadParts != null)
            {
                Dictionary<int, string> remoteParts = new Dictionary<int, string>();
                foreach (ListParts.Part part in alreadyUploadParts)
                {
                    int num = -1;
                    bool parse = int.TryParse(part.partNumber, out num);
                    if (!parse) throw new ArgumentException("ListParts.Part parse error");

                    remoteParts.Add(num, part.eTag);
                }

                foreach (PartStructure part in uploadParts.parts)
                {
                    if (remoteParts.ContainsKey(part.partId))
                    {
                        part.hasUploaded = true;
                        part.eTag = remoteParts[part.partId];
                    }
                    else
                    {
                        part.hasUploaded = false;
                        part.eTag = null;
                    }
                }
            }

            return uploadParts;
        }

        private static UploadParts InitMultiUploadPart(UploadParts uploadParts)
        {
            string uploadId = getUploadId(uploadParts.objectKey);
            uploadParts.uploadId = uploadId;
            return uploadParts;
        }

        private static UploadParts SliceContent(EntryInfo entryInfo, string uploadId = null)
        {

            long size = entryInfo.content_size;

            List<PartStructure> parts = new List<PartStructure>();

            int i = 1;
            while (size > 0)
            {
                PartStructure part = new PartStructure();
                part.partId = i;
                part.hasUploaded = false;
                part.partStart = entryInfo.content_size - size;
                part.partLength = size > UploadConfig.sliceSizeForUpload ? UploadConfig.sliceSizeForUpload : size;
                part.partEnd = part.partStart + part.partLength - 1;
                part.eTag = null;
                parts.Add(part);

                size = size - UploadConfig.sliceSizeForUpload;
                i++;
            }

            UploadParts uploadParts = new UploadParts(entryInfo, parts);
            if (uploadId != null)
            {
                uploadParts.uploadId = uploadId;
            }
            return uploadParts;
        }

        private static string uploadPart(string objectKey, int partNum, string uploadId, string srcPath, long start, long contentLength)
        {
            try
            {
                UploadPartRequest uploadPartRequest = new UploadPartRequest(Parameters.CosBucket, objectKey, partNum, uploadId, srcPath, start, contentLength);
                uploadPartRequest.SetSign(TimeUtils.GetCurrentTime(TimeUnit.SECONDS), 600);
                uploadPartRequest.SetCosProgressCallback(delegate (long completed, long total)
                {
                    ParametersUpload.alreadyUploadSize4Current = ParametersUpload.alreadyUploadPartsSize4Current + completed;
                });

                checkAuth();
                UploadPartResult result = cosXml.UploadPart(uploadPartRequest);

                ParametersUpload.alreadyUploadPartsSize4Current += contentLength;
                return result.eTag;
            }
            catch (COSXML.CosException.CosClientException clientEx)
            {
                Debug.LogError("CosClientException: " + clientEx);
                throw clientEx;
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                Debug.LogError("CosServerException: " + serverEx.GetInfo());
                throw serverEx;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static string getUploadId(string objectKey)
        {
            try
            {
                InitMultipartUploadRequest request = new InitMultipartUploadRequest(Parameters.CosBucket, objectKey);
                request.SetSign(TimeUtils.GetCurrentTime(TimeUnit.SECONDS), 600);

                checkAuth();
                InitMultipartUploadResult result = cosXml.InitMultipartUpload(request);
                
                return result.initMultipartUpload.uploadId;
            }
            catch (COSXML.CosException.CosClientException clientEx)
            {
                Debug.LogError("CosClientException: " + clientEx);
                throw clientEx;
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                Debug.LogError("CosServerException: " + serverEx.GetInfo());
                throw serverEx;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static List<COSXML.Model.Tag.ListParts.Part> getUploadsAlready(string objectKey, string uploadId)
        {
            try
            {
                ListPartsRequest request = new ListPartsRequest(Parameters.CosBucket, objectKey, uploadId);
                request.SetSign(TimeUtils.GetCurrentTime(TimeUnit.SECONDS), 600);

                checkAuth();
                ListPartsResult result = cosXml.ListParts(request);

                List<COSXML.Model.Tag.ListParts.Part> alreadyUploadParts = result.listParts.parts;
                return alreadyUploadParts;
            }
            catch (COSXML.CosException.CosClientException clientEx)
            {
                Debug.LogError("CosClientException: " + clientEx);
                throw clientEx;
            }

            catch (COSXML.CosException.CosServerException serverEx)
            {
                Debug.LogError("CosServerException: " + serverEx.GetInfo());
                throw serverEx;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private static void completeUpload(UploadParts uploadParts)
        {
            try
            {
                CompleteMultipartUploadRequest request = new CompleteMultipartUploadRequest(Parameters.CosBucket,
    uploadParts.objectKey, uploadParts.uploadId);
                request.SetSign(TimeUtils.GetCurrentTime(TimeUnit.SECONDS), 600);

                for (int i = 0; i < uploadParts.totalParts; i++)
                {
                    PartStructure part = uploadParts.parts[i];
                    request.SetPartNumberAndETag(part.partId, part.eTag);
                }

                checkAuth();
                CompleteMultipartUploadResult res = cosXml.CompleteMultiUpload(request);

                // Debug.Log(res.GetResultInfo());
            }
            catch (COSXML.CosException.CosClientException clientEx)
            {
                Debug.LogError("CosClientException: " + clientEx);
                throw clientEx;
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                Debug.LogError("CosServerException: " + serverEx.GetInfo());
                throw serverEx;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public static CosXml getCosXml()
        {
            CosXmlConfig config = new CosXmlConfig.Builder()
                .SetConnectionTimeoutMs(60000)  //设置连接超时时间，单位毫秒，默认45000ms
                .SetReadWriteTimeoutMs(40000)  //设置读写超时时间，单位毫秒，默认45000ms
                .IsHttps(true)  //设置默认 HTTPS 请求
                .SetAppid(Parameters.CosAppId) //设置腾讯云账户的账户标识 APPID
                .SetRegion(Parameters.CosRegion) //设置一个默认的存储桶地域
                .Build();

            DefaultSessionQCloudCredentialProvider qCloudCredentialProvider = new DefaultSessionQCloudCredentialProvider(TemporaryAuth.secretId,
               TemporaryAuth.secretKey, TemporaryAuth.expiredTime, TemporaryAuth.token);

            return new CosXmlServer(config, qCloudCredentialProvider);
        }

        private static void checkAuth()
        {
            if (TemporaryAuth.isNearExpired())
            {
                try
                {
                    TemporaryAuth.refresh();
                    cosXml = getCosXml();
                }
                catch (Exception e)
                {
                    Debug.Log("Refresh temporary token failed.");
                    throw e;
                }
            }
        }
    }
}
