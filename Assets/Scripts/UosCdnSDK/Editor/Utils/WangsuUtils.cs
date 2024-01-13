using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using Wangsu.WcsLib.Core;
using Wangsu.WcsLib.HTTP;
using Wangsu.WcsLib.Utility;

namespace UosCdn
{
    public class WangsuUtils
    {
        
        public const int MaxSingleFile = 200 * 1024 * 1024;
        public static ParametersBucket pb = ParametersBucket.GetParametersBucket();

        public static void uploadSingle(EntryInfo entryInfo)
        {
            Debug.Log("Single");
            Mac mac = new Mac("", "");
            Auth auth = new Auth(mac);
            Config config = new Config("unitycdn.up27.v1.wcsapi.com", "unitycdn.mgr27.v1.wcsapi.com", true);

            string uploadToken = getUploadToken(entryInfo);
            
            byte[] data;
            long size = 0;

            using (FileStream fs = new FileStream(entryInfo.full_path, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    size = fs.Length;
                    data = new byte[size];
                    fs.Read(data, 0, (int)size);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            
            SimpleUpload su = new SimpleUpload(auth, config);
            //here we use putPolicy as token
            
            var putExtra = new PutExtra();
            putExtra.MimeType = entryInfo.content_type;
            HttpResult sr = su.UploadData(data, uploadToken, entryInfo.objectKey, putExtra);

            if ((int)HttpStatusCode.NotAcceptable == sr.Code)
            {
                // Debug.Log("WangsuUtils: File Already Exist. Do not need to upload.");
                ParametersUpload.alreadyUploadSize4Current = ParametersUpload.alreadyUploadSize4Current + entryInfo.content_size;
                return;
            }

            if ((int)HttpStatusCode.OK != sr.Code)
            {
                throw new Exception(String.Format("Upload Simple Error {0}", sr.Code));
            }
            
            ParametersUpload.alreadyUploadSize4Current = ParametersUpload.alreadyUploadSize4Current + entryInfo.content_size;
        }

        public static void uploadMulti(EntryInfo entryInfo)
        {
            if (entryInfo.content_size < MaxSingleFile)
            {
                uploadSingle(entryInfo);
                return;
            }
                 
            Mac mac = new Mac("", "");
            Auth auth = new Auth(mac);
            Config config = new Config("unitycdn.up27.v1.wcsapi.com", "unitycdn.mgr27.v1.wcsapi.com", true);

            string uploadToken = getUploadToken(entryInfo);

            byte[] data;
            long size = 0;

            using (FileStream fs = new FileStream(entryInfo.full_path, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    size = fs.Length;
                    data = new byte[size];
                    fs.Read(data, 0, (int)size);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            long blockSize = 4 * 1024 * 1024;
            int firstChunkSize = 1024;

            if (firstChunkSize > size)
            {
                firstChunkSize = (int)size;
            }

            if (blockSize > size)
            {
                blockSize = (int)size;
            }

            SliceUpload su = new SliceUpload(auth, config);
            HttpResult result = su.MakeBlock(blockSize, 0, data, 0, firstChunkSize, uploadToken, entryInfo.objectKey);

            if ((int)HttpStatusCode.NotAcceptable == result.Code)
            {
                // Debug.Log("File Already Exist. Do not need to upload.");
                return;
            }

            ParametersUpload.alreadyUploadSize4Current = ParametersUpload.alreadyUploadSize4Current + firstChunkSize;

            if ((int)HttpStatusCode.OK != result.Code)
            {
                throw new Exception("Make block error");
            }

            WangsuResponse wr = JsonUtility.FromJson<WangsuResponse>(result.Text);

            long blockCount = (size + blockSize - 1) / blockSize;
            string[] contexts = new string[blockCount];
            contexts[0] = wr.ctx;

            // 上传第 1 个 block 剩下的数据
            if (firstChunkSize < blockSize)
            {
                result = su.Bput(contexts[0], firstChunkSize, data, firstChunkSize, (int)(blockSize - firstChunkSize), uploadToken, entryInfo.objectKey);

                if ((int)HttpStatusCode.OK != result.Code)
                {
                    throw new Exception("Bput error");
                }

                ParametersUpload.alreadyUploadSize4Current = ParametersUpload.alreadyUploadSize4Current + blockSize - firstChunkSize;

                wr = JsonUtility.FromJson<WangsuResponse>(result.Text);
                contexts[0] = wr.ctx;
            }

            // 上传后续 block，每次都是一整块上传
            for (int blockIndex = 1; blockIndex < blockCount; ++blockIndex)
            {
                long leftSize = size - blockSize * blockIndex;
                int chunkSize = (int)(leftSize > blockSize ? blockSize : leftSize);
                result = su.MakeBlock(chunkSize, blockIndex, data, (int)(blockSize * blockIndex), chunkSize, uploadToken, entryInfo.objectKey);
                if ((int)HttpStatusCode.OK == result.Code)
                {
                    wr = JsonUtility.FromJson<WangsuResponse>(result.Text);
                    contexts[blockIndex] = wr.ctx;
                }
                else
                {
                    throw new Exception("Makeblock error");
                    // return;
                }
                ParametersUpload.alreadyUploadSize4Current = ParametersUpload.alreadyUploadSize4Current + chunkSize;
            }

            // 合成文件，注意与前面打印的 ETag 对比
            // Debug.Log(entryInfo.objectKey);
            var putExtra = new PutExtra();
            putExtra.MimeType = entryInfo.content_type;
            result = su.MakeFile(size, entryInfo.objectKey, contexts, uploadToken, putExtra);
            if ((int)HttpStatusCode.OK != result.Code)
            {
                throw new Exception(String.Format("MakeFile error - {0}", result.Code));
            }
        }

        private static string getUploadToken(EntryInfo entryInfo)
        {
            try
            {
                string url = Parameters.apiHost + "api/v1/buckets/" + pb.selectedBucketUuid + "/uploadtoken/" + entryInfo.content_hash + "/";
                string respJson = HttpUtil.getHttpResponse(url, "GET");
                UploadInfo uploadInfo = JsonUtility.FromJson<UploadInfo>(respJson);
                return uploadInfo.uploadToken;
            }
            catch (Exception e)
            {
                Debug.Log("Get upload token failed.");
                throw e;
            }
        }
    }
}
