using System;
using LitJson;
using UnityEngine;
using WGame.Utils;

namespace WGame.Ability
{
    public enum ETriggerType
    {
        Signal = 0,
        Duration,
    }
    
    [Serializable]
    public sealed class DataEvent : IData
    {
        [SerializeField] private string _trackName;
        [SerializeField] private int _trackIndex;
        [SerializeField] private int _triggerTime;
        [SerializeField] private int _duration;
        [SerializeField] private ETriggerType _triggerType;
        [SerializeField] private EventDataType _dataType;
        [SerializeReference] private IEventData _eventData;
        
        [System.NonSerialized] private int _curTime = 0;
        
        public ETriggerType TriggerType
        {
            get => _triggerType;
            set => _triggerType = value;
        }
        
        public int TriggerTime
        {
            get => _triggerTime;
            set => _triggerTime = value;
        }
        
        public int Duration
        {
            get => _duration;
            set => _duration = value;
        }
        
        public EventDataType EventType
        {
            get => _dataType;
            set
            {
                if (_dataType != value)
                {
                    _dataType = value;
                    switch (_dataType)
                    {
                        case EventDataType.None:
                            EventData = null;
                            break;
                        case EventDataType.PlayAnim:
                            EventData = new EventPlayAnim();
                            break;
                        case EventDataType.PlayEffect:
                            EventData = new EventPlayEffect();
                            break;
                        case EventDataType.NoticeMessage:
                            EventData = new EventNoticeMessage();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
        public string TrackName
        {
            get => _trackName;
            set => _trackName = value;
        }
        public int TrackIndex
        {
            get => _trackIndex;
            set => _trackIndex = value;
        }
        
        public IEventData EventData
        {
            get => _eventData;
            set => _eventData = value;
        }

        public string DebugName => TrackIndex.ToString();

        public void Deserialize(JsonData jd)
        {
            _trackName = JsonHelper.ReadString(jd["TrackName"]);
            _trackIndex = JsonHelper.ReadInt(jd["TrackIndex"]);

            _triggerType = JsonHelper.ReadEnum<ETriggerType>(jd["TriggerType"]);
            _triggerTime = JsonHelper.ReadInt(jd["TriggerTime"]);
            _duration = JsonHelper.ReadInt(jd["Duration"]);
            EventType = JsonHelper.ReadEnum<EventDataType>(jd["EventType"]);
            
            (_eventData as IData).Deserialize(jd["EventData"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            writer.WriteObjectStart();
            JsonHelper.WriteProperty(ref writer, "TrackName", _trackName);
            JsonHelper.WriteProperty(ref writer, "TrackIndex", _trackIndex);

            JsonHelper.WriteProperty(ref writer, "TriggerType", _triggerType.ToString());
            JsonHelper.WriteProperty(ref writer, "TriggerTime", _triggerTime);
            JsonHelper.WriteProperty(ref writer, "Duration", _duration);
            JsonHelper.WriteProperty(ref writer, "EventType", _dataType.ToString());

            writer.WritePropertyName("EventData");
            writer.WriteObjectStart();
            writer = (EventData as IData).Serialize(writer);
            writer.WriteObjectEnd();

            writer.WriteObjectEnd();

            return writer;
        }
        
        public bool CheckTime(int deltaTime)
        {
            _curTime += deltaTime;
            return _curTime <= Duration;
        }
        
        public void Reset()
        {
            _curTime = 0;
        }
        
        public void CopyTo(DataEvent evt)
        {
            evt._trackName = this._trackName;
            evt._trackIndex = this.TrackIndex;
            evt._triggerTime = this._triggerTime;
            evt._duration = this._duration;
            evt._triggerType = this._triggerType;
            evt._dataType = this._dataType;
            evt._eventData = this._eventData.Clone();
        }
    }
}