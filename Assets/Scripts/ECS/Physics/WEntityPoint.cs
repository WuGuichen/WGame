using TWY.Physics;

public struct WEntityPoint
{
    private UnityEngine.Transform _transform;
    private readonly GameEntity _entity;

    public WEntityPoint(UnityEngine.Transform transform, GameEntity entity)
    {
        _transform = transform;
        _entity = entity;
    }

    public GameEntity Entity => _entity;

    public bool Intersect(SphereF sphere) => (sphere.Center.ToVector3() - _transform.position).sqrMagnitude <= sphere.SqrRadius;

    public bool Intersect(AABBF box) => box.Contains(_transform.position.ToFloat3());

    public bool Intersect(CapsuleF capsuleF) => capsuleF.IntersectPoint(_transform.position.ToFloat3());
}