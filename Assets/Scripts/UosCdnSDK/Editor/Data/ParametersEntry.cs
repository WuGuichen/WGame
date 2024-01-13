using System;
using System.Collections.Generic;
using UnityEngine;

namespace UosCdn
{
    [Serializable]
    public class ParametersEntry
    {
        private static ParametersEntry _Singleton = null;

        public bool showEntryArea = true;
        public string showEntryAreaText = "";

        public string syncPath = "";

        public int selectedEntryIndex = 0;
        public string selectedEntryUuid = "";
        public string selectedEntryName = "";

        public Entry[] entryList = new Entry[0];
        public String[] entryNameList = new String[0];

        public int currentEntryPage = 0;
        public int totalEntryCounts = 0;
        public int totalEntryPages = 1;

        public bool entryPreviousButton = false;
        public bool entryNextButton = false;

        public static ParametersEntry GetParametersEntry()
        {
            if (_Singleton == null)
            {
                _Singleton = new ParametersEntry();
            }
            return _Singleton;
        }

        public void refreshSyncPath()
        {
            return;
        }
    }


}