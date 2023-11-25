namespace WGame.Trigger
{
    public class WtEventListener
    {
        private WtEventBridge _bridge;
        private int _type;

        public WtEventListener(WtEventDispatcher owner, int type)
        {
            _bridge = owner.GetEventBridge(type);
            _type = type;
        }

        public int type => _type;

        public void Add(WtEventCallback0 callback)
        {
            _bridge.Add(callback);
        }
        
        public void Add(WtEventCallback1 callback)
        {
            _bridge.Add(callback);
        }

        public void Remove(WtEventCallback0 callback)
        {
            _bridge.Remove(callback);
        }

        public void Remove(WtEventCallback1 callback)
        {
            _bridge.Remove(callback);
        }

        public void Set(WtEventCallback0 callback)
        {
            _bridge.Clear();
            if(callback != null)
                _bridge.Add(callback);
        }
        
        public void Set(WtEventCallback1 callback)
        {
            _bridge.Clear();
            if(callback != null)
                _bridge.Add(callback);
        }

        public bool isEmpty => !_bridge.owner.hasTrigger(_type);
        public bool isDispatching => _bridge.owner.isDispatching(_type);

        public void Clear()
        {
            _bridge.Clear();
        }
        
        public bool Call()
        {
            return _bridge.owner.InternalDispatchEvent(_type, _bridge, null, null);
        }

        public bool Call(WTrigger.Context data)
        {
            return _bridge.owner.InternalDispatchEvent(_type, _bridge, data, null);
        }
        
        public bool BubbleCall()
        {
            return _bridge.owner.BubbleEvent(_type, null);
        }
        
        public bool BubbleCall(WTrigger.Context data)
        {
            return _bridge.owner.BubbleEvent(_type, data);
        }

        public bool BroadcastCall()
        {
            return _bridge.owner.BroadcastEvent(_type, null);
        }
        
        public bool BroadcastCall(WTrigger.Context data)
        {
            return _bridge.owner.BroadcastEvent(_type, data);
        }
    }
}