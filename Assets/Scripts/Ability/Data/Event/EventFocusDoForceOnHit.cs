using LitJson;

namespace WGame.Ability
{
    public class EventFocusDoForceOnHit : IData, IEventData
    {
        public string DebugName { get; }
        [EditorData("作用方向", EditorDataType.TypeID, 10)]
        public int DirType { get; set; }
        
        [EditorData("冲量", EditorDataType.Int)]
        public int Impulse { get; set; }
        
        public void Deserialize(JsonData jd)
        {
            var data = jd["Data"];
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            writer.WritePropertyName("Data");
            writer.WriteArrayStart();
            writer.WriteArrayEnd();
            return writer;
        }

        public EventDataType EventType => EventDataType.FocusDoForce;
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
            return new EventFocusDoForceOnHit()
            {
            };
        }
    }
}