using System.Collections.Generic;

namespace WGame.Attribute
{
    public delegate void WaEventCallback0();

    public delegate void WaEventCallback1(WaEventContext context);
    
    public class WaEventContext
    {
        private static Stack<WaEventContext> pool = new Stack<WaEventContext>();

        public GameEntity sender;
        public int buffID;
        public int attrID;
        public int changedValue;
        public int value;
        
        internal static WaEventContext Get()
        {
            if (pool.Count > 0)
            {
                var context = pool.Pop();
                context.buffID = -1;
                context.attrID = -1;
                return context;
            }
            else
            {
                var context = new WaEventContext();
                context.buffID = -1;
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
            value.buffValue.Clear();
            value.value = 0;
            pool.Push(value);
        }

        public int value { get; private set; }
        // value减去buffValue就是基本属性值, 如果有加基本属性值的buff再说
        private Dictionary<int, int> buffValue;
        private AttrBridge bridge;
        public AttrBridge Bridge
        {
            get => bridge;
        }

        public void AddValue(int addValue)
        {
            value += addValue;
        }
        
        public int AddBuff(int buffID, int buffVal)
        {
            if (this.buffValue.TryGetValue(buffID, out int val))
            {
                if (val != buffVal)
                {
                    buffValue[buffID] = buffVal;
                    int changedVal = buffVal - val;
                    AddValue(changedVal);
                    return changedVal;
                }
            }

            return 0;
        }

        public AttrType()
        {
            buffValue = new Dictionary<int, int>();
            bridge = new AttrBridge();
        }
        
        public void CallChange(GameEntity sender, int attrID, int buffID, int changedValue,int value)
        {
            var context = new WaEventContext();
            context.sender = sender;
            context.attrID = attrID;
            context.buffID = buffID;
            context.changedValue = changedValue;
            context.value = value;
            bridge.CallInternal(context);
        }
    }

    public class WAttribute
    {
        private Dictionary<int, AttrType> _attrDict = new Dictionary<int, AttrType>();

        private GameEntity owner;
        
        public WAttribute(GameEntity owner)
        {
            this.owner = owner;
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
                oldValue.AddValue(changedValue);
                oldValue.CallChange(sender, attrID, 0, changedValue, value);
            }
            else
            {
                var attr = AttrType.Get();
                attr.AddValue(value);
                _attrDict[attrID] = attr;
            }
        }

        public void Add(GameEntity sender, int attrID, int value)
        {
            if (_attrDict.TryGetValue(attrID, out var attr))
            {
            }
        }

        public void AddBuff(GameEntity sender, int attrID, int buffID)
        {
            if (_attrDict.TryGetValue(attrID, out var oldValue))
            {
            }
        }
        
        public int Get(int attrID)
        {
            if (_attrDict.TryGetValue(attrID, out var oldValue))
            {
                return oldValue.value;
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

            owner = null;
        }
    }
}