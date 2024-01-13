using System;
using System.Collections.Generic;
using UnityEngine;

namespace UosCdn
{
    [Serializable]
    public class ParametersBucket
    {
        private static ParametersBucket _Singleton = null;

        public bool showBucketArea = true;
        public string showBucketAreaText = "";

        public string bucketName = "";

        public string selectedBucketUuid = "";
        public string selectedBucketName = "";

        public Bucket[] bucketList = new Bucket[0];
        public String[] bucketNameList = new String[0];

        public int currentBucketPage = 0;
        public int selectedBucketIndex = 0;

        public int totalBucketCounts = 0;
        public int totalBucketPages = 1;

        public bool bucketPreviousButton = false;
        public bool bucketNextButton = false;

        public static ParametersBucket GetParametersBucket()
        {
            if (_Singleton == null)
            {
                _Singleton = new ParametersBucket();
            }
            return _Singleton;
        }
    }


}