namespace WGame.Ability
{
    using LitJson;
    using Utils;
    
    public sealed class EventNoticeMessage : IData, IEventData
    {
        public string DebugName => "Notice";
        
        [EditorData("接收者", EditorDataType.NoticeReceiver, 50)]
        public int ReceiverID { get; set; }
        
        public EventDataType EventType => EventDataType.NoticeMessage;
        
        public void Deserialize(JsonData jd)
        {
            ReceiverID = JsonHelper.ReadInt(jd["ReceiverID"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "ReceiverID", ReceiverID);

            return writer;
        }

        public IEventData Clone()
        {
            var evt = new EventNoticeMessage();
            evt.ReceiverID = this.ReceiverID;

            return evt;
        }
    }
}