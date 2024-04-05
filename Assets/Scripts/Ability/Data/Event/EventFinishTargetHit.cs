using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class EventFinishTargetHit : IEventData, IData
    {
        public EventDataType EventType => EventDataType.FinishTargetHit;
        
        [EditorData("伤害率", EditorDataType.Int)]
        public int DmgRate { get; set; }
        
        public void Enter(EventOwner owner)
        {
            owner.ApplyHitToFinishAtkTarget(DmgRate);
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
        }

        public void Exit(EventOwner owner, bool isBreak)
        {
        }

        public IEventData Clone()
        {
            return new EventFinishTargetHit() { DmgRate = DmgRate };
        }

        public string DebugName { get; }
        public void Deserialize(JsonData jd)
        {
            DmgRate = JsonHelper.ReadInt(jd["Rate"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "Rate", DmgRate);
            return writer;
        }
    }
}