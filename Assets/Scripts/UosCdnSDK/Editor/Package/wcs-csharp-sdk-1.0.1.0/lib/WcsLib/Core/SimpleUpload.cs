﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wangsu.WcsLib.HTTP;
using Wangsu.WcsLib.Utility;

// ChangeLog:
// UMU @ 2017/12/19, Fix bug on MAC, AppendLine -> Append + "\r\n"
namespace Wangsu.WcsLib.Core
{
    /// <summary>
    /// 普通上传
    /// https://wcs.chinanetcenter.com/document/API/FileUpload/Upload
    /// 若文件大小超过500M，必须使用分片上传。
    /// </summary>
    public class SimpleUpload
    {
        public SimpleUpload(Auth auth, Config config)
        {
            this.auth = auth;
            this.config = config;
            this.httpManager = new HttpManager();
        }

        public SimpleUpload(Mac mac, Config config) : this(new Auth(mac), config)
        {
        }

        /// <summary>
        /// 上传数据
        /// putPolicy 中的 JSON 字符串是严格模式，不允许最后一个元素后面有逗号
        /// https://stackoverflow.com/questions/201782/can-you-use-a-trailing-comma-in-a-json-object
        /// 请确保 JSON 字符串的正确性和紧凑性，最好用 JSON 库生成，而不要自己用字符串拼接。
        /// </summary>
        /// <param name="data">待上传的数据</param>
        /// <param name="putPolicy">上传策略数据，JSON 字符串</param>
        /// <param name="key">可选，要保存的key</param>
        /// <param name="extra">可选，上传可选设置</param>
        /// <returns>上传数据后的返回结果</returns>
        public HttpResult UploadData(byte[] data, string putPolicy, string key = null, PutExtra putExtra = null)
        {
#if DEBUG
            if (null == putPolicy)
            {
                throw new ArgumentNullException("putPolicy");
            }
#endif
            if (putExtra == null)
            {
                putExtra = new PutExtra();
            }

            string filename = key;
            if (string.IsNullOrEmpty(key))
            {
                filename = "uploading.tmp";
            }
            
            string boundary = HttpManager.CreateFormDataBoundary();
            StringBuilder bodyBuilder = new StringBuilder();

            // write token
            bodyBuilder.Append("--" + boundary + "\r\n"
                + "Content-Disposition: form-data; name=\"token\"\r\n\r\n"
                + putPolicy + "\r\n");
            
            // write extra params
            if (null != putExtra.Params && putExtra.Params.Count > 0)
            {
                foreach (var p in putExtra.Params)
                {
                    if (p.Key.StartsWith("x:"))
                    {
                        bodyBuilder.Append("--" + boundary + "\r\n"
                            + "Content-Disposition: form-data; name=\"" + p.Key + "\"\r\n\r\n"
                            + p.Value + "\r\n");
                    }
                }
            }

            // write key
            if (null != key)
            {
                bodyBuilder.Append("--" + boundary + "\r\n"
                    + "Content-Disposition: form-data; name=\"key\"\r\n\r\n"
                    + key + "\r\n");
            }

            // write mime type
            if (!string.IsNullOrEmpty(putExtra.MimeType))
            {
                bodyBuilder.Append("--" + boundary + "\r\n"
                    + "Content-Disposition: form-data; name=\"mimeType\"\r\n\r\n"
                    + putExtra.MimeType + "\r\n");
            }

            // write deadline
            if (-1 != putExtra.Deadline)
            {
                bodyBuilder.Append("--" + boundary + "\r\n"
                    + "Content-Disposition: form-data; name=\"deadline\"\r\n\r\n"
                    + putExtra.Deadline.ToString() + "\r\n");
            }

            // write filename
            bodyBuilder.Append("--" + boundary + "\r\n"
                + "Content-Disposition: form-data; name=\"file\"; filename=\""
                + filename + "\"\r\nContent-Type: application/octet-stream\r\n\r\n");

            // write file data
            StringBuilder bodyEnd = new StringBuilder();
            bodyEnd.Append("\r\n--" + boundary + "--\r\n");

            byte[] partHead = Encoding.UTF8.GetBytes(bodyBuilder.ToString());
            byte[] partTail = Encoding.UTF8.GetBytes(bodyEnd.ToString());

            // 允许空内容
            int dataLength = 0;
            if (null != data)
            {
                dataLength = data.Length;
            }

            byte[] body = new byte[partHead.Length + dataLength + partTail.Length];
            //Array.Copy(partHead, 0, body, 0, partHead.Length);
            // Buffer.BlockCopy 比 Array.Copy 简单，所以更快。
            Buffer.BlockCopy(partHead, 0, body, 0, partHead.Length);
            if (null == data)
            {
                Buffer.BlockCopy(partTail, 0, body, partHead.Length, partTail.Length);
            }
            else
            {
                Buffer.BlockCopy(data, 0, body, partHead.Length, data.Length);
                Buffer.BlockCopy(partTail, 0, body, partHead.Length + data.Length, partTail.Length);
            }
            
            string url = config.GetUploadUrlPrefix() + "/file/upload";
            HttpResult result = httpManager.PostMultipart(url, body, boundary);

            return result;
        }

        /// <summary>
        /// 上传数据流
        /// </summary>
        /// <param name="stream">(确定长度的)数据流</param>
        /// <param name="putPolicy">上传策略数据，JSON 字符串</param>
        /// <param name="key">可选，要保存的key</param>
        /// <param name="extra">上传可选设置</param>
        /// <returns>上传数据流后的返回结果</returns>
        public HttpResult UploadStream(Stream stream, string putPolicy, string key = null, PutExtra putExtra = null)
        {
#if DEBUG
            if (null == putPolicy)
            {
                throw new ArgumentNullException("putPolicy");
            }
#endif
            int bufferSize = 4 * 1024 * 1024;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            using (MemoryStream dataMS = new MemoryStream())
            {
                while ((bytesRead = stream.Read(buffer, 0, bufferSize)) != 0)
                {
                    dataMS.Write(buffer, 0, bytesRead);
                }
                return UploadData(dataMS.ToArray(), putPolicy, key, putExtra);
            }
        }

        /// <summary>
        /// 上传本地文件，可能抛异常
        /// </summary>
        /// <param name="localFilename">本地文件名</param>
        /// <param name="putPolicy">上传策略数据，JSON 字符串</param>
        /// <param name="key">可选，要保存的key</param>
        /// <param name="extra">上传可选设置</param>
        /// <returns>上传数据流后的返回结果</returns>
        public HttpResult UploadFile(string localFilename, string putPolicy, string key = null, PutExtra putExtra = null)
        {
#if DEBUG
            if (null == putPolicy)
            {
                throw new ArgumentNullException("putPolicy");
            }
#endif
            FileStream fs = new FileStream(localFilename, FileMode.Open);
            return this.UploadStream(fs, putPolicy, key, putExtra);
        }

        private Auth auth;
        private Config config;
        private HttpManager httpManager;
    }
}
