using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public enum TriggerAddType
    {
        Override = 0,
        NotOverride,
        Add,
    }
    public class EventTriggerBase : IData, IEventData
    {
        public string DebugName { get; }
        [EditorData("触发类型", EditorDataType.TypeID, 5)]
        public int TriggerType { get; set; }
        [EditorData("添加类型", EditorDataType.Enum)]
        public TriggerAddType AddType { get; set; }
        [EditorData("触发次数", EditorDataType.Int)]
        public int TriggerTimes { get; set; }
        
        public void Deserialize(JsonData jd)
        {
            var cfg = jd["Trigger"]; 
            TriggerType = JsonHelper.ReadInt(cfg[0]);
            AddType = JsonHelper.ReadEnum<TriggerAddType>(cfg[1]);
            TriggerTimes = JsonHelper.ReadInt(cfg[2]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            writer.WritePropertyName("Trigger");
            writer.WriteArrayStart();
            writer.Write(TriggerType);
            writer.Write(AddType.ToString());
            writer.Write(TriggerTimes);
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
            throw new System.NotImplementedException();
        }
    }
}