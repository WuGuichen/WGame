namespace WGame.Ability
{
    public abstract class BuffStatus
    {
        protected BuffData _buff;
        protected BuffManager _mgr = null;
        public BuffManager BuffMgr => _mgr;
        private float _leftTime;
        private int _leftTimeMillisecond;
        private int _totalTime;

        public int ID => _buff.ID;
        public string Name => _buff.Name;
        
        protected const float mPrecision = 0.001f;
        public bool IsEnable { get; private set; }

        protected virtual void Reset()
        {
            _leftTime = 0;
            _leftTimeMillisecond = 0;
            _totalTime = 0;
            _buff = null;
            _mgr = null;
            IsEnable = false;
        }

        public virtual void Initialize(BuffManager buffManager, BuffData buff)
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

        public virtual void OnUpdate(float deltaTime)
        {
            _leftTime += deltaTime;
        }
        protected virtual void OnRemove(){}
        
        public virtual bool HasFinished()
        {
            return _buff.Duration > 0 && _leftTime >= _totalTime;
        }

        public abstract int ChangeAttrType();

        public virtual void Apply(int attrType, ref int addVal, ref int mulVal)
        {

        }

        public static void PushBuff(BuffStatus buff)
        {
            if (buff is CBuffStatus cBuffStatus)
            {
                CBuffStatus.Push(cBuffStatus);
            }
            else
            {
                buff.Reset();
            }
        }
    }
}