using System;
using System.Collections.Generic;

namespace WGame.Ability
{
    public class BuffManager
    {
        private const float _precision = 0.001f;
        
        private LinkedList<BuffStatus> _buffList = new();
        private bool isNeedRefreshGetBuff = false;

        public LinkedList<BuffStatus> BuffList => _buffList;

        private LinkedList<BuffStatus> _addList = new();
        private LinkedList<BuffStatus> _handleList = new();
        private LinkedList<BuffStatus> _singleHandleList = new();

        private HashSet<string> _updatedBuff = new();

        public BuffOwner Owner { get; private set; }

        public Action<BuffStatus> onBuffUpdate { get; set; }
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
                    if (itr.Current.AddType == BuffAddType.UseSingleTime)
                    {
                        _singleHandleList.AddLast(itr.Current);
                    }
                    else
                    {
                        itr.Current.OnUpdate(deltaTime);
                        onBuffUpdate?.Invoke(itr.Current);
                        if (itr.Current.HasFinished())
                        {
                            _handleList.AddLast(itr.Current);
                        }
                    }
                }
            }
            
            using (var itr = _singleHandleList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    var item = itr.Current;
                    if (_updatedBuff.Contains(item.Name))
                    {
                        continue;
                    }
                    _updatedBuff.Add(item.Name);
                    item.OnUpdate(deltaTime);
                    onBuffUpdate?.Invoke(itr.Current);
                    if (item.HasFinished())
                    {
                        _handleList.AddLast(itr.Current);
                    }
                }
            }
            
            _updatedBuff.Clear();
            _singleHandleList.Clear();
            
            using (var itr = _handleList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    _buffList.Remove(itr.Current);
                    onBuffRemoved.Invoke(itr.Current);
                    BuffStatus.PushBuff(itr.Current);
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

            if (_addList.Count > 0)
            {
                if (buffData.AddType == BuffAddType.RefreshAllTime)
                {
                    foreach (var buffStatus in _addList)
                    {
                        buffStatus.ResetTime();
                    }
                }

                if (_addList.Count == buffData.AddNum)
                {
                    if (buffData.AddType != BuffAddType.ReplaceFist)
                    {
                        WLogger.Print("Return");
                        _addList.Clear();
                        return;
                    }
                    else
                    {
                        var item = _addList.First.Value;
                        _buffList.Remove(item);
                        onBuffRemoved.Invoke(item);
                        BuffStatus.PushBuff(item);
                    }
                }

                _addList.Clear();
            }

            BuffStatus buff = null;
            switch (buffFactoryData.BuffType)
            {
                case BuffType.Numerical:
                    buff = new NBuffStatus();
                    break;
                case BuffType.Condition:
                    buff = CBuffStatus.Get();
                    break;
                case BuffType.ChangeAttr:
                    buff = new SBuffChangeAttrStatus();
                    break;
                default:
                    break;
            }

            if (buff.Initialize(this, buffData))
            {
                _buffList.AddLast(buff);
                onBuffAdded.Invoke(buff);
            }
            else
            {
                BuffStatus.PushBuff(buff);
            }
        }
        
        public void DelBuff(string id)
        {
            using (var itr = _buffList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    if (id == itr.Current.Name)
                    {
                        _buffList.Remove(itr.Current);
                        onBuffRemoved.Invoke(itr.Current);
                        BuffStatus.PushBuff(itr.Current);

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