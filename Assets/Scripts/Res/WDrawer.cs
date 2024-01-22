using System;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
using UnityTimer;
using WGame.Runtime;

public class WDrawer : SingletonMono<WDrawer>
{
    private static Stack<Transform> _poolStack = new Stack<Transform>();

    private static int _poolHead = 0;

    private Transform _transform;
    public Transform Trans => _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private static Transform GetDrawer(Transform parent, Vector3 position, Quaternion rotation, Vector3 scale)
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

        obj.localRotation = rotation;
        obj.position = position;
        obj.localScale = scale;
        obj.parent = parent;
        return obj;
    }
    private static Transform GetDrawer(Transform parent)
    {
        return GetDrawer(parent, parent.position, parent.rotation, Vector3.zero);
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
    private static CircleDrawInfo GetCircleInfo(Transform drawer)
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

    private readonly Color defaulColor = Color.cyan * 0.4f;
    
    public void RegisterCircle(Vector3 center, Vector3 forward, float radius, float autoReleaseTime = 2f)
    {
        RegisterCircle(center, forward, radius, defaulColor, autoReleaseTime);
    }
    
    public void RegisterCircle(Vector3 center, Vector3 forward, float radius, Color color, float autoReleaseTime = 4f)
    {
        var info = new CircleInfo()
        {
            center = center,
            forward = forward,
            radius = radius,
            fillColor = color,
        };
        var circle = RegisterCircle(info);
        Timer.Register(autoReleaseTime, () =>
        {
            CancelCircle(circle);
        });
    }
    public CircleDrawInfo RegisterCircle(CircleInfo info)
    {
        return RegisterCircle(_transform, info);
    }
    
    public CircleDrawInfo RegisterCircle(Transform parent, Vector3 position, Quaternion rotation)
    {
        return RegisterCircle(parent, position, rotation, Vector3.one);
    }
    
    public CircleDrawInfo RegisterCircle(Transform parent, Vector3 position)
    {
        return RegisterCircle(parent, position, parent.rotation, Vector3.one);
    }
    
    public CircleDrawInfo RegisterCircle(Transform parent, Vector3 position, Quaternion rotate, Vector3 scale)
    {
        var trans = GetDrawer(parent, position, rotate, scale);
        var drawInfo = GetCircleInfo(trans);
        drawerInfoList.Add(drawInfo);
        return drawInfo;
    }
    
    public CircleDrawInfo RegisterCircle(Transform parent, CircleInfo info)
    {
        var trans = GetDrawer(parent, info.center, Quaternion.identity, Vector3.one);
        var drawInfo = GetCircleInfo(trans);
        drawInfo.Info = info;
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
        var drawInfo = GetCircleInfo(GetDrawer(parent));
        drawInfo.Info = info;
        drawerInfoList.Add(drawInfo);
        return drawInfo;
    }

    public void CancelCircle(CircleDrawInfo drawInfo)
    {
        drawerInfoList.Remove(drawInfo);
        PushCircleInfo(drawInfo, _transform);
    }

    public void RecycleDrawObj(Transform filter)
    {
        drawers.Remove(filter);
        Push(filter, _transform);
    }

    private void LateUpdate()
    {
        for (int i = 0; i < drawerInfoList.Count; i++)
        {
            if(drawerInfoList[i].IsActive)
                Circle.Draw(drawerInfoList[i].Info);
        }
    }

    public void DrawSphere(float radius, Color color, float lifeTime = 2f)
    {
        var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.transform.localScale = new Vector3(radius, radius, radius);
    }
}
