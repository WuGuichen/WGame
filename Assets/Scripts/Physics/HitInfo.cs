using Unity.Mathematics;
using UnityEngine;

public struct HitInfo
{
    public int EntityId { get; private set; }
    public float SqrDist { get; private set; }
    private float dist;

    public float Dist
    {
        get
        {
            if (dist == SqrDist)
                dist = math.sqrt(SqrDist);
            return dist;
        }
    }
    public Vector3 Position { get; private set; }

    public HitInfo(int id, float sqrDist, Vector3 position)
    {
        EntityId = id;
        SqrDist = sqrDist;
        dist = sqrDist;
        Position = position;
    }

    public HitInfo(int id, Vector3 position, float dist, float sqrDist)
    {
        EntityId = id;
        SqrDist = sqrDist;
        this.dist = dist;
        Position = position;
    }
    public HitInfo(int id, Vector3 position, float dist)
    {
        EntityId = id;
        SqrDist = dist * dist;
        this.dist = dist;
        Position = position;
    }
}
