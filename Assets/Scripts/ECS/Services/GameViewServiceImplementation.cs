using Entitas;
using Entitas.Unity;
using TWY.Physics;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(IRigidbodyService))]
[RequireComponent(typeof(Pathfinding.Seeker))]
[RequireComponent(typeof(Pathfinding.RaycastModifier))]
[RequireComponent(typeof(WCharacterInfo))]
public class GameViewServiceImplementation :MonoBehaviour, IGameViewService, IEventListener, IActionCameraRotateListener, IActionModelRotateListener
{
    private Transform _cameraTransform;
    private Transform _model;
    private GameEntity _entity;
    private Transform _transform;
    private Vector3[] thrustPos;
    private float[] thrustTime;
    private int[] thrustType;
    private int curThrustIndex = 0;
    private Vector3 curDeltaTarThrustPos;
    [SerializeField]
    private Transform headTrans;
    
    private float height;
    private float halfHeight;
    private float radius;
    private float radiusTwo;

    private ITimeService _timeService;
    public void OnDead()
    {
        GetEntity().isMoveable = false;
        gameObject.Unlink();
    }

    public float Height => height;
    public float HalfHeight => halfHeight;

    public Vector3 HeadPos
    {
        get
        {
            var pos = headTrans.position;
            return new Vector3(pos.x, pos.y + 0.4f, pos.z);
        }
    }

    public void BeFocused(bool value)
    {
        var entity = GetEntity();
        if (entity.hasUIHeadPad)
        {
            if (value)
                entity.uIHeadPad.service.Show();
            else
                entity.uIHeadPad.service.Hide(2f);
        }
    }

    public int InstanceID => _entity.instanceID.ID;

    public Vector3 GetCameraPos()
    {
        return _cameraTransform.position;
    }

    public void SetMoveVelocity(Vector3 dir)
    {
        _entity.rigidbodyService.service.Velocity += (dir * _timeService.DivFixedDeltaTime);
    }

    public Vector3 LocalizeVectorXY(Vector2 vector, bool isFocus)
    {
        if (isFocus)
        {
            return _model.right * vector.x + _model.forward * vector.y;
        }
        else
        {
            return _cameraTransform.right * vector.x + _cameraTransform.forward * vector.y;
        }
    }

    public Transform Model => _model;
    public Vector3 Position => _transform.position;
    public Vector3 FocusPoint => _focusPoint.Position;
    public Vector2 PlanarPosition => new Vector2(Position.x, Position.z);

    private WEntityPoint _focusPoint;
    
    public IGameViewService OnInit(GameEntity entity)
    {
        _timeService = Contexts.sharedInstance.meta.timeService.instance;
        _transform = transform;
        _entity = entity;
        if (_cameraTransform != null)
            return this;
        _cameraTransform = _transform.GetChild(0);
        _model = _transform.GetChild(1);
        var focusTrans = _transform.GetChild(2);
        _focusPoint = new WEntityPoint(focusTrans, entity);
        var capsule = _transform.GetComponent<CharacterController>();
        if (capsule)
        {
            height = capsule.height;
            halfHeight = height / 2;
            radius = capsule.radius;
            radiusTwo = radius *2;
        }
        return this;
    }

    public void RegisterEventListener(Contexts contexts, IEntity entity)
    {
        var e = entity as GameEntity;
        e.AddActionCameraRotateListener(this);
        e.AddActionModelRotateListener(this);
    }

    public void OnActionCameraRotate(GameEntity entity, Quaternion rot)
    {
        _cameraTransform.rotation = rot;
    }

    public void OnActionModelRotate(GameEntity entity, Quaternion rot)
    {
        _model.localRotation = rot;
    }
    
    public GameEntity GetEntity()
    {
        // if (_entity == null)
        //     _entity = Model.GetComponent<IMotionService>().LinkEntity;
        return _entity;
    }

    public void Destroy()
    {
        gameObject.Unlink();
        var sensorMonos = Model.GetComponentsInChildren<SensorMono>();
        for (var i = 0; i < sensorMonos.Length; i++)
        {
            sensorMonos[i].Dispose();
        }
        DestroyImmediate(this.gameObject);
    }

    public void OnDeadState(GameEntity entity)
    {
        if (entity.isDeadState)
        {
            WLogger.Info("Dead");
        }
        else
        {
            WLogger.Info("Alive");
        }
    }

    public override int GetHashCode()
    {
        return InstanceID;
    }

    public AABBF Bounds {
        get
        {
            var pos = _transform.position;
            return new AABBF(new float3(pos.x, pos.y + halfHeight, pos.z), Size);
        }
    }

    public float Radius => radiusTwo;
    public float3 Size => new float3(radiusTwo, height, radiusTwo);
}
