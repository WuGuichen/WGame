using System;
using System.Collections.Generic;

namespace WGame.Trigger
{
    public class WTrigger : WtEventDispatcher
    {
        #region pool

        private static Stack<WTrigger> _pool = new Stack<WTrigger>();

        public static WTrigger Get(WtEventType type, Func<Context, bool> condition)
        {
            if (_pool.Count > 0)
            {
                var trigger = _pool.Pop();
                trigger.eventType = type;
                trigger.triggerIndex = WTriggerMgr.Inst.GenTriggerIndex(type.mainType);
                trigger.condition = condition;
                trigger.IsActive = true;
                return trigger;
            }

            return new WTrigger(type, condition, WTriggerMgr.Inst.GenTriggerIndex(type.mainType));
        }

        public static void Push(WTrigger trigger)
        {
            trigger.condition = null;
            trigger.IsActive = false;
            trigger.onTrigger.Clear();
            trigger.triggerIndex = -1;
            _pool.Push(trigger);
        }

        #endregion

        private bool isActive;

        public bool IsActive
        {
            get => isActive;
            set
            {
                if(value == false)
                    triggerCount = 0;
                isActive = value;
            }
        }

        public struct Context
        {
            public object obj;
            public int pInt;

            public Context(object obj)
            {
                this.obj = obj;
                pInt = 0;
            }

            public Context(int param)
            {
                pInt = param;
                obj = null;
            }
        }

        public int triggerCount = 0;
        public int triggerIndex = -1;

        private WtEventListener _onTrigger;
        private WtEventListener _onTriggerFail;

        public Func<Context, bool> condition { get; private set; }
        public WtEventType eventType { get; private set; }

        public WTrigger(WtEventType type, Func<Context, bool> condition, int index)
        {
            this.IsActive = true;
            this.condition = condition;
            eventType = type;
            this.triggerIndex = index;
        }

        public WtEventListener onTrigger
        {
            get { return _onTrigger ?? (_onTrigger = new WtEventListener(this, 1)); }
        }

        public WtEventListener onTriggerFail
        {
            get { return _onTriggerFail ?? (_onTrigger = new WtEventListener(this, 2)); }
        }

        public void TryTrigger()
        {
            if (!IsActive)
                return;
            onTrigger.Call();
            triggerCount++;
            // else
            // {
            //     onTriggerFail.Call(data);
            // }
        }

        public void TryTrigger(Context data)
        {
            if (!IsActive)
                return;
            if (condition != null)
            {
                if (condition.Invoke(data))
                {
                    onTrigger.Call();
                    triggerCount++;
                }
            }
            else
            {
                onTrigger.Call();
                triggerCount++;
            }
        }
    }
}