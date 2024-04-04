using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class EventSetOwnerAttribute : IEventData, IData
    {
        public string DebugName => GetType().ToString();
        
        [EditorData("属性名", EditorDataType.AttributeTypeID)]
        public int AttrID { get; set; }

        [EditorData("值", EditorDataType.Int)] public int Value { get; set; } = 0;
        
        public void Deserialize(JsonData jd)
        {
            AttrID = JsonHelper.ReadInt(jd["Attr"]);
            Value = JsonHelper.ReadInt(jd["Val"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "Attr", AttrID);
            JsonHelper.WriteProperty(ref writer, "Val", Value);
            return writer;
        }

        public EventDataType EventType => EventDataType.SetOwnerAttr;
        public void Enter(EventOwner owner)
        {
            owner.SetAttribute(AttrID, Value);
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
        }

        public void Exit(EventOwner owner, bool isBreak)
        {
        }

        public IEventData Clone()
        {
            return new EventSetOwnerAttribute()
            {
                AttrID = AttrID,
                Value = Value,
            };
        }
    }
}
