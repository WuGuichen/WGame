using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class CustomProperty : IData
    {
        public string DebugName { get; }
        [EditorData("", EditorDataType.String)]
        public string Name { get; set; }

        [EditorData("", EditorDataType.Object)]
        public CustomParam Value { get; set; } = new CustomParam();
        
        public void Deserialize(JsonData jd)
        {
            Name = JsonHelper.ReadString(jd["K"]);
            Value.Deserialize(jd["V"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            writer.WriteObjectStart();
            JsonHelper.WriteProperty(ref writer, "K", Name);
            writer.WritePropertyName("V");
            writer.WriteObjectStart();
            writer = Value.Serialize(writer);
            writer.WriteObjectEnd();
            writer.WriteObjectEnd();
            return writer;
        }
    }
}