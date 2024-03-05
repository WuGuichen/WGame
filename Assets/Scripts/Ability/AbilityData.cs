using System.Collections.Generic;
using LitJson;
using UnityEngine;
using WGame.Utils;

namespace WGame.Ability
{
    public sealed class AbilityData : IData
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int TotalTime { get; set; }
        public string DebugName => Name;

        [field: SerializeReference]
        public List<DataEvent> EventList { get; set; } = new();

        public void Deserialize(JsonData jsonData)
        {
            ID = JsonHelper.ReadString(jsonData["ID"]);
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

        public void ForceSort()
        {
        }
    }
}