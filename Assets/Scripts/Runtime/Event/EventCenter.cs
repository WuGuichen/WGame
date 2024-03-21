using System.Collections.Generic;
using WGame.Utils;

namespace WGame.Runtime
{
    public delegate void WEventCallback0();
    public delegate void WEventCallback1(TAny context);
    // public delegate void WEventCallback2(TAny);
    public class WEventContext
    {
        private static Stack<WEventContext> pool = new Stack<WEventContext>();
        
        public static WEventContext Get(int pInt)
        {
            if (pool.Count > 0)
            {
                var context = pool.Pop();
                context.pInt = pInt;
                return context;
            }
            else
            {
                return new WEventContext(){pInt = pInt};
            }
        }
        public static WEventContext Get(string pString)
        {
            if (pool.Count > 0)
            {
                var context = pool.Pop();
                context.pString = pString;
                return context;
            }
            else
            {
                return new WEventContext(){pString = pString};
            }
        }

        public static void Push(WEventContext value)
        {
            pool.Push(value);
        }

        public int pInt;
        public string pString;
    }

    public static class EventCenter
    {
        class EventBridge
        {
            private WEventCallback0 _callback0;
            private WEventCallback1 _callback1;

            public EventBridge()
            {

            }

            public void Add(WEventCallback0 callback)
            {
                _callback0 -= callback;
                _callback0 += callback;
            }

            public void Add(WEventCallback1 callback)
            {
                _callback1 -= callback;
                _callback1 += callback;
            }

            public void Remove(WEventCallback0 callback)
            {
                _callback0 -= callback;
            }

            public void Remove(WEventCallback1 callback)
            {
                _callback1 -= callback;
            }

            public void Clear()
            {
                _callback0 = null;
                _callback1 = null; 
            }

            // public void CallInternal(WEventContext context)
            // {
            //     _callback1?.Invoke(context);
            //     _callback0?.Invoke();
            // }
            
            public void CallInternal(TAny ctx)
            {
                _callback1?.Invoke(ctx);
                _callback0?.Invoke();
            }

            public void CallInternal()
            {
                _callback1?.Invoke(null);
                _callback0?.Invoke();
            }
        }

        private static Dictionary<int, EventBridge> eventList = new(67);

        public static void AddListener(int type, WEventCallback0 callback)
        {
            if (!eventList.TryGetValue(type, out var bridge))
            {
                bridge = new EventBridge();
                eventList.Add(type, bridge);
            }

            bridge.Add(callback);
        }

        public static void AddListener(int type, WEventCallback1 callback)
        {
            if (!eventList.TryGetValue(type, out var bridge))
            {
                bridge = new EventBridge();
                eventList.Add(type, bridge);
            }

            bridge.Add(callback);
        }

        public static void RemoveListener(int type, WEventCallback0 callback)
        {
            if (!eventList.TryGetValue(type, out var bridge))
            {
                return;
            }

            bridge.Remove(callback);
        }

        public static void RemoveListener(int type, WEventCallback1 callback)
        {
            if (!eventList.TryGetValue(type, out var bridge))
            {
                return;
            }

            bridge.Remove(callback);
        }

        public static void Trigger(int type, string ctx)
        {
            if (eventList.TryGetValue(type, out var bridge))
            {
                UnityEngine.Profiling.Profiler.BeginSample("TriggerEvent");
                var t = new TAnyString(ctx);
                bridge.CallInternal(t);
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }
        public static void Trigger(int type, int ctx)
        {
            if (eventList.TryGetValue(type, out var bridge))
            {
                UnityEngine.Profiling.Profiler.BeginSample("TriggerEvent");
                var t = new TAnyInt(ctx);
                bridge.CallInternal(t);
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        public static void Trigger(int type, ulong ctx)
        {
            if (eventList.TryGetValue(type, out var bridge))
            {
                UnityEngine.Profiling.Profiler.BeginSample("TriggerEvent");
                bridge.CallInternal(new TAnyUnsignLong(ctx));
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }
        public static void Trigger(int type)
        {
            if (eventList.TryGetValue(type, out var bridge))
            {
                UnityEngine.Profiling.Profiler.BeginSample("TriggerEvent");
                bridge.CallInternal();
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        // public static void Trigger(int type, WEventContext context)
        // {
        //     if (eventList.TryGetValue(type, out var bridge))
        //     {
        //         bridge.CallInternal(context);
        //     }
        //     WEventContext.Push(context);
        // }
    }
}