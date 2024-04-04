using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class EventFocusKeepDist : IData, IEventData
    {
        public string DebugName { get; }

        [EditorData("最近距离(cm)", EditorDataType.Int, 130f)]
        public int OffsetDist { get; set; }
        
        
        public void Deserialize(JsonData jd)
        {
            OffsetDist = JsonHelper.ReadInt(jd["Offset"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "Offset", OffsetDist);
            return writer;
        }

        public EventDataType EventType => EventDataType.FocusKeepDist;
        
        public void Enter(EventOwner owner)
        {
            
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
            if (owner.TryGetFocusDistance(out var dist))
            {
                if (dist * 100f < OffsetDist)
                {
                    owner.EnableStates(AStateType.KeepDistance);
                }
            }
        }

        public void Exit(EventOwner owner, bool isBreak)
        {
            
        }

        public IEventData Clone()
        {
            return new EventFocusKeepDist()
            {
                OffsetDist = OffsetDist
            };
        }
    }
}