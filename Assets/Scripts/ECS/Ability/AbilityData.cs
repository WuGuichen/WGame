using LitJson;
using WGame.EditorUtils;

namespace WGame.Ability
{
    public sealed class AbilityData : IData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int TotalTime { get; set; }
        public string DebugName => Name;
        
        public void Deserialize(JsonData jsonData)
        {
            ID = JsonHelper.ReadInt(jsonData["ID"]);
            Name = JsonHelper.ReadString(jsonData["Name"]);
            TotalTime = JsonHelper.ReadInt(jsonData["TotalTime"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            writer.WriteObjectStart();
            
            JsonHelper.WriteProperty(ref writer, "ID", ID);
            JsonHelper.WriteProperty(ref writer, "Name", Name);
            JsonHelper.WriteProperty(ref writer, "TotalTime", TotalTime);
            
            writer.WriteObjectEnd();
            return writer;
        }
    }
}