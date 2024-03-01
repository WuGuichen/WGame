using LitJson;
using UnityEngine;

namespace WGame.Ability
{
    public class DataEvent : IData
    {
        public string DebugName { get; }

        [SerializeField] private int _triggerTime;
        [SerializeField] private int _duration;
        [SerializeField] private EventTriggerType _triggerType;
        [SerializeField] private EventDataType _dataType;
        [SerializeReference] private IEventData _eventData;
        
        public void Deserialize(JsonData jsonData)
        {
            throw new System.NotImplementedException();
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}