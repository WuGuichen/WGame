using System;
using System.Collections.Generic;
using COSXML.Model.Tag;
using UnityEngine;

namespace UosCdn
{
    public class ContentController
    {
        public static Dictionary<string, string> contentTypeMapping = new Dictionary<string, string>
        {
            {".hash", "application/octet-stream"},
            {".json", "text/plain; charset=utf-8"},
            {".bundle", "application/octet-stream"}
        };
        
        public static void UploadContent(EntryInfo entryInfo)
        {
            resetCurrentUploadParams(entryInfo);
            if (entryInfo.content_size < UploadConfig.divisionForUpload)
            {
                uploadSingle(entryInfo);
            }
            else
            {
                uploadMulti(entryInfo);
            }
        }

        private static void uploadSingle(EntryInfo entryInfo)
        {
            // Debug.Log("Upload Single");
            switch (Parameters.backend)
            {
                case "cos":
                    CosUtils.uploadSingle(entryInfo.objectKey, entryInfo.full_path);
                    break;
                case "BlueCloud":
                    WangsuUtils.uploadMulti(entryInfo);
                    break;
                default:
                    WangsuUtils.uploadMulti(entryInfo);
                    break;
            }
        }

        private static void uploadMulti(EntryInfo entryInfo)
        {
            // Debug.Log("Upload Multi");
            switch (Parameters.backend)
            {
                case "cos":
                    CosUtils.uploadMulti(entryInfo);
                    break;
                case "BlueCloud":
                    WangsuUtils.uploadMulti(entryInfo);
                    break;
                default:
                    WangsuUtils.uploadMulti(entryInfo);
                    break;
            }
        }

        private static void resetCurrentUploadParams(EntryInfo entryInfo)
        {
            ParametersUpload.totalUploadSize4Current = entryInfo.content_size;
            ParametersUpload.alreadyUploadSize4Current = 0;
            ParametersUpload.alreadyUploadPartsSize4Current = 0;
        }
    }
}
