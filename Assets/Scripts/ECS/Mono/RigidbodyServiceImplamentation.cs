using System;
using Entitas;
using Entitas.Unity;
using UnityEngine;
using UnityTimer;
using Vector3 = UnityEngine.Vector3;

public class RigidbodyServiceImplamentation : MonoBehaviour, IRigidbodyService, IActionThrustUpListener, IEventListener
{
    private Rigidbody _rigidbody;
    private Vector3 targetVelocity;
    private Timer slerpTimer;
    private CapsuleCollider _rigidCollider;
    private GameEntity entity;

    public Rigidbody Rigidbody => _rigidbody;

    public IRigidbodyService OnInit()
    {
        if (_rigidbody != null)
            return this;
        _rigidbody = GetComponent<Rigidbody>();
        _rigidCollider = GetComponent<CapsuleCollider>();
        return this;
    }

    public Vector3 GetSensorBoundsCenter()
    {
        return _rigidCollider.center + transform.position;
    }

    public Vector3 Velocity
    {
        get => _rigidbody.velocity;
        set => _rigidbody.velocity = value;
    }

    public void MovePosition(Vector3 dir)
    {
        throw new NotImplementedException();
    }

    public void OnFixedUpdate(float deltaTime)
    {
    }

    public void SetEntity(GameEntity entity)
    {
        this.entity = entity;
    }

    public void OnActionThrustUp(GameEntity entity, float value)
    {
        var vec = _rigidbody.velocity;
        vec.y = value;
        _rigidbody.velocity = vec;
        this.entity.RemoveActionThrustUp();
    }

    public void RegisterEventListener(Contexts contexts, IEntity entity)
    {
        this.entity = entity as GameEntity;
        this.entity.AddActionThrustUpListener(this);
    }
}
