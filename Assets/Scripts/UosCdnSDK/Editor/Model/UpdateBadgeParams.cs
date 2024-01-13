using System;

namespace UosCdn
{
    [Serializable]
    public class UpdateBadgeParams
    {
        public string name;
        public string releaseid;

        public UpdateBadgeParams(string name, string releaseid)
        {
            this.name = name;
            this.releaseid = releaseid;
        }
    }
}