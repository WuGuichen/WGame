namespace WGame.Trigger
{
    [System.Serializable]
    public struct WtEventType
    {
        public int mainType;
        public int subType;
        public int subTypeParam;
        public int eventType;
        public int eventParam;

        public WtEventType(int mainType, int subType, int subTypeParam, int eventType, int eventParam)
        {
            this.mainType = mainType;
            this.subType = subType;
            this.subTypeParam = subTypeParam;
            this.eventType = eventType;
            this.eventParam = eventParam;
        }

        public bool IsContain(WtEventType data)
        {
            return (data.mainType == this.mainType
                    && (data.subType & this.subType) != 0
                    && (data.subTypeParam == this.subTypeParam)
                    && (data.eventType & data.eventType) != 0
                    && (data.eventParam == this.eventParam));
        }

        public override string ToString()
        {
            return string.Format("mainType:{0}, subType:{1}, subParam:{2}, eventType:{3}, eventParam:{4}",
                mainType, subType, subTypeParam, eventType, eventParam);
        }
    }
}
