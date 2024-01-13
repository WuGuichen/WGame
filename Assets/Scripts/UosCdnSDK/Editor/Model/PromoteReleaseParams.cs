using System;

namespace UosCdn
{
    [Serializable]
    public class PromoteReleaseParams
    {
        public string from_release;
        public string to_bucket;
        public string notes;

        public PromoteReleaseParams(string from_release, string to_bucket, string notes)
        {
            this.from_release = from_release;
            this.to_bucket = to_bucket;
            this.notes = notes;
        }
    }
}