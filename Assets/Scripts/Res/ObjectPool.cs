using System;
using System.Collections.Generic;
using UnityEngine;
using WGame.Res;
using WGame.Runtime;

public class ObjectPool : SingletonMono<ObjectPool>
{
    class Pool
    {
        private Transform root;
        public int count { get; private set; }

        public Pool(string name, Transform parent)
        {
            root = new GameObject(name).transform;
            root.parent = parent;
            count = 0;
        }

        public bool Get(Transform parent, out GameObject obj)
        {
            return Get(parent, Vector3.zero, Quaternion.identity, out obj);
        }

        public bool Get(Transform parent, Vector3 pos, Quaternion rot, out GameObject obj)
        {
            if (count > 0)
            {
                count--;
                var trans = root.GetChild(count);
                trans.parent = parent;
                trans.position = pos;
                trans.rotation = rot;
                obj = trans.gameObject;
                obj.SetActive(true);
                return true;
            }

            obj = null;
            return false;
        }

        public void Push(GameObject obj)
        {
            if (obj.transform.parent.name != root.name)
            {
                obj.SetActive(false);
                obj.transform.parent = root;
                count++;
            }
        }
    }
    private Dictionary<int, Pool> objPools;

    private void Awake()
    {
        objPools = new Dictionary<int, Pool>();
    }

    public void PreLoad(int objId, int num)
    {
        var data = GameData.Tables.TbObjectData[objId];
        while (num > 0)
        {
            YooassetManager.Inst.LoadGameObject(data.Path, o =>
            {
                PushObject(data.Id, o);
            });
            num--;
        }
    }

    public void GetObject(int objId, Transform parent = null,
        Action<GameObject> callback = null)
    {
        GetObject(objId, Vector3.zero, Quaternion.identity, parent, callback);
    }

    public void GetObject(int objId, Vector3 pos, Quaternion rot, Transform parent = null, Action<GameObject> callback = null)
    {
        if (parent == null) 
            parent = transform;
        var data = GameData.Tables.TbObjectData[objId];
        if (objPools.TryGetValue(objId, out var pool))
        {
            if (pool.Get(parent, pos, rot, out var obj))
            {
                callback?.Invoke(obj);
                return;
            }
        }

        YooassetManager.Inst.LoadGameObject(data.Path, o =>
        {
            var trans = o.transform;
            trans.parent = parent;
            trans.position = pos;
            trans.rotation = rot;
            callback?.Invoke(o);
        });
    }

    public void PushObject(int objId, GameObject obj)
    {
        if (!objPools.TryGetValue(objId, out var root))
        {
            root = new Pool(objId.ToString(), transform);
            objPools[objId] = root;
        }
        root.Push(obj); 
    }
}
