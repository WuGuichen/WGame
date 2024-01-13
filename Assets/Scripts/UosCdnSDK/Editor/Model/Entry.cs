using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UosCdn
{
    [Serializable]
    public class Entry
    {
        public string content_hash;
        public string entryid;
        public string path;
        public string content_type;
        public long content_size;
        public string last_modified;

        public override string ToString()
        {
            return string.Format("{0}[{1}]", entryid, path);
        }

        public string ToMessage()
        {
            return string.Format("ID:\t{0}\nPath:\t{1}\nContent Hash:\t{2}\nContent Type:\t{3}\nContent Size:\t{4}MB\nLast Modified Time:\t{5}\n", entryid, path, content_hash, content_type, content_size/1048576.0, last_modified);
        }
    }
}
