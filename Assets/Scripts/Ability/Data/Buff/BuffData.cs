using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class BuffData : IData
    {
        [EditorData("ID", EditorDataType.BuffDataTypeID)]
        public int ID { get; set; }
        [EditorData("名称", EditorDataType.String)]
        public string Name { get; set; }
        [EditorData("Buff目标", EditorDataType.Enum)]
        public BuffTargetType Target { get; set; } = BuffTargetType.None;

        [EditorData("堆叠层数", EditorDataType.Int)]
        public int AddNum { get; set; } = 1;
        [EditorData("持续时间", EditorDataType.Int)]
        public int Duration { get; set; } = -1;
        
        public string DebugName => Name;
        public virtual void Deserialize(JsonData jd)
        {
            ID = JsonHelper.ReadInt(jd["ID"]);
            Name = JsonHelper.ReadString(jd["Name"]);
            Target = JsonHelper.ReadEnum<BuffTargetType>(jd["Target"]);
            AddNum = JsonHelper.ReadInt(jd["AddNum"]);
            Duration = JsonHelper.ReadInt(jd["Duration"]);
        }

        public virtual JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "ID", ID);
            JsonHelper.WriteProperty(ref writer, "Name", Name);
            JsonHelper.WriteProperty(ref writer, "Target", Target.ToString());
            JsonHelper.WriteProperty(ref writer, "AddNum", AddNum);
            JsonHelper.WriteProperty(ref writer, "Duration", Duration);
            return writer;
        }
    }
}