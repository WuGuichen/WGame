using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class EventInputTriggerToAbility : IData, IEventData
    {
        [EditorData("输入类型", EditorDataType.MaskTypeID, 3)]
        public int InputType { get; set; }
        [EditorData("输入值", EditorDataType.Bool)]
        public bool InputValue { get; set; }
        
        [EditorData("切换到Ability", EditorDataType.TypeID, 6)]
        public int AbilityType { get; set; }

        public string DebugName { get; }
        public void Deserialize(JsonData jd)
        {
            var data = jd["Data"];
            InputType = JsonHelper.ReadInt(data[0]);
            InputValue = JsonHelper.ReadBool(data[1]);
            AbilityType = JsonHelper.ReadInt(data[2]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            writer.WritePropertyName("Data");
            writer.WriteArrayStart();
            writer.Write(InputType);
            writer.Write(InputValue);
            writer.Write(AbilityType);
            writer.WriteArrayEnd();
            return writer;
        }

        public EventDataType EventType => EventDataType.TriggerInputToAbility;
        public void Enter(EventOwner owner)
        {
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
            if (InputValue == owner.CheckInput(InputType))
            {
                owner.SetAbility(AbilityType, true);
            }
        }

        public void Exit(EventOwner owner, bool isBreak)
        {
        }

        public IEventData Clone()
        {
            return new EventInputTriggerToAbility()
            {
                InputType = InputType,
                InputValue = InputValue,
                AbilityType = AbilityType
            };
        }
    }
}