using System;
using System.Collections.Generic;
using Oddworm.Framework;
using UnityEngine;

public enum TriggerObjType
{
    Sphere,
}

public abstract class TriggerObject
{
    private TriggerObjType Type;
    public string TypeName { get; }

    public Vector3 Position { get; set; }

    public int TargetLayer { get; private set; }

    public bool IsEnable { get; private set; }
    
    public SensorEntity Owner { get; private set; }

    private const int MAX_TARGET_NUM = 16;
    private Collider[] _colliders = new Collider[MAX_TARGET_NUM];
    private Dictionary<int, int> _hittedTargetAndPart = new();
    public Dictionary<int, int> HittedTargetAndPart => _hittedTargetAndPart;

    private List<int> _targets = new();
    private HashSet<int> _targetSet = new();
    private int _detectTargetNum;
    private short[] _parts = new short[MAX_TARGET_NUM];

    private TriggerObject(TriggerObjType type)
    {
        Type = type;
        TypeName = type.ToString();
    }

    public static Sphere GenSphere(SensorEntity sensor, Vector3 pos, float radius)
    {
        var obj = Sphere.Get(pos, radius);
        obj.Init(sensor);
        return obj;
    }

    private void Init(SensorEntity sensor)
    {
        IsEnable = true;
        Owner = sensor;
        _targetSet.Clear();
        TargetLayer = 1 << EntityUtils.GetTargetSensorLayer(sensor.linkCharacter.Character);
    }

    private static void PushInternal(TriggerObject obj)
    {
        switch (obj.Type)
        {
            case TriggerObjType.Sphere:
                Sphere.Push(obj as Sphere);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Dispose()
    {
        Owner = null;
        IsEnable = false;
        PushInternal(this);
    }

    protected abstract int Detect();

    public int ProcessDetect()
    {
        _targets.Clear();
        _detectTargetNum = 0;
        var numCols = Detect();
        for (int i = 0; i < numCols; i++)
        {
            if (EntityUtils.TryGetEntitySensorMono(_colliders[i].GetInstanceID(), out var mono))
            {
                var id = mono.EntityId;
                if (_targetSet.Contains(id))
                {
                    continue;
                }

                var part = (short)mono.PartType;
                bool isNewTarget = true;
                int targetIdx = 0;
                for (int j = 0; j < _detectTargetNum; j++)
                {
                    // 如果已经存在目标的其他部位
                    if (_targets[j] == id)
                    {
                        _parts[j] &= part;
                        isNewTarget = false;
                        targetIdx = j;
                        break;
                    }
                }

                if (isNewTarget)
                {
                    _targets.Add(id);
                    _parts[_detectTargetNum] = part;
                    _detectTargetNum++;
                }
            }
        }

        for (int i = 0; i < _detectTargetNum; i++)
        {
            _targetSet.Add(_targets[i]);
        }

        return _detectTargetNum;
    }

    public abstract void Draw();

    public int GetTarget(int idx) => _targets[idx];
    public int GetPart(int idx) => _parts[idx];

    public class Sphere : TriggerObject
    {
        public static Stack<Sphere> _pool = new Stack<Sphere>(8);

        public static Sphere Get(Vector3 position, float radius)
        {
            if (_pool.Count > 0)
            {
                var obj = _pool.Pop();
                obj.Position = position;
                obj.Radius = radius;
                return obj;
            }

            var newObj = new Sphere();
            newObj.Position = position;
            newObj.Radius = radius;
            return newObj;
        }

        public static void Push(Sphere obj)
        {
            _pool.Push(obj);
        }

        private Sphere() : base(TriggerObjType.Sphere)
        {
        }

        public float Radius { get; set; }

        protected override int Detect()
        {
            return Physics.OverlapSphereNonAlloc(Position, Radius, _colliders, TargetLayer);
        }

        public override void Draw()
        {
            if (IsEnable)
            {
                DbgDraw.WireSphere(Position, Quaternion.identity, new Vector3(Radius, Radius, Radius), Color.white, 0.2f);
            }
        }
    }
}
