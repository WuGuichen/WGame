namespace WGame.Trigger
{
    class WtEventBridge
    {
        public WtEventDispatcher owner;

        private WtEventCallback0 _callback0;
        private WtEventCallback1 _callback1;

        internal bool _dispatching;

        public WtEventBridge(WtEventDispatcher owner)
        {
            this.owner = owner;
        }

        public void Add(WtEventCallback0 callback)
        {
            _callback0 -= callback;
            _callback0 += callback;
        }

        public void Remove(WtEventCallback0 callback)
        {
            _callback0 -= callback;
        }
        
        public void Add(WtEventCallback1 callback)
        {
            _callback1 -= callback;
            _callback1 += callback;
        }

        public void Remove(WtEventCallback1 callback)
        {
            _callback1 -= callback;
        }

        public bool isEmpty => (_callback1 == null && _callback0 == null);

        public void Clear()
        {
            _callback0 = null;
            _callback1 = null;
        }

        public void CallInternal(WtEventContext context)
        {
            _dispatching = true;
            context.sender = owner;
            try
            {
                _callback1?.Invoke(context);
                _callback0?.Invoke();
            }
            finally
            {
                _dispatching = false;
            }
        }
    }
}