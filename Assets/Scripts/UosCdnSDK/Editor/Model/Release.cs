using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UosCdn
{
    [Serializable]
    public class Release
    {
        public string releaseid;
        public int releasenum;
        public string created_by;
        public string entries_link;
        public string content_hash;
        public string content_size;

        public override string ToString()
        {
            return string.Format("{0}", releaseid);
        }

        public string ToMessage()
        {
            return string.Format("ID:\t{0}\nRelease Number:\t{1}\nCreated By:\t{2}\nEntries Link:\t{3}\nContent Hash:\t{4}\nContent Type:\t{5}\n", releaseid, releasenum, created_by, entries_link, content_hash, content_size);
        }
    }
}