namespace WGame.Ability
{
    using LitJson;
    
    public sealed class EventDoAction : IData, IEventData
    {
        public string DebugName => ActionName;
        
        [EditorData("行为名", EditorDataType.Action)]
        public string ActionName { get; set; }
        
        public void Deserialize(JsonData jd)
        {
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            return writer;
        }

        public EventDataType EventType => EventDataType.DoAction;

        public IEventData Clone()
        {
            var evt = new EventDoAction();

            return evt;
        }
    }
}