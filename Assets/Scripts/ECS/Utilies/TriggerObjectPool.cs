using UnityEngine;

public class TriggerObjectPool
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
}
