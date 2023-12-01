using System.Collections.Generic;
using Shapes;
using UnityEngine;
using WGame.Runtime;

public class WDrawer : SingletonMono<WDrawer>
{
    private static Stack<Transform> _poolStack = new Stack<Transform>();

    private static int _poolHead = 0;
    
    private static Transform GetDrawer(Transform parent)
    {
        Transform obj;
        if (_poolStack.Count > 0)
        {
            obj = _poolStack.Pop();
            obj.gameObject.SetActive(true);
        }
        else
        {
            var go = new GameObject("WDrawerObj");
            obj = go.transform;
        }
        obj.parent = parent;
        return obj;
    }

    private static void Push(Transform obj, Transform parent)
    {
        obj.gameObject.SetActive(false);
        obj.parent = parent;
        _poolStack.Push(obj);
    }

    private static List<CircleDrawInfo> circleDrawInfos = new List<CircleDrawInfo>();
    private static Stack<int> emptyCircleInfoId = new Stack<int>();

    public static CircleDrawInfo GetCircleInfo(int id)
    {
        if (id < circleDrawInfos.Count)
            return circleDrawInfos[id];
        return null;
    }
    private static CircleDrawInfo GetCircleInfo(Transform drawer, CircleInfo info)
    {
        if (emptyCircleInfoId.Count > 0)
        {
            return circleDrawInfos[emptyCircleInfoId.Pop()];
        }
        var res = new CircleDrawInfo()
        {
            IsActive = true,
            ID = circleDrawInfos.Count,
            Drawer = drawer,
            Info = info,
        };
        circleDrawInfos.Add(res);
        return res;
    }

    private static void PushCircleInfo(CircleDrawInfo drawInfo, Transform parent)
    {
        Push(drawInfo.Drawer, parent);
        emptyCircleInfoId.Push(drawInfo.ID);
    }

    public class CircleDrawInfo
    {
        public bool IsActive { get; set; }
        public int ID { get; set; }
        public Transform Drawer { get; set; }
        public CircleInfo Info { get; set; }
    }

    private List<Transform> drawers = new List<Transform>();
    private List<CircleDrawInfo> drawerInfoList = new List<CircleDrawInfo>();

    public CircleDrawInfo RegisterCircle(Transform parent, CircleInfo info)
    {
        var drawInfo = GetCircleInfo(GetDrawer(parent), info);
        drawerInfoList.Add(drawInfo);
        return drawInfo;
    }
    
    public CircleDrawInfo RegisterCircle(Transform parent, float radius = 3f)
    {
        var info = new CircleInfo()
        {
            center = parent.position,
            forward = parent.forward,
            radius = radius,
            fillColor = Color.cyan,
        };
        var drawInfo = GetCircleInfo(GetDrawer(parent), info);
        drawerInfoList.Add(drawInfo);
        return drawInfo;
    }

    public void CancelCircle(CircleDrawInfo drawInfo)
    {
        drawerInfoList.Remove(drawInfo);
        PushCircleInfo(drawInfo, transform);
    }

    public void RecycleDrawObj(Transform filter)
    {
        drawers.Remove(filter);
        Push(filter, transform);
    }

    private void LateUpdate()
    {
        for (int i = 0; i < drawerInfoList.Count; i++)
        {
            if(drawerInfoList[i].IsActive)
                Circle.Draw(drawerInfoList[i].Info);
        }
    }
}
