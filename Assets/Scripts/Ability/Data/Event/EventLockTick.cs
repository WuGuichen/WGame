using LitJson;

namespace WGame.Ability
{
    public class EventLockTick : IData, IEventData
    {
        public string DebugName { get; }
        public void Deserialize(JsonData jd)
        {
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            return writer;
        }

        public EventDataType EventType { get; }
        public void Enter(EventOwner owner)
        {
            owner.IsLockTick = true;
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
            
        }

        public void Exit(EventOwner owner, bool isBreak)
        {
            owner.IsLockTick = false;
        }

        public IEventData Clone()
        {
            return new EventLockTick();
        }
    }
}