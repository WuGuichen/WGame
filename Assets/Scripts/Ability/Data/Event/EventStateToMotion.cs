using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class EventStateToMotion : IData, IEventData
    {
        public string DebugName { get; }
        [EditorData("状态", EditorDataType.TypeID, 2)]
        public int WaitState { get; set; }
        [EditorData("值", EditorDataType.Bool)]
        public bool CheckType { get; set; }
        [EditorData("切换状态", EditorDataType.TypeID, 1)]
        public int MotionType { get; set; }
        public void Deserialize(JsonData jd)
        {
            var data = jd["Data"];
            WaitState = JsonHelper.ReadInt(data[0]);
            CheckType = JsonHelper.ReadBool(data[1]);
            MotionType = JsonHelper.ReadInt(data[2]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            writer.WritePropertyName("Data");
            writer.WriteArrayStart();
            writer.Write(WaitState);
            writer.Write(CheckType);
            writer.Write(MotionType);
            writer.WriteArrayEnd();
            return writer;
        }

        public EventDataType EventType => EventDataType.TriggerStateToMotion;
        public void Enter(EventOwner owner)
        {
            
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
            if (owner.CheckState(WaitState) == CheckType)
            {
                owner.SetMotionAbility(MotionType, true);
            }
        }

        public void Exit(EventOwner owner, bool isBreak)
        {
            
        }

        public IEventData Clone()
        {
            return new EventStateToMotion() { CheckType = CheckType, WaitState = WaitState, MotionType = MotionType };
        }
    }
}