using WGame.Utils;

namespace WGame.Ability
{
    using LitJson;
    
    public sealed class EventDoAction : IData, IEventData
    {
        public string DebugName => "行为";
        
        [EditorData("行为类型", EditorDataType.ActionID)]
        public int ActionType { get; set; }

        [EditorData("行为参数", EditorDataType.Object)]
        public CustomParam ActionParam { get; set; } = new CustomParam();
        
        public void Deserialize(JsonData jd)
        {
            ActionType = JsonHelper.ReadInt(jd["Type"]);
            ActionParam.Deserialize(jd["Param"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "Type", ActionType.ToString());
            writer.WritePropertyName("Param");
            writer.WriteObjectStart();
            writer = ActionParam.Serialize(writer);
            writer.WriteObjectEnd();
            return writer;
        }

        public EventDataType EventType => EventDataType.DoAction;
        public void Enter(EventOwner owner)
        {
            switch (ActionType)
            {
                case WActionType.SetAnimGroup:
                    owner.SetAnimGroup(ActionParam.Value.AsInt());
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
            WLogger.Error("无法复制");
            return null;
        }
    }
}