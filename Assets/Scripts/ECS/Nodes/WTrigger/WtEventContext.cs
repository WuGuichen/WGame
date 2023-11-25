using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WGame.Trigger
{
    public class WtEventContext
    {
        public WtEventDispatcher sender { get; internal set; }
        
        public object initiator { get; internal set; }

        public int type;

        public object data;
        
        internal bool _defaultPrevented;
        internal bool _stopsPropagation;
        
        internal List<WtEventBridge> callChain = new List<WtEventBridge>();

        private static Stack<WtEventContext> pool = new Stack<WtEventContext>();

        public bool isDefaultPrevented => _defaultPrevented;

        public void StopPropagation() => _stopsPropagation = true;
        public void PreventDefault() => _defaultPrevented = true;

        internal static WtEventContext Get()
        {
            if (pool.Count > 0)
            {
                var context = pool.Pop();
                context._defaultPrevented = false;
                context._stopsPropagation = false;
                return context;
            }
            else
            {
                return new WtEventContext();
            }
        }

        internal static void Push(WtEventContext value)
        {
            pool.Push(value);
        }
    }
}