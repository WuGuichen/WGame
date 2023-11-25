using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;
using WGame.Runtime;

namespace WGame.Res
{
    public class EffectMgr : Singleton<EffectMgr>
    {
        private static object locker = new object();

        private Dictionary<int, GameObject> loadedEffects = new Dictionary<int, GameObject>();

        public static void LoadEffect(int objId, Transform parent, Vector3 pos, Quaternion dir,
            float duration = -1f, Action<GameObject> onCompleted = null)
        {
            // Blood_Arterial_Medium_RedBright
            var data = GameData.Tables.TbObjectData[objId];
            string path = data.Path;
            if (data.NeedCache)
            {
                ObjectPool.Inst.GetObject(data.Id, pos, dir, parent, obj =>
                {
                    onCompleted?.Invoke(obj);
                    int key = obj.GetInstanceID();
                    if(Inst.loadedEffects.ContainsKey(key))
                        WLogger.Warning("重复加载特效");
                    Inst.loadedEffects[key] = obj;
                    DisposeEffect(objId, key, duration);
                });
            }
            else
            {
                YooassetManager.Inst.LoadGameObject(path, obj =>
                {
                    onCompleted?.Invoke(obj);
                    obj.transform.position = pos;
                    obj.transform.rotation = dir;
                    int key = obj.GetInstanceID();
                    if(Inst.loadedEffects.ContainsKey(key))
                        WLogger.Warning("重复加载特效");
                    Inst.loadedEffects[key] = obj;
                    DisposeEffect(objId, key, duration);
                });
            }
        }

        public static void DisposeEffect(int objId ,int key, float delayTime = 0)
        {
            var data = GameData.Tables.TbObjectData[objId];
            if (Inst.loadedEffects.TryGetValue(key, out var obj))
            {
                if (delayTime > 0)
                {
                    Timer.Register(delayTime, () =>
                    {
                        if(data.NeedCache)
                            ObjectPool.Inst.PushObject(data.Id, obj);
                        else
                            GameObject.Destroy(obj);
                        Inst.loadedEffects.Remove(key);
                    });
                }
                else
                {
                    if(data.NeedCache)
                        ObjectPool.Inst.PushObject(data.Id, obj);
                    else
                        GameObject.Destroy(obj);
                    Inst.loadedEffects.Remove(key);
                }
            }

        }
    }
}