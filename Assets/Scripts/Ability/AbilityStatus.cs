using System;
using System.Collections.Generic;

namespace WGame.Ability
{
    public abstract class AbilityStatus
    {
        protected AbilityData _ability;
        private float _curTime;
        private int _millisecondTime;
        public string AbilityName => _ability.Name;
        public bool IsEnable { get; private set; }

        private int _curTriggerredIndex;
        private LinkedList<DataEvent> _durationList = new();

        protected void Reset()
        {
            _curTime = 0;
            _millisecondTime = 0;
            _curTriggerredIndex = 0;
            _ability = null;
            IsEnable = false;
        }
        
        protected void Initialize(AbilityData ability)
        {
            Reset();
            _ability = ability;
            IsEnable = true;
            OnStart();
            Update();
        }

        public void Process(float deltaTime)
        {
            _millisecondTime = (int)(_curTime * 1000);
            if (!IsEnable)
            {
                return;
            }
            
            Update();
            
            if (_millisecondTime > _ability.TotalTime)
            {
                IsEnable = false;
                OnEnd();
                return;
            }

            _curTime += deltaTime;
        }

        private void Update()
        {
            int eventCount = _ability.EventList.Count;
            for (var i = _curTriggerredIndex; i < eventCount; i++)
            {
                var eventData = _ability.EventList[i];
                bool checkTime = eventData.TriggerTime <= _millisecondTime;
                if (checkTime)
                {
                    _curTriggerredIndex = i+1;
                }
                switch (eventData.TriggerType)
                {
                    case ETriggerType.Signal:
                        if (checkTime)
                        {
                            OnTriggerSignal(eventData.EventData);
                        }
                        break;
                    case ETriggerType.Duration:
                        if (checkTime)
                        {
                            OnEnterDuration(eventData.EventData, eventData.Duration);
                            _durationList.AddLast(eventData);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var itr = _durationList.GetEnumerator();
            while (itr.MoveNext())
            {
                OnProcessDuration(itr.Current);
            }
        }

        private void OnProcessDuration(DataEvent dataEvent)
        {
            if (_curTime > dataEvent.EndTime)
            {
                switch (dataEvent.EventType)
                {
                    case EventDataType.PlayAnim:
                        OnEndPlayAnim(dataEvent.EventData as EventPlayAnim);
                        break;
                    case EventDataType.DoAction:
                        OnEndDoAction(dataEvent.EventData as EventDoAction);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _durationList.Remove(dataEvent);
            }
            else
            {
                // switch (dataEvent.EventType)
                // {
                //     case EventDataType.PlayAnim:
                //         OnDurationPlayAnim(dataEvent.EventData as EventPlayAnim);
                //         break;
                //     case EventDataType.DoAction:
                //         OnDurationDoAction(dataEvent.EventData as EventDoAction);
                //         break;
                //     default:
                //         throw new ArgumentOutOfRangeException();
                // }
            }
        }

        private void OnEnterDuration(IEventData eventData, float duration)
        {
            if (eventData is EventPlayAnim playAnim)
            {
                OnEnterDurationPlayAnim(playAnim);
            }
            else if (eventData.EventType == EventDataType.DoAction)
            {
                OnEnterDurationDoAction(eventData as EventDoAction, duration);
            }
        }

        private void OnTriggerSignal(IEventData eventData)
        {
            if (eventData is EventPlayEffect playEffect)
            {
                OnTriggerPlayEffect(playEffect);
            }
            else if (eventData is EventDoAction doAction)
            {
                OnTriggerDoAction(doAction);
            }
            else if (eventData is EventNoticeMessage noticeMessage)
            {
                OnNoticeMessage(noticeMessage);
            }
        }

        protected abstract void OnEnterDurationDoAction(EventDoAction actionData, float duration);
        protected abstract void OnEndDoAction(EventDoAction actionData);
        protected abstract void OnTriggerDoAction(EventDoAction actionData);
        protected abstract void OnEnterDurationPlayAnim(EventPlayAnim animData);
        protected abstract void OnEndPlayAnim(EventPlayAnim animData);
        protected abstract void OnTriggerPlayEffect(EventPlayEffect effectData);
        protected abstract void OnNoticeMessage(EventNoticeMessage message);

        protected virtual void OnEnd()
        {
            var itr = _durationList.GetEnumerator();
            while (itr.MoveNext())
            {
                var item = itr.Current;
                var eType = item.EventType;
                switch (eType)
                {
                    case EventDataType.PlayAnim:
                        OnEndPlayAnim(item.EventData as EventPlayAnim);
                        break;
                    case EventDataType.DoAction:
                        OnEndDoAction(item.EventData as EventDoAction);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected virtual void OnStart()
        {
            WLogger.Print($"start {_ability.Name}");
        }
    }
}