using System;
using System.Collections.Generic;

namespace WGame.Trigger
{
    public delegate void WtEventCallback0();
    public delegate void WtEventCallback1(WtEventContext context);

    public class WtEventDispatcher : IWtEventDispatcher
    {
        private Dictionary<int, WtEventBridge> _dic;

        public WtEventDispatcher()
        {
            
        }

        WtEventBridge GetBridge(int type)
        {
            if (type == 0)
                throw new Exception("你要给事件添加唯一ID");
            if (_dic == null)
                _dic = new Dictionary<int, WtEventBridge>();

            WtEventBridge bridge = null;
            if (!_dic.TryGetValue(type, out bridge))
            {
                bridge = new WtEventBridge(this);
                _dic[type] = bridge;
            }
            
            return bridge;
        }
        
        public void AddTrigger(int type, WtEventCallback0 callback)
        {
            GetBridge(type).Add(callback);
        }

        public void AddTrigger(int type, WtEventCallback1 callback)
        {
            GetBridge(type).Add(callback);
        }

        public void RemoveTrigger(int type, WtEventCallback0 callback)
        {
            if (_dic == null)
                return;

            if(_dic.TryGetValue(type, out var bridge))
                bridge.Remove(callback);
        }

        public void RemoveTrigger(int type, WtEventCallback1 callback)
        {
            if (_dic == null)
                return;
            
            if(_dic.TryGetValue(type, out var bridge))
                bridge.Remove(callback);
        }

        public void RemoveTriggers(int type = 0)
        {
            if (_dic == null)
                return;

            if (type != 0)
            {
                if(_dic.TryGetValue(type, out var bridge))
                    bridge.Clear();
            }
            else
            {
                foreach (var kv in _dic)
                {
                    kv.Value.Clear();
                }
            }
        }

        public bool hasTrigger(int type)
        {
            var bridge = TryGetEventBridge(type);
            if  (bridge == null)
                return false;
            return bridge.isEmpty;
        }

        public bool isDispatching(int type)
        {
            var bridge = TryGetEventBridge(type);
            if (bridge == null)
                return false;
            return bridge._dispatching;
        }

        internal WtEventBridge TryGetEventBridge(int type)
        {
            if (_dic == null)
                return null;

            _dic.TryGetValue(type, out var bridge);
            return bridge;
        }

        internal WtEventBridge GetEventBridge(int type)
        {
            if (_dic == null)
                _dic = new Dictionary<int, WtEventBridge>();

            if (!_dic.TryGetValue(type, out var bridge))
            {
                bridge = new WtEventBridge(this);
                _dic[type] = bridge;
            }

            return bridge;
        }

        public bool DispatchTrigger(WtEventContext context)
        {
            var bridge = TryGetEventBridge(context.type);

            WtEventDispatcher savedSender = context.sender;

            if (bridge != null && !bridge.isEmpty)
            {
                bridge.CallInternal(context);
            }

            context.sender = savedSender;
            return context.isDefaultPrevented;
        }

        internal bool BubbleEvent(int type, object data, List<WtEventBridge> addChain)
        {
            var context = WtEventContext.Get();
            context.initiator = this;

            context.type = type;
            context.data = data;

            List<WtEventBridge> bubbleChain = context.callChain;
            bubbleChain.Clear();
            
            GetChainBridges(type, bubbleChain, true);

            int length = bubbleChain.Count;
            // for (int i = length - 1; i >= 0; i--)
            // {
            // }
            if (!context._stopsPropagation)
            {
                for (int i = 0; i < length; ++i)
                {
                    bubbleChain[i].CallInternal(context);

                    if (context._stopsPropagation)
                        break;
                }

                if (addChain != null)
                {
                    length = addChain.Count;
                    for (int i = 0; i < length; ++i)
                    {
                        var bridge = addChain[i];
                        if (bubbleChain.IndexOf(bridge) == -1)
                        {
                            bridge.CallInternal(context);
                        }
                    }
                }
            }
            
            WtEventContext.Push(context);
            context.initiator = null;
            context.sender = null;
            context.data = null;
            return context._defaultPrevented;
        }

        public bool BubbleEvent(int type, object data)
        {
            return BubbleEvent(type, data, null);
        }

        public bool BroadcastEvent(int type, object data)
        {
            var context = WtEventContext.Get();
            context.initiator = this;
            context.type = type;
            context.data = data;
            List<WtEventBridge> bubbleChain = context.callChain;
            bubbleChain.Clear();

            int length = bubbleChain.Count;
            for (int i = 0; i < length; ++i)
            {
                bubbleChain[i].CallInternal(context);
            }
            
            WtEventContext.Push(context);
            context.initiator = null;
            context.sender = null;
            context.data = null;
            return context._defaultPrevented;
        }

        internal void GetChainBridges(int type, List<WtEventBridge> chain, bool bubble)
        {
            var bridge = TryGetEventBridge(type);
            if(bridge != null && !bridge.isEmpty)
                chain.Add(bridge);

            if (!bubble)
                return;
        }

        public bool DispatchTrigger(int type)
        {
            return DispatchTrigger(type, null);
        }

        public bool DispatchTrigger(int type, object data)
        {
            return InternalDispatchEvent(type, null, data, null);
        }

        public bool DispatchTrigger(int type, object data, object initiator)
        {
            return InternalDispatchEvent(type, null, data, initiator);
        }
        
        internal bool InternalDispatchEvent(int type, WtEventBridge bridge, object data, object initiator)
        {
            if (bridge == null)
                bridge = TryGetEventBridge(type);

            bool b1 = bridge != null && !bridge.isEmpty;
            if (b1)
            {
                var context = WtEventContext.Get();
                context.initiator = initiator != null ? initiator : this;
                context.type = type;
                context.data = data;

                bridge.CallInternal(context);
                WtEventContext.Push(context);
                context.initiator = null;
                context.sender = null;
                context.data = null;

                return context._defaultPrevented;
            }
            else
            {
                return false;
            }
        }
    }
}