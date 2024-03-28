using System.Collections.Generic;
using UnityEngine;
using WGame.Utils;

namespace WGame.Ability
{
    using LitJson;
    
    public sealed class EventPlayAnim : IData, IEventData
    {
        public string DebugName => AnimName;
        
        [EditorData("动画名", EditorDataType.AnimationClip)]
        public string AnimName { get; set; }
        public EventDataType EventType => EventDataType.PlayAnim;
        public float AnimCrossFadeDuration { get; set; }
        
        public void Deserialize(JsonData jd)
        {
            AnimName = JsonHelper.ReadString(jd["AnimName"]);
            AnimCrossFadeDuration = JsonHelper.ReadFloat(jd["FadeTime"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "AnimName", AnimName);
            JsonHelper.WriteProperty(ref writer, "FadeTime", AnimCrossFadeDuration);

            return writer;
        }

        public IEventData Clone()
        {
            EventPlayAnim evt = new EventPlayAnim();
            evt.AnimName = this.AnimName;
            evt.AnimCrossFadeDuration = this.AnimCrossFadeDuration;

            return evt;
        }
    }
}