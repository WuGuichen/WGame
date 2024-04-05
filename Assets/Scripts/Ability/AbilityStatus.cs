using System;
using System.Collections.Generic;

namespace WGame.Ability
{
    public abstract class AbilityStatus
    {
        protected AbilityData _ability;
        private float _curTime;
        public bool IsToEnd => (_ability.TotalTime - _millisecondTime) < 10;
        private int _millisecondTime;
        public string AbilityName => _ability.Name;
        public int AbilityID => _ability.ID;
        public bool IsEnable { get; private set; }

        private int _curTriggerredIndex;
        protected LinkedList<DataEvent> _durationList = new();
        private List<DataEvent> _waitDeleteList = new();

        protected void Reset()
        {
            _curTime = 0;
            _millisecondTime = 0;
            _curTriggerredIndex = 0;
            _ability = null;
            IsEnable = false;
            var itr = _durationList.GetEnumerator();
            while (itr.MoveNext())
            {
                var item = itr.Current;
                OnExitDuration(item.EventData, true);
            }
            _durationList.Clear();
        }

        protected void Initialize(AbilityData ability)
        {
            if (ability == null)
            {
                return;
            }
            Reset();
            _ability = ability;
            IsEnable = true;
            OnStart();
            Update(0);
        }

        public void SetTime(float time)
        {
            _curTime = time;
        }

        public void Process(float deltaTime)
        {
            if (!IsEnable)
            {
                return;
            }
            
            _curTime += deltaTime;
            _millisecondTime = (int)(_curTime * 1000);
            Update(deltaTime);
            
            if (_millisecondTime > _ability.TotalTime)
            {
                IsEnable = false;
                OnEnd();
            }
        }

        private void Update(float deltaTime)
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
                            if (eventData.IsEnable)
                            {
                                OnTriggerSignal(eventData.EventData);
                            }
                        }
                        break;
                    case ETriggerType.Duration:
                        if (checkTime)
                        {
                            if (eventData.IsEnable)
                            {
                                OnEnterDuration(eventData.EventData);
                                _durationList.AddLast(eventData);
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var itr = _durationList.GetEnumerator();
            while (itr.MoveNext())
            {
                OnProcessDuration(itr.Current, deltaTime);
            }
            foreach (var dataEvent in _waitDeleteList)
            {
                _durationList.Remove(dataEvent);
            }
            _waitDeleteList.Clear();
        }

        private void OnProcessDuration(DataEvent dataEvent, float deltaTime)
        {
            if (_millisecondTime > dataEvent.EndTime)
            {
                OnExitDuration(dataEvent.EventData, false);
                _waitDeleteList.Add(dataEvent);
            }
            else
            {
                OnProcessDuration(dataEvent.EventData, deltaTime,_millisecondTime - dataEvent.TriggerTime, dataEvent.Duration);
            }
        }

        protected abstract void OnExitDuration(IEventData eventData, bool isBreak);
        protected abstract void OnProcessDuration(IEventData eventData, float deltaTime, int duration, int totalTime);
        protected abstract void OnEnterDuration(IEventData eventData);

        protected abstract void OnTriggerSignal(IEventData eventData);

        protected virtual void OnEnd()
        {
        }

        protected virtual void OnStart()
        {
            WLogger.Print($"start {_ability.Name}");
        }
    }
}