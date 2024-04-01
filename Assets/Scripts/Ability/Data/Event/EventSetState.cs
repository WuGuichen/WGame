using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class EventSetState : IData, IEventData
    {
        public string DebugName => "设置状态";
        [EditorData("状态类型", EditorDataType.MaskTypeID, 2)]
        public int StateMask { get; set; }
        
        public void Deserialize(JsonData jd)
        {
            StateMask = JsonHelper.ReadInt(jd["States"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "States", StateMask);
            return writer;
        }

        public EventDataType EventType => EventDataType.LockTick;
        public void Enter(EventOwner owner)
        {
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
            owner.EnableStates(StateMask);
        }

        public void Exit(EventOwner owner, bool isBreak)
        {
        }

        public IEventData Clone()
        {
            return new EventSetState(){StateMask = StateMask};
        }
    }
}