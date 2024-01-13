using System;
using System.Collections.Generic;
using UnityEngine;

namespace UosCdn
{
    [Serializable]
    public class ParametersBadge
    {
        private static ParametersBadge _Singleton = null;

        public bool showBadgeArea = true;
        public string showBadgeAreaText = "";

        public string badgeName = "";

        public string selectedBadgeName = "";
        public string selectedBadgeLinkedRelease = "";

        public Badge[] badgeList = new Badge[0];
        public String[] badgeNameList = new String[0];

        public int currentBadgePage = 0;
        public int selectedBadgeIndex = 0;

        public int totalBadgeCounts = 0;
        public int totalBadgePages = 1;

        public bool badgePreviousButton = false;
        public bool badgeNextButton = false;

        public static ParametersBadge GetParametersBadge()
        {
            if (_Singleton == null)
            {
                _Singleton = new ParametersBadge();
            }
            return _Singleton;
        }
    }


}