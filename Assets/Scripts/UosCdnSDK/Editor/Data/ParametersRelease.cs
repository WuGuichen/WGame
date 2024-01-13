using System;
using System.Collections.Generic;
using UnityEngine;

namespace UosCdn
{
    [Serializable]
    public class ParametersRelease
    {
        private static ParametersRelease _Singleton = null;

        public bool showReleaseArea = true;
        public string showReleaseAreaText = "";

        public string selectedReleaseName = "";
        public string selectedReleaseId = "";

        public Release[] releaseList = new Release[0];
        public String[] releaseNameList = new String[0];

        public int currentReleasePage = 0;
        public int selectedReleaseIndex = 0;

        public int totalReleaseCounts = 0;
        public int totalReleasePages = 1;

        public bool releasePreviousButton = false;
        public bool releaseNextButton = false;

        public static ParametersRelease GetParametersRelease()
        {
            if (_Singleton == null)
            {
                _Singleton = new ParametersRelease();
            }
            return _Singleton;
        }
    }
}
