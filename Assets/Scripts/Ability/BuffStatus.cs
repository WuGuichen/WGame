namespace WGame.Ability
{
    public abstract class BuffStatus
    {
        protected BuffData _buff;
        protected BuffManager _mgr = null;
        public BuffManager BuffMgr => _mgr;
        private float _lifeTime;
        private float _lifePercentTime = 0f;
        public float LifePercentTime => _lifePercentTime;
        private float _totalTime;

        public int EntityID => _mgr.Owner.EntityID;
        public string Name => _buff.Name;
        public BuffAddType AddType => _buff.AddType;
        
        protected const float mPrecision = 0.001f;
        public bool IsEnable { get; private set; }

        protected virtual void Reset()
        {
            _lifeTime = 0f;
            _totalTime = 0f;
            _lifePercentTime = 0f;
            _buff = null;
            _mgr = null;
            IsEnable = false;
        }

        public virtual bool Initialize(BuffManager buffManager, BuffData buff)
        {
            Reset();
            _buff = buff;
            _mgr = buffManager;
            ResetTime();
            IsEnable = true;
            return true;
        }
        
        public void ResetTime()
        {
            _totalTime = _buff.Duration*mPrecision;
            _lifeTime = 0f;
            _lifePercentTime = 0f;
        }

        public virtual void OnUpdate(float deltaTime)
        {
            _lifeTime += deltaTime;
            _lifePercentTime = _lifeTime / _totalTime;
        }
        protected virtual void OnRemove(){}
        
        public virtual bool HasFinished()
        {
            return _buff.Duration > 0 && _lifeTime >= _totalTime;
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