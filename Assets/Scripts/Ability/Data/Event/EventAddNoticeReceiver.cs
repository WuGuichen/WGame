namespace WGame.Ability
{
    using LitJson;
    using Utils;
    
    public sealed class EventAddNoticeReceiver : IData, IEventData
    {
        public string DebugName => "Notice";

        [EditorData("接受器类型", EditorDataType.TypeID, 9)]
        public int ReceiverID { get; set; }

        [EditorData("接受器次数", EditorDataType.Int)]
        public int Times { get; set; } = 1;

        public EventDataType EventType => EventDataType.AddMessageReceiver;
        public void Enter(EventOwner owner)
        {
            owner.AddNoticeReceiver(ReceiverID, Times);
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
            
        }

        public void Exit(EventOwner owner, bool isBreak)
        {
            owner.RemoveNoticeReceiver(ReceiverID);
        }

        public void Deserialize(JsonData jd)
        {
            ReceiverID = JsonHelper.ReadInt(jd["Type"]);
            Times = JsonHelper.ReadInt(jd["Times"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "Type", ReceiverID);
            JsonHelper.WriteProperty(ref writer, "Times", Times);

            return writer;
        }

        public IEventData Clone()
        {
            var evt = new EventAddNoticeReceiver
            {
                ReceiverID = this.ReceiverID,
                Times = this.Times
            };

            return evt;
        }
    }
}