namespace WGame.Ability
{
    public abstract class BuffStatus
    {
        protected BuffData _buff;
        protected BuffManager _mgr = null;
        private float _leftTime;
        private int _leftTimeMillisecond;
        private int _totalTime;

        public int ID => _buff.ID;
        public bool IsEnable { get; private set; }

        protected virtual void Reset()
        {
            _leftTime = 0;
            _leftTimeMillisecond = 0;
            _totalTime = 0;
            _buff = null;
            IsEnable = false;
        }

        protected virtual void Initialize(BuffManager buffManager, BuffData buff)
        {
            Reset();
            _buff = buff;
            _mgr = buffManager;
            _totalTime = buff.Duration;
            _leftTimeMillisecond = _totalTime;
            _leftTime = _totalTime * 0.001f;
            IsEnable = true;
        }

        public void Process(float deltaTime)
        {
            _leftTimeMillisecond = (int)(_leftTime * 1000);
            
            if (!IsEnable)
            {
                return;
            }
            
            OnUpdate(deltaTime);

            if (_leftTimeMillisecond <= 0)
            {
                IsEnable = false;
                OnRemove();
                return;
            }

            _leftTime -= deltaTime;
        }
        
        protected abstract void OnUpdate(float deltaTime);
        protected abstract void OnRemove();
    }
}