using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UosCdn
{
    [Serializable]
    public class Bucket
    {
        public string description;
        public string id;
        public string name;
        public string projectguid;

        public string ToMessage()
        {
            return string.Format("ID:\t{0}\nName:\t{1}\nDescription:\t{2}\nProject Guid:\t{3}\n", id, name, description, projectguid);
        }
    }
}