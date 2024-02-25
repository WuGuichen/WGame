using UnityEngine;

public class SensorMono : MonoBehaviour
{
    private GameEntity _entity;
    public GameEntity Entity => _entity;

    [SerializeField] private EntityPartType _partType;
    public EntityPartType PartType => _partType;

    [SerializeField] private Collider _collider;
    public Collider Collider => _collider;

    private int _colliderId;

    public int ColliderId
    {
        get => _colliderId; 
    }

    public void SetData(GameEntity entity, EntityPartType type, Collider collider)
    {
        if (entity == null)
        {
            throw WLogger.ThrowError("entity数据错误");
        }

        if (_collider == null)
        {
            throw WLogger.ThrowError("collider数据错误");
        }
        _entity = entity;
        _partType = type;
        _colliderId = collider.GetInstanceID();
        EntityUtils.RegisterCollider(this, entity);
    }
    public void SetData(GameEntity entity)
    {
        SetData(entity, _partType, _collider);
    }

    public void Dispose()
    {
        EntityUtils.CancelCollider(this);
        _entity = null;
        _colliderId = 0;
        _collider = null;
    }
}
