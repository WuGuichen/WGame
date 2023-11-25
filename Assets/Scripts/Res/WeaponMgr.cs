using System;
using System.Collections.Generic;
using UnityEngine;
using WGame.Runtime;

namespace WGame.Res
{
    public class WeaponMgr : Singleton<WeaponMgr>
    {
        private Dictionary<int, int> mapWeapon = new Dictionary<int, int>();
        public void GetWeaponObj(int objId, Action<GameObject> callback, Transform parent = null)
        {
            ObjectPool.Inst.GetObject(objId, parent, (o) =>
            {
                int instId = o.GetInstanceID();
                if (mapWeapon.ContainsKey(instId))
                {
                    WLogger.Warning("重复的添加统一物体");
                }
                else
                {
                    mapWeapon[instId] = objId;
                }
                callback?.Invoke(o);
            });
        }

        public void PushWeaponObj(GameObject obj)
        {
            if (mapWeapon.TryGetValue(obj.GetInstanceID(), out var objId))
            {
                ObjectPool.Inst.PushObject(objId, obj);
                return;
            }
            WLogger.Error("没有加载对应的武器");
        }
    }
}