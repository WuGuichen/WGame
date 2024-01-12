using Entitas;
using Entitas.Unity;
using TWY.Physics;
using Unity.Mathematics;
using UnityEngine;

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
    public void OnDead()
    {
        GetEntity().isMoveable = false;
        gameObject.Unlink();
    }

    public void OnAlive()
    {
        GetEntity().isMoveable = true;
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

    public void Thrust()
    {
        var entity = GetEntity();
        thrustPos = entity.actionThrust.targetPositions;
        thrustTime = entity.actionThrust.durations;
        thrustType = entity.actionThrust.types;
        curThrustIndex = -1;
        NextThrust();
    }

    public void OnUpdateMove(float deltaTime)
    {
        if (curThrustIndex >= thrustTime.Length)
            return;
        if (thrustTime[curThrustIndex] > 0)
        {
            var delta = deltaTime;
            if (deltaTime > thrustTime[curThrustIndex])
                delta = thrustTime[curThrustIndex];
            thrustTime[curThrustIndex] -= deltaTime;
            _transform.position += curDeltaTarThrustPos * delta;
        }
        else
        {
            NextThrust();
        }
    }

    public Transform RightHand => null;

    private void NextThrust()
    {
        curThrustIndex++;
        if (curThrustIndex >= thrustPos.Length)
        {
            if (GetEntity().hasActionThrust)
            {
                GetEntity().RemoveActionThrust();
            }
        }
        else
        {
            thrustPos[curThrustIndex].y = 0;
            if(thrustTime[curThrustIndex] > 0)
                curDeltaTarThrustPos = thrustPos[curThrustIndex] / thrustTime[curThrustIndex];
            else
                WLogger.Error("数据有误");
        }
    }

    public int InstanceID => _entity.instanceID.ID;

    public Vector3 GetCameraPos()
    {
        return _cameraTransform.position;
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
