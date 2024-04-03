using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class EventSetMoveParam : IData, IEventData
    {
        public string DebugName { get; }
        [EditorData("类型", EditorDataType.TypeID, 4)]
        public int ParamType { get; set; }
        [EditorData("百分比值", EditorDataType.Int)]
        public int ParamValue { get; set; }
        
        public void Deserialize(JsonData jd)
        {
            var data = jd["Data"];
            ParamType = JsonHelper.ReadInt(data[0]);
            ParamValue = JsonHelper.ReadInt(data[1]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            writer.WritePropertyName("Data");
            writer.WriteArrayStart();
            writer.Write(ParamType);
            writer.Write(ParamValue);
            writer.WriteArrayEnd();
            return writer;
        }

        public EventDataType EventType => EventDataType.SetMoveParam;
        public void Enter(EventOwner owner)
        {
            switch (ParamType)
            {
                case MoveParamType.MoveSpeed:
                    owner.SetMoveSpeed(ParamValue);
                    break;
                case MoveParamType.RootMotion:
                    owner.SetRootMotion(ParamValue);
                    break;
                case MoveParamType.RotateSpeed:
                    owner.SetRotateSpeed(ParamValue);
                    break;
            }
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
        }


        public void Exit(EventOwner owner, bool isBreak)
        {
            
        }

        public IEventData Clone()
        {
            return new EventSetMoveParam() { ParamValue = ParamValue, ParamType = ParamType };
        }
    }
}