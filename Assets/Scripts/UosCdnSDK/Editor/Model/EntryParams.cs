using System;

namespace UosCdn
{
    [Serializable]
    public class EntryParams
    {
        public string path;
        public string content_hash;
        public long content_size;
        public string content_type;
        public string content_link;

        public EntryParams(string path, string hash, long size, string contentType)
        {
            this.path = path;
            this.content_hash = hash;
            this.content_size = size;
            this.content_type = contentType;
            this.content_link = string.Format("cos:{0}/{1}", Parameters.uosAppId, hash);
        }

        public EntryParams(EntryInfo entryInfo)
        {
            this.path = entryInfo.path;
            this.content_hash = entryInfo.content_hash;
            this.content_size = entryInfo.content_size;
            this.content_type = entryInfo.content_type;
            this.content_link = string.Format("cos:{0}/{1}", Parameters.uosAppId, entryInfo.content_hash);
        }
    }
}