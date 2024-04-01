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
        [SerializeField] private EventDataType _dataType = EventDataType.DoAction;
        [SerializeReference] private IEventData _eventData = new EventDoAction();
        
        [System.NonSerialized] private int _curTime = 0;
        public bool IsEnable { get; set; } = true;
        
        public int EndTime { get; private set; }
        // [EditorData("触发类型", EditorDataType.Enum)]
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

        public static IEventData GetEventData(EventDataType dataType)
        {
            IEventData data = null;
            switch (dataType)
            {
                // case EventDataType.None:
                //     EventData = null;
                //     break;
                case EventDataType.PlayAnim:
                    data = new EventPlayAnim();
                    break;
                case EventDataType.Interrupt:
                    data = new EventInterrupt();
                    break;
                case EventDataType.DoAction:
                    data = new EventDoAction();
                    break;
                case EventDataType.LockTick:
                    data = new EventLockTick();
                    break;
                case EventDataType.SetState:
                    data = new EventSetState();
                    break;
                case EventDataType.SetMoveParam:
                    data = new EventSetMoveParam();
                    break;
                case EventDataType.TriggerInputToMotion:
                    data = new EventInputTriggerToMotion();
                    break;
                case EventDataType.TriggerInputToAbility:
                    data = new EventInputTriggerToAbility();
                    break;
                case EventDataType.TriggerStateToMotion:
                    data = new EventStateToMotion();
                    break;
                case EventDataType.TriggerStateToAbility:
                    data = new EventStateToAbility();
                    break;
                case EventDataType.PlayEffect:
                    data = new EventPlayEffect();
                    break;
                case EventDataType.NoticeMessage:
                    data = new EventNoticeMessage();
                    break;
                case EventDataType.MoveToPoint:
                    data = new EventMoveToPoint();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return data;
        }

        [EditorData("事件类型", EditorDataType.EnumNamed)]
        public EventDataType EventType
        {
            get => _dataType;
            set
            {
                if (_dataType != value)
                {
                    _dataType = value;
                    EventData = GetEventData(_dataType);
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

        private const int TRACK_NAME = 0;
        private const int TRACK_INDEX = 1;
        private const int TRIGGER_TYPE = 2;
        private const int TRIGGER_TIME = 3;
        private const int IS_NABLE = 4;
        private const int DURATION = 5;

        public void Deserialize(JsonData jd)
        {
            var cfg = jd["Cfg"];
            _trackName = JsonHelper.ReadString(cfg[TRACK_NAME]);
            _trackIndex = JsonHelper.ReadInt(cfg[TRACK_INDEX]);
            _triggerType = JsonHelper.ReadEnum<ETriggerType>(cfg[TRIGGER_TYPE]);
            _triggerTime = JsonHelper.ReadInt(cfg[TRIGGER_TIME]);
            IsEnable = JsonHelper.ReadBool(cfg[IS_NABLE]);
            _duration = JsonHelper.ReadInt(cfg[DURATION]);
            EndTime = _triggerTime + _duration;
            EventType = JsonHelper.ReadEnum<EventDataType>(jd["Type"]);
            
            (_eventData as IData).Deserialize(jd["Data"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("Cfg");
            writer.WriteArrayStart();
            writer.Write(_trackName);
            writer.Write(_trackIndex);
            writer.Write(_triggerType.ToString());
            writer.Write(_triggerTime);
            writer.Write(IsEnable);
            writer.Write(_duration);
            // JsonHelper.WriteProperty(ref writer, "TrackName", _trackName);
            // JsonHelper.WriteProperty(ref writer, "TrackIndex", _trackIndex);
            //
            // JsonHelper.WriteProperty(ref writer, "TriggerType", _triggerType.ToString());
            // JsonHelper.WriteProperty(ref writer, "TriggerTime", _triggerTime);
            // JsonHelper.WriteProperty(ref writer, "Duration", _duration);
            //
            writer.WriteArrayEnd();
            JsonHelper.WriteProperty(ref writer, "Type", _dataType.ToString());

            writer.WritePropertyName("Data");
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