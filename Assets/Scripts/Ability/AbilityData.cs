using System.Collections.Generic;
using LitJson;
using UnityEngine;
using WGame.Utils;

namespace WGame.Ability
{
    public sealed class AbilityData : IData
    {
        [EditorData("ID", EditorDataType.Lable)]
        public int ID { get; set; } = -1;
        [EditorData("名称", EditorDataType.String)]
        public string Name { get; set; }
        [EditorData("总时间(ms)", EditorDataType.Int)]
        public int TotalTime { get; set; }
        public string DebugName => Name;

        [field: System.NonSerialized] public PropertyContext Context { get; private set; } = new PropertyContext();

        [EditorData("变量", EditorDataType.List)]
        public List<CustomProperty> Properties { get; set; } = new();

        [field: SerializeReference]
        public List<DataEvent> EventList { get; set; } = new();

        private const int BASE_ID = 0;
        private const int BASE_NAME = 1;
        private const int BASE_TOTALTIME = 2;

        public void Deserialize(JsonData jd)
        {
            var cfg = jd["Base"];
            ID = JsonHelper.ReadInt(cfg[BASE_ID]);
            Name = JsonHelper.ReadString(cfg[BASE_NAME]);
            TotalTime = JsonHelper.ReadInt(cfg[BASE_TOTALTIME]);

            var pList = jd["Prop"];
            for (int i=0; i<pList.Count; ++i)
            {
                var property = new CustomProperty();
                property.Deserialize(pList[i]);
                Properties.Add(property);
                Context.AddProperty(property.Name, property.Value.Value);
            }
            DeserializeEvent(jd["Events"], ae => { EventList.Add(ae); });
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            writer.WriteObjectStart();
            
            writer.WritePropertyName("Base");
            writer.WriteArrayStart();
            writer.Write(ID);
            writer.Write(Name);
            writer.Write(TotalTime);
            writer.WriteArrayEnd();
            // JsonHelper.WriteProperty(ref writer, "ID", ID);
            // JsonHelper.WriteProperty(ref writer, "Name", Name);
            // JsonHelper.WriteProperty(ref writer, "TotalTime", TotalTime);
            
            SerializeEvent(ref writer);
            
            writer.WritePropertyName("Prop");
            writer.WriteArrayStart();
            using (var itr = Properties.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    writer = itr.Current.Serialize(writer);
                }
            }
            writer.WriteArrayEnd();
            
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