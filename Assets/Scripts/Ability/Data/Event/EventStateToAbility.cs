using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class EventStateToAbility : IData, IEventData
    {
        public string DebugName { get; }
        [EditorData("状态", EditorDataType.TypeID, 2)]
        public int WaitState { get; set; }
        [EditorData("值", EditorDataType.Bool)]
        public bool CheckType { get; set; }
        [EditorData("切换能力", EditorDataType.TypeID, 6)]
        public int AbilityType { get; set; }
        
        public void Deserialize(JsonData jd)
        {
            var data = jd["Data"];
            WaitState = JsonHelper.ReadInt(data[0]);
            CheckType = JsonHelper.ReadBool(data[1]);
            AbilityType = JsonHelper.ReadInt(data[2]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            writer.WritePropertyName("Data");
            writer.WriteArrayStart();
            writer.Write(WaitState);
            writer.Write(CheckType);
            writer.Write(AbilityType);
            writer.WriteArrayEnd();
            return writer;
        }

        public EventDataType EventType => EventDataType.TriggerStateToAbility;
        public void Enter(EventOwner owner)
        {
            
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
            if (owner.CheckState(WaitState) == CheckType)
            {
                owner.SetMotionAbility(AbilityType, true);
            }
        }

        public void Exit(EventOwner owner, bool isBreak)
        {
            
        }

        public IEventData Clone()
        {
            return new EventStateToAbility() { CheckType = CheckType, WaitState = WaitState, AbilityType = AbilityType };
        }
    }
}
