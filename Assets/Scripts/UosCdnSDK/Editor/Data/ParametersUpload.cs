using System;
using System.Collections.Generic;
using UnityEngine;

namespace UosCdn
{
    [Serializable]
    public class ParametersUpload
    {
        public static UploadPartsStatusList unfinishedUploadList = new UploadPartsStatusList();
        public static Dictionary<string, int> unfinishedIndexObjetkeyMapping = new Dictionary<string, int>();

        public static bool syncFinished = true;
        public static long totalUploadSize = 0;
        public static long alreadyUploadSize = 0;
        public static int totalUploadFiles = 0;
        public static int alreadyUploadFiles = 0;

        public static long totalUploadSize4Current = 0;
        public static long alreadyUploadSize4Current = 0;
        public static long alreadyUploadPartsSize4Current = 0;

        public static int createdFiles = 0;
        public static int updatedFiles = 0;
        public static int failedFiles = 0;
    }
}