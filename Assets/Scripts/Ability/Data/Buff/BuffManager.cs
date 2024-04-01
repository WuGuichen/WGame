using System;
using System.Collections.Generic;

namespace WGame.Ability
{
    public class BuffManager
    {
        private const float _precision = 0.001f;
        
        private LinkedList<BuffStatus> _buffList = new();
        private LinkedList<BuffStatus> _addList = new();
        private LinkedList<BuffStatus> _handleList = new();
        public BuffOwner Owner { get; private set; }

        public Action<BuffStatus> onBuffAdded { get; set; }
        public Action<BuffStatus> onBuffRemoved { get; set; }

        public BuffManager(BuffOwner owner)
        {
            Owner = owner;
        }

        public void Update(float deltaTime)
        {
            using (var itr = _buffList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    itr.Current.OnUpdate(deltaTime);
                    if (itr.Current.HasFinished())
                    {
                        _handleList.AddLast(itr.Current);
                    }
                }
            }
            using (var itr = _handleList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    BuffStatus.PushBuff(itr.Current);
                    _buffList.Remove(itr.Current);
                    onBuffAdded.Invoke(itr.Current);
                }
            }
            _handleList.Clear();
        }

        public void AddBuff(string id)
        {
            if (!DataMgr.Inst.TryGetBuffData(id, out var buffFactoryData))
            {
                return;
            }

            var buffData = buffFactoryData.Buff;
            using (var itr = _buffList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    if (id == itr.Current.Name)
                    {
                        _addList.AddLast(itr.Current);
                    }
                }
            }
            
            if (_addList.Count == buffData.AddNum)
            {
                if (buffData.AddType == BuffAddType.AddNone)
                {
                    return;
                }
                else
                {
                    BuffStatus.PushBuff(_addList.First.Value);
                    _addList.RemoveFirst();
                }
            }
            _addList.Clear();
            
            BuffStatus buff = null;
            switch (buffFactoryData.BuffType)
            {
                case BuffType.Numerical:
                    buff = new NBuffStatus();
                    buff.Initialize(this, buffData);
                    break;
                case BuffType.Condition:
                    buff = CBuffStatus.Get(this, buffData);
                    break;
                default:
                    break;
            }

            _buffList.AddLast(buff);
            onBuffAdded.Invoke(buff);
        }
        
        public void DelBuff(int id)
        {
            using (var itr = _buffList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    if (id == itr.Current.ID)
                    {
                        BuffStatus.PushBuff(itr.Current);
                        _buffList.Remove(itr.Current);
                        onBuffRemoved.Invoke(itr.Current);

                        break;
                    }
                }
            }
        }
        
        public float Apply(int attrType, float val)
        {
            int addVal = 0;
            int mulVal = 0;

            using (LinkedList<BuffStatus>.Enumerator itr = _buffList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    itr.Current.Apply(attrType, ref addVal, ref mulVal);
                }
            }

            val = val + addVal * _precision + val * mulVal * _precision * 0.01f;

            return val;
        }
        
        public void Dispose()
        {
            using (var itr = _buffList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    BuffStatus.PushBuff(itr.Current);
                }
            }
            _buffList.Clear();

            Owner = null;
        }

    }
}