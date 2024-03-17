using System.Collections.Generic;
using LitJson;
using UnityEngine;
using WGame.Utils;

namespace WGame.Ability
{
    public sealed class AbilityData : IData
    {
        [EditorData("ID", EditorDataType.String)]
        public string ID { get; set; }
        [EditorData("名称", EditorDataType.String)]
        public string Name { get; set; }
        [EditorData("总时间(ms)", EditorDataType.Int)]
        public int TotalTime { get; set; }
        public string DebugName => Name;

        [field: SerializeReference]
        public List<DataEvent> EventList { get; set; } = new();

        public void Deserialize(JsonData jd)
        {
            ID = JsonHelper.ReadString(jd["ID"]);
            Name = JsonHelper.ReadString(jd["Name"]);
            TotalTime = JsonHelper.ReadInt(jd["TotalTime"]);
            
            DeserializeEvent(jd["Events"], ae => { EventList.Add(ae); });
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            writer.WriteObjectStart();
            
            JsonHelper.WriteProperty(ref writer, "ID", ID);
            JsonHelper.WriteProperty(ref writer, "Name", Name);
            JsonHelper.WriteProperty(ref writer, "TotalTime", TotalTime);
            
            SerializeEvent(ref writer);
            
            writer.WriteObjectEnd();
            return writer;
        }

        public void ForceSort()
        {
            EventList.Sort((l, r) =>
            {
                return l.TrackIndex < r.TrackIndex ? -1 : (l.TrackIndex == r.TrackIndex ? 0 : 1);
            });
        }

        private void SerializeEvent(ref JsonWriter writer)
        {
            EventList.Sort((l, r) => l.TriggerTime < r.TriggerTime ? -1 : (l.TriggerTime == r.TriggerTime ? 0 : 1));
            
            writer.WritePropertyName("Events");
            writer.WriteArrayStart();
            using (List<DataEvent>.Enumerator itr = EventList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    writer = itr.Current.Serialize(writer);
                }
            }
            writer.WriteArrayEnd();
        }
        
        private void DeserializeEvent(JsonData jd, System.Action<DataEvent> callback)
        {
            for (int i=0; i<jd.Count; ++i)
            {
                var ae = new DataEvent();
                ae.Deserialize(jd[i]);
                callback?.Invoke(ae);
            }
        }
    }
}