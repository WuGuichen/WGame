using UnityEngine;

public struct HitInfo
{
    public int EntityId { get; private set; }
    public float SqrDist { get; private set; }
    public Vector3 Position { get; private set; }

    // public HitInfo(int id, float sqrDist)
    // {
    //     EntityId = id;
    //     SqrDist = sqrDist;
    // }
    
    public HitInfo(int id, float sqrDist, Vector3 position)
    {
        EntityId = id;
        SqrDist = sqrDist;
        Position = position;
    }
    //
    // private static float3 DEFAULT = new float3(1, 0, 0);
}
