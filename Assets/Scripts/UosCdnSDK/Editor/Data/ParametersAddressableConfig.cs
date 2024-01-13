using System;
using System.Collections.Generic;
using UnityEngine;

namespace UosCdn
{
    [Serializable]
    public class ParametersAddressableConfig
    {
        private static ParametersAddressableConfig _Singleton = null;

        public bool showAddressableConfigArea = true;

        public GUIContent remoteLoadPathText = new GUIContent();

        public string remoteLoadUrl = "";

        public static ParametersAddressableConfig GetParametersAddressableConfig()
        {
            if (_Singleton == null)
            {
                _Singleton = new ParametersAddressableConfig();
            }
            return _Singleton;
        }
    }


}