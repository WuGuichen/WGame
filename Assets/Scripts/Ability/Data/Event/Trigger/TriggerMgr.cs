using System;
using System.Collections.Generic;
using WGame.Runtime;

namespace WGame.Ability
{
    public sealed class TriggerMgr : Singleton<TriggerMgr>
    {
        private Dictionary<int, LinkedList<Action<TriggerContext>>> _triggerHandlerDict = new();
        private LinkedList<TriggerContext> _contextList = new LinkedList<TriggerContext>();
        private LinkedList<TriggerContext> _handlerList = new LinkedList<TriggerContext>();

        public override void InitInstance()
        {
            
        }

        public void Dispose()
        {
            _triggerHandlerDict.Clear();
            _contextList.Clear();
        }
        
        public void Register(int name, Action<TriggerContext> handler)
        {
            LinkedList<Action<TriggerContext>> handlers = null;
            if (!_triggerHandlerDict.TryGetValue(name, out handlers))
            {
                handlers = new LinkedList<Action<TriggerContext>>();
                _triggerHandlerDict[name] = handlers;
            }

            handlers.AddLast(handler);
        }

        public void Unregister(int name, Action<TriggerContext> handler)
        {
            if (null == handler)
            {
                _triggerHandlerDict.Remove(name);
                return;
            }

            LinkedList<Action<TriggerContext>> handlers = null;
            if (_triggerHandlerDict.TryGetValue(name, out handlers))
            {
                handlers.Remove(handler);
            }
        }
        
        public void Trigger(TriggerContext context)
        {
            if (_triggerHandlerDict.TryGetValue(context.TriggerType, out var handlers))
            {
                using var itr = handlers.GetEnumerator();
                while (itr.MoveNext())
                {
                    itr.Current.Invoke(context);
                }
            }
        }

        public void PostMessage(TriggerContext msg)
        {
            lock (_contextList)
            {
                _contextList.AddLast(msg);
            }
        }

        public void Update(float deltaTime)
        {
            lock (_contextList)
            {
                using (var itr = _contextList.GetEnumerator())
                {
                    while (itr.MoveNext())
                    {
                        _handlerList.AddLast(itr.Current);
                    }
                }

                _contextList.Clear();
            }

            using (var itr = _handlerList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    Trigger(itr.Current);
                }
            }

            _handlerList.Clear();
        }

    }
}