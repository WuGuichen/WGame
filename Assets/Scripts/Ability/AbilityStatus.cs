using System;

namespace WGame.Ability
{
    public abstract class AbilityStatus
    {
        protected AbilityData _ability;
        private float _curTime;
        private int _millisecondTime;
        public bool IsEnable { get; private set; }

        private int _curTriggerredIndex;

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
                            OnEnterDuration(eventData.EventData);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OnEnterDuration(IEventData eventData)
        {
            if (eventData is EventPlayAnim playAnim)
            {
                OnDurationPlayAnim(playAnim);
            }
        }

        private void OnTriggerSignal(IEventData eventData)
        {
            if (eventData is EventPlayEffect playEffect)
            {
                OnTriggerPlayEffect(playEffect);
            }
        }

        protected abstract void OnDurationPlayAnim(EventPlayAnim animData);
        protected abstract void OnTriggerPlayEffect(EventPlayEffect effectData);

        protected virtual void OnEnd()
        {
            WLogger.Print($"End {_ability.Name}");
        }

        protected virtual void OnStart()
        {
            WLogger.Print($"start {_ability.Name}");
        }
    }
}