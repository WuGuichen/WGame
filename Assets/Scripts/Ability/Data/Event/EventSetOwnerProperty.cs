using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class EventSetOwnerProperty : IEventData, IData
    {
        public string DebugName { get; }
        
        [EditorData("参数名", EditorDataType.String)]
        public string ValName { get; set; }

        [EditorData("值", EditorDataType.Object)]
        public CustomParam Value { get; set; } = new();
        
        public void Deserialize(JsonData jd)
        {
            ValName = JsonHelper.ReadString("Name");
            Value.Deserialize(jd);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "Name", ValName);
            writer.WritePropertyName("Val");
            writer.WriteObjectStart();
            Value.Serialize(writer);
            writer.WriteObjectEnd();
            return writer;
        }

        public EventDataType EventType => EventDataType.SetOwnerProperty;
        public void Enter(EventOwner owner)
        {
            // owner.SetProperty()
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
            throw new System.NotImplementedException();
        }

        public void Exit(EventOwner owner, bool isBreak)
        {
            throw new System.NotImplementedException();
        }

        public IEventData Clone()
        {
            WLogger.Error("无法复制");
            return null;
        }
    }
}