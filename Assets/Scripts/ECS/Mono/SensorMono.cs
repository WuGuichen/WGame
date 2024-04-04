using Oddworm.Framework;
using UnityEngine;

public class SensorMono : MonoBehaviour
{
    private GameEntity _entity;
    public GameEntity Entity => _entity;
    public int EntityId { get; private set; }

    [SerializeField] private EntityPartType _partType;
    [SerializeField] private PartMaterialType _matrialType = PartMaterialType.Bone;
    public EntityPartType PartType => _partType;

    [SerializeField] private Collider _collider;
    public Collider Collider => _collider;

    private int _colliderId;

    public int ColliderId
    {
        get => _colliderId; 
    }

    private Transform _transform;
    public Transform Trans => _transform;

    public SensorMono SetLayer(int layer)
    {
        gameObject.layer = layer;
        return this;
    }

    public SensorMono SetSize(float radius, float height)
    {
        SetSize(radius, height, Vector3.zero);
        return this;
    }
    
    public SensorMono SetSize(float radius, float height, Vector3 center)
    {
        var capsule = _collider as CapsuleCollider;
        if (capsule != null)
        {
            capsule.height = height;
            capsule.radius = radius;
            capsule.center = center;
        }

        return this;
    }

    public SensorMono SetData(GameEntity entity, EntityPartType type, Collider collider)
    {
        if (entity == null)
        {
            throw WLogger.ThrowError("entity数据错误");
        }

        if (collider == null)
        {
            throw WLogger.ThrowError("collider数据错误");
        }
        _collider = collider;
        _collider.isTrigger = true;
        _entity = entity;
        EntityId = entity.instanceID.ID;
        _partType = type;
        _colliderId = collider.GetInstanceID();
        _transform = transform;
        EntityUtils.RegisterCollider(this, entity);
        return this;
    }
    public SensorMono SetData(GameEntity entity)
    {
        return SetData(entity, _partType, _collider);
    }

    public void RefreshPosition()
    {
        var pos = _entity.position.value;
        pos.y += _entity.gameViewService.service.HalfHeight;
        DbgDraw.WireCapsule(pos, Quaternion.identity, _entity.gameViewService.service.Radius, _entity.gameViewService.service.Height, Color.red, 1f);
        _transform.position = pos;
    }

    public void Dispose()
    {
        EntityUtils.CancelCollider(this);
        _entity = null;
        _colliderId = 0;
        _collider = null;
    }

    public static bool TryGetHitEffect(int weaponID, int impulse, out string res)
    {
        if (weaponID == 1)
        {
            // 长剑
            res = "HCFX_Hit_01";
            return true;
        }
        else if(weaponID == 2)
        {
            // 拳头
            res = "HCFX_Hit_03";
            return true;
        }

        res = null;
        return false;
    }
}
