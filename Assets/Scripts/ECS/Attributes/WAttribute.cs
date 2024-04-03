using System.Collections.Generic;
using WGame.Ability;
using WGame.Utils;

namespace WGame.Attribute
{
    public delegate void WaEventCallback0();

    public delegate void WaEventCallback1(WaEventContext context);
    
    public class WaEventContext
    {
        private static Stack<WaEventContext> pool = new Stack<WaEventContext>();

        public GameEntity sender;
        public GameEntity owner;
        public int attrID;
        public int changedValue;
        public int value;
        
        internal static WaEventContext Get()
        {
            if (pool.Count > 0)
            {
                var context = pool.Pop();
                context.attrID = -1;
                return context;
            }
            else
            {
                var context = new WaEventContext();
                context.attrID = -1;
                return context;
            }
        }

        internal static void Push(WaEventContext value)
        {
            pool.Push(value);
        }
    }

    internal class AttrBridge
    {
        private WaEventCallback0 _callback0;
        private WaEventCallback1 _callback1;

        public void Add(WaEventCallback0 callback)
        {
            _callback0 -= callback;
            _callback0 += callback;
        }

        public void Add(WaEventCallback1 callback)
        {
            _callback1 -= callback;
            _callback1 += callback;
        }

        public void Remove(WaEventCallback0 callback)
        {
            _callback0 -= callback;

        }

        public void Remove(WaEventCallback1 callback)
        {
            _callback1 -= callback;
        }

        public void Clear()
        {
            _callback0 = null;
            _callback1 = null;
        }

        public void CallInternal(WaEventContext context)
        {
            _callback0?.Invoke();
            _callback1?.Invoke(context);
        }
    }
    
    internal class AttrType
    {
        internal static Stack<AttrType> pool = new Stack<AttrType>();

        internal static AttrType Get()
        {
            if (pool.Count > 0)
            {
                var value = pool.Pop();
                return value;
            }

            return new AttrType();
        }

        internal static void Push(AttrType value)
        {
            value.bridge.Clear();
            value.value = 0;
            pool.Push(value);
        }

        public int value { get; private set; }
        private AttrBridge bridge;
        public AttrBridge Bridge
        {
            get => bridge;
        }

        public void AddValue(int addValue)
        {
            value += addValue;
        }
        
        public AttrType()
        {
            bridge = new AttrBridge();
        }
        
        public void CallChange(GameEntity sender, int attrID, int changedValue,int value, GameEntity owner)
        {
            var context = new WaEventContext();
            context.sender = sender;
            context.owner = owner;
            context.attrID = attrID;
            context.changedValue = changedValue;
            context.value = value;
            bridge.CallInternal(context);
            var msg = TriggerContext.Get(TriggerEventType.ChangeAttr);
            msg.AddProperty("target", DataType.Int, owner.instanceID.ID);
            TriggerMgr.Inst.Trigger(msg);
        }
    }

    public class WAttribute
    {
        private Dictionary<int, AttrType> _attrDict = new Dictionary<int, AttrType>();

        private GameEntity owner;
        private BuffManager _buff;
        
        public WAttribute(GameEntity owner)
        {
            this.owner = owner;
        }

        public void Init()
        {
            _buff = this.owner.linkAbility.Ability.abilityService.service.BuffManager;
            _buff.onBuffAdded += OnAddBuff;
            _buff.onBuffRemoved += OnRemoveBuff;
        }
        
        public void Remove(int attrID)
        {
            _attrDict.Remove(attrID);
        }
        
        public void Set(int attrID, int value)
        {
            Set(owner, attrID, value);
        }

        public void Set(GameEntity sender, int attrID, int value)
        {
            if (_attrDict.TryGetValue(attrID, out var oldValue))
            {
                if (oldValue.value == value)
                    return;
                int changedValue = value - oldValue.value;
                if (changedValue != 0)
                {
                    oldValue.AddValue(changedValue);
                    oldValue.CallChange(sender, attrID, changedValue, value, owner);
                }
            }
            else
            {
                var attr = AttrType.Get();
                attr.AddValue(value);
                _attrDict[attrID] = attr;
            }
        }

        public void Add(GameEntity sender, int attrID, int changeValue)
        {
            if (_attrDict.TryGetValue(attrID, out var oldValue))
            {
                var value = oldValue.value;
                if (changeValue < -value)
                {
                    changeValue = value;
                }

                if (changeValue != 0)
                {
                    oldValue.AddValue(changeValue);
                    oldValue.CallChange(sender, attrID, changeValue, oldValue.value, owner);
                }
            }
            else
            {
                var attr = AttrType.Get();
                attr.AddValue(changeValue);
                _attrDict[attrID] = attr;
            }
        }
        
        private void OnAddBuff(BuffStatus buff)
        {
            var change = buff.ChangeAttrType();
            if (change >= 0)
            {
                if (_attrDict.TryGetValue(change, out var value))
                {
                    value.CallChange(owner, change, 0, Get(change, true), owner);
                }
            }
        }
        
        private void OnRemoveBuff(BuffStatus buff)
        {
            var change = buff.ChangeAttrType();
            if (change >= 0)
            {
                if (_attrDict.TryGetValue(change, out var value))
                {
                    value.CallChange(owner, change, 0, Get(change, true), owner);
                }
            }
        }
        
        public int Get(int attrID, bool containBuff = false)
        {
            if (containBuff)
            {
                var res = 0f;
                if (_attrDict.TryGetValue(attrID, out var oldValue))
                {
                    res = oldValue.value;
                }
                float addBuffVal = _buff.Apply(attrID, res);
                return (int)addBuffVal;
            }
            else
            {
                if (_attrDict.TryGetValue(attrID, out var oldValue))
                {
                    return oldValue.value;
                }
            }

            return 0;
        }

        public void RegisterEvent(int attrID, WaEventCallback0 callback)
        {
            if (_attrDict.TryGetValue(attrID, out var attr))
            {
                attr.Bridge.Add(callback);
            }
        }
        
        public void RegisterEvent(int attrID, WaEventCallback1 callback)
        {
            if (_attrDict.TryGetValue(attrID, out var attr))
            {
                attr.Bridge.Add(callback);
            }
        }

        public void CancelEvent(int attrID, WaEventCallback0 callback)
        {
            if (_attrDict.TryGetValue(attrID, out var attr))
            {
                attr.Bridge.Remove(callback);
            }
        }
        
        public void CancelEvent(int attrID, WaEventCallback1 callback)
        {
            if (_attrDict.TryGetValue(attrID, out var attr))
            {
                attr.Bridge.Remove(callback);
            }
        }

        public void Destroy()
        {
            foreach (var kv in _attrDict)
            {
                AttrType.Push(kv.Value);
            }

            _buff.onBuffAdded -= OnAddBuff;
            _buff.onBuffRemoved -= OnRemoveBuff;
            owner = null;
            _buff = null;
        }
    }
}