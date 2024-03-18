using System.Collections.Generic;

namespace WGame.Ability
{
    public sealed class CBuffStatus : BuffStatus
    {
        #region  pool

        private static Stack<CBuffStatus> _pool = new();

        public static CBuffStatus Get(BuffManager manager, BuffData buffData)
        {
            if (_pool.Count > 0)
            {
                var res = _pool.Pop();
                res.Initialize(manager ,buffData);
                return res;
            }

            return new CBuffStatus(manager, buffData);
        }

        private CBuffStatus(BuffManager manager, BuffData buffData)
        {
            Initialize(manager ,buffData);
        }

        #endregion
        
        private List<ICondition> _conditionList = new();

        protected override void Initialize(BuffManager buffManager, BuffData buff)
        {
            base.Initialize(buffManager, buff);

            var cb = buff as CBuffData;
            using var itr = cb.ConditionList.GetEnumerator();
            while (itr.MoveNext())
            {
                var bc = itr.Current.Clone();
                bc.Init(this);

                _conditionList.Add(bc);
            }
        }

        protected override void Reset()
        {
            using var itr = _conditionList.GetEnumerator();
            while (itr.MoveNext())
            {
                itr.Current.Destroy();
            }

            _conditionList.Clear();
            
            base.Reset();
        }

        public void OnTrigger()
        {
            bool check = true;
            using (var itr = _conditionList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    check = itr.Current.CheckBuff(_mgr.Owner);
                    if (!check)
                        break;
                }
            }
            
            // 所有条件满足
            if (check)
            {
                var cp = _buff as CBuffData;
                if (cp.AddBuffList.Count > 0)
                {
                    _mgr.Owner.AddBuff(cp.AddBuffList);
                }
                if (cp.RemoveBuffList.Count > 0)
                {
                    _mgr.Owner.RemoveBuff(cp.RemoveBuffList);
                }

                using var itr = _conditionList.GetEnumerator();
                while (itr.MoveNext())
                {
                    itr.Current.Reset();
                }
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnRemove()
        {
            throw new System.NotImplementedException();
        }
    }
}