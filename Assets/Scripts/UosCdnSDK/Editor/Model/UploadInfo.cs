using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UosCdn
{
    [Serializable]
    public class UploadInfo
    {
        public string uploadToken;
        public string uploadUrl;
    }
}