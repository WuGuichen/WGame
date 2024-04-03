using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class EventTriggerSetValue : IData, IEventData
    {
        public string DebugName { get; }
        [EditorData("触发事件", EditorDataType.MaskTypeID, 5)]
        public int TriggerType { get; set; }
        
        [EditorData("参数名", EditorDataType.String)]
        public string ValName { get; set; }

        [EditorData("值", EditorDataType.Object)]
        public CustomParam Value { get; set; } = new CustomParam();
        public void Deserialize(JsonData jd)
        {
            ValName = JsonHelper.ReadString("Name");
            Value.Deserialize(jd["Value"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "Name", ValName);
            writer.WritePropertyName("Value");
            writer.WriteObjectStart();
            writer = Value.Serialize(writer);
            writer.WriteObjectEnd();
            return writer;
        }

        public EventDataType EventType { get; }
        public void Enter(EventOwner owner)
        {
            
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
        }

        public void Exit(EventOwner owner, bool isBreak)
        {
        }

        public IEventData Clone()
        {
            return new EventTriggerSetValue(){};
        }
    }
}