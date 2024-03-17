using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class BuffData : IData
    {
        [EditorData("ID", EditorDataType.String)]
        public string ID { get; set; }
        [EditorData("名称", EditorDataType.String)]
        public string Name { get; set; }
        public BuffTargetType Target { get; set; } = BuffTargetType.None;
        public string DebugName => Name;
        public virtual void Deserialize(JsonData jd)
        {
            ID = JsonHelper.ReadString(jd["ID"]);
            Name = JsonHelper.ReadString(jd["Name"]);
            Target = JsonHelper.ReadEnum<BuffTargetType>(jd["Target"]);
        }

        public virtual JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "ID", ID);
            JsonHelper.WriteProperty(ref writer, "Name", Name);
            JsonHelper.WriteProperty(ref writer, "Target", Target.ToString());
            return writer;
        }
    }
}