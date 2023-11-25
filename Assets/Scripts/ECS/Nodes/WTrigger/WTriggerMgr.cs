using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WGame.Runtime;

namespace WGame.Trigger
{
    public partial class WTriggerMgr : Singleton<WTriggerMgr>
    {
        private List<WTrigger> waitDeletedTrigger = new List<WTrigger>();
        private Dictionary<int, Dictionary<int, WTrigger>> triggerList = new Dictionary<int, Dictionary<int, WTrigger>>();
        private int[] _triggerIndexes;
        public WTriggerMgr()
        {
            _triggerIndexes = new int[256];
        }
        public int GenTriggerIndex(int type)
        {
            _triggerIndexes[type]++;
            return _triggerIndexes[type];
        }

        public void TriggerEvent(WtEventType type)
        {
            if (TryGetTriggers(type, out var triggers))
            {
                foreach (var kv in triggers)
                {
                    kv.Value.TryTrigger();
                }
            }
        }

        private bool TryGetTriggers(int mainType, int subType, int eventType, out Dictionary<int, WTrigger> triggers)
        {
            if (triggerList.TryGetValue(GetTypeID(mainType, subType, eventType), out triggers))
            {
                return true;
            }

            return false;
        }
        private bool TryGetTriggers(WtEventType type, out Dictionary<int, WTrigger> triggers)
        {
            if (triggerList.TryGetValue(GetTypeID(type), out triggers))
            {
                return true;
            }

            return false;
        }
        
        public void TriggerEvent(WtEventType type, WTrigger.Context data)
        {
            if (TryGetTriggers(type, out var triggers))
            {
                foreach (var kv in triggers)
                {
                    kv.Value.TryTrigger(data);
                }
            }
        }
        
        public void TriggerEvent(int mainType, int subType, int eventType, WTrigger.Context context)
        {
            if (TryGetTriggers(mainType, subType, eventType, out var triggers))
            {
                foreach (var kv in triggers)
                {
                    kv.Value.TryTrigger(context);
                }
            }
        }

        public void TriggerEvent(int mainType, int subType, int eventType)
        {
            if (TryGetTriggers(mainType, subType, eventType, out var triggers))
            {
                foreach (var kv in triggers)
                {
                    kv.Value.TryTrigger();
                }
            }
        }
        
        public void Register(WTrigger trigger)
        {
            var type = trigger.eventType;
            if (!TryGetTriggers(type, out var triggers))
            {
                triggers = new Dictionary<int, WTrigger>()
                {
                    {trigger.triggerIndex, trigger}
                };
                triggerList[GetTypeID(type)] = triggers;
            }

            triggers[trigger.triggerIndex] = trigger;
        }

        public void Cancel(WTrigger trigger)
        {
            trigger.IsActive = false;
            waitDeletedTrigger.Add(trigger);
        }

        private int GetTypeID(int mainType, int subType, int eventType)
        {
            return (mainType << 20) | (subType << 10) | (eventType);
        }
        private int GetTypeID(WtEventType type)
        {
            return (type.mainType << 20) | (type.subType << 10) | (type.eventType);
        }

        private void Delete(WTrigger trigger)
        {
            var type = trigger.eventType;
            if (!TryGetTriggers(type, out var triggers))
            {
                return;
            }

            triggers.Remove(trigger.triggerIndex);
            WTrigger.Push(trigger);
        }

        public void OnEndUpdate()
        {
            for (int i = 0; i < waitDeletedTrigger.Count; i++)
            {
                Delete(waitDeletedTrigger[i]);
            }
            waitDeletedTrigger.Clear();
        }

        public void OnDispose()
        {
            
        }
    }
}