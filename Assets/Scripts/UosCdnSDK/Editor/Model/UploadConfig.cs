using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UosCdn
{
    [Serializable]
    public class UploadConfig
    {
        // public const long divisionForUpload = 5242880; // 5M;
        public const long divisionForUpload = 1048576; // 1M;
        public const long sliceSizeForUpload = 1048576; // 5M
    }

    [Serializable]
    public class PartUploadList
    {
        public Dictionary<string, UploadParts> uploadPartsList;
    }

    [Serializable]
    public class UploadParts
    {

        public string path;

        public string fullPath;

        public string contentHash;

        public string objectKey;

        public string uploadId;

        public long totalParts;

        public long currentPart;

        public List<PartStructure> parts;

        public UploadParts()
        {
        }

        public UploadParts(EntryInfo entryInfo, List<PartStructure> parts)
        {
            this.path = entryInfo.path;
            this.fullPath = entryInfo.full_path;
            this.contentHash = entryInfo.content_hash;
            this.objectKey = entryInfo.objectKey;
            this.parts = parts;
            this.totalParts = parts.Count;
            this.currentPart = 1;
        }

        public UploadParts(UploadPartsStatus ups)
        {
        }
    }

    [Serializable]
    public class PartStructure
    {
        public int partId;

        public bool hasUploaded;

        public long partStart;

        public long partEnd;

        public long partLength;

        public string eTag;

    }

    [Serializable]
    public class UploadPartsStatusList
    {
        public List<UploadPartsStatus> unfinishedList;

        public UploadPartsStatusList()
        {
            unfinishedList = new List<UploadPartsStatus>();
        }

        public override string ToString()
        {
            string res = "";
            foreach(UploadPartsStatus ups in unfinishedList)
            {
                res += ups.ToString();
            }
            return res;
        }
    }


    [Serializable]
    public class UploadPartsStatus
    {
        public string objectKey;

        public string uploadId;

        public override string ToString()
        {
            return string.Format("objectKey: {0}, uploadId: {1};", objectKey, uploadId);
        }
    }

    [Serializable]
    public class ProjectInfo
    {
        public string UnityProjectGuid;
        public string Provider;
        public bool Status;
    }
}