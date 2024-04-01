using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class EventInterrupt : IData, IEventData
    {
        public string DebugName => "Break";

        [EditorData("类型", EditorDataType.MaskTypeID, 1)]
        public int BreakType { get; set; }
        
        public void Deserialize(JsonData jd)
        {
            BreakType = JsonHelper.ReadInt(jd["Type"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "Type", BreakType);
            return writer;
        }

        public EventDataType EventType => EventDataType.Interrupt;
        public void Enter(EventOwner owner)
        {
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
            owner.SetAbilityBreak(BreakType);
        }

        public void Exit(EventOwner owner, bool isBreak)
        {
        }

        public IEventData Clone()
        {
            return new EventInterrupt()
            {
                BreakType = BreakType,
            };
        }
    }
}