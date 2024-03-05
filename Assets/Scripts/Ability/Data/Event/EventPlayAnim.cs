using WGame.Utils;

namespace WGame.Ability
{
    using LitJson;
    using UnityEngine;
    
    public enum WAnimType
    {
        SetBool = 0,
        SetFloat = 1,
        SetInt = 2,
        SetTrigger = 3,
        Force = 4,
    }
    
    public sealed class EventPlayAnim : IData, IEventData
    {
        public string DebugName => GetType().ToString();
        
        public string AnimName { get; set; }
        public WAnimType AnimType { get; set; }
        public string TriggerName { get; set; }
        public string TriggerValue { get; set; }
        public EventDataType EventType => EventDataType.PlayAnim;
        public float AnimCrossFadeDuration { get; set; }
        
        public void Deserialize(JsonData jd)
        {
            AnimName = JsonHelper.ReadString(jd["AnimName"]);
            AnimType = JsonHelper.ReadEnum<WAnimType>(jd["AnimType"]);
            TriggerName = JsonHelper.ReadString(jd["TriggerName"]);
            TriggerValue = JsonHelper.ReadString(jd["TriggerValue"]);
            AnimCrossFadeDuration = JsonHelper.ReadFloat(jd["FadeTime"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "AnimName", AnimName);
            JsonHelper.WriteProperty(ref writer, "AnimType", AnimType.ToString());
            JsonHelper.WriteProperty(ref writer, "TriggerName", TriggerName);
            JsonHelper.WriteProperty(ref writer, "TriggerValue", TriggerValue);
            JsonHelper.WriteProperty(ref writer, "FadeTime", AnimCrossFadeDuration);

            return writer;
        }

        public IEventData Clone()
        {
            EventPlayAnim evt = new EventPlayAnim();
            evt.AnimName = this.AnimName;
            evt.AnimType = this.AnimType;
            evt.TriggerName = this.TriggerName;
            evt.TriggerValue = this.TriggerValue;
            evt.AnimCrossFadeDuration = this.AnimCrossFadeDuration;

            return evt;
        }
    }
}