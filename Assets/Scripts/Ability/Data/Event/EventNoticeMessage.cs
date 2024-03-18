namespace WGame.Ability
{
    using LitJson;
    using Utils;
    
    public sealed class EventNoticeMessage : IData, IEventData
    {
        public string DebugName => "Notice";

        private int _receiverID;
        [EditorData("触发时机", EditorDataType.NoticeReceiver, 50)]
        public int ReceiverID { get => _receiverID;
            set
            {
                if (_receiverID != value)
                {
                    _receiverID = value;
                    // if(_receiverID == )
                }
            } }
        
        [EditorData("触发消息", EditorDataType.Object)]
        public INoticeMessage Message { get; set; }
        
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