using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WGame.Trigger
{
    public static class WTriggerExtension
    {
        public static void Register(this WTrigger trigger)
        {
            WTriggerMgr.Inst.Register(trigger);
        }

        public static void Cancel(this WTrigger trigger)
        {
            WTriggerMgr.Inst.Cancel(trigger);
        }
    }
}