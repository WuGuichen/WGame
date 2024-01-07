using Unity.Mathematics;
using UnityEngine;

public struct HitInfo
{
    public int EntityId { get; private set; }
    public float SqrDist { get; private set; }
    private float dist;
    private bool isInitDist;

    public float Dist
    {
        get
        {
            if (isInitDist)
            {
                return dist;
            }
            dist = math.sqrt(SqrDist);
            isInitDist = true;
            return dist;
        }
    }
    public Vector3 Position { get; private set; }

    public HitInfo(int id, float sqrDist, Vector3 position)
    {
        EntityId = id;
        SqrDist = sqrDist;
        dist = sqrDist;
        isInitDist = false;
        Position = position;
    }

    public HitInfo(int id, Vector3 position, float dist, float sqrDist)
    {
        EntityId = id;
        SqrDist = sqrDist;
        this.dist = dist;
        isInitDist = true;
        Position = position;
    }
    public HitInfo(int id, Vector3 position, float dist)
    {
        EntityId = id;
        SqrDist = dist * dist;
        this.dist = dist;
        isInitDist = true;
        Position = position;
    }

    public static HitInfo EMPTY = new HitInfo(-1, -1, Vector3.zero);
    public bool IsEmpty => EntityId < 0;
    public bool IsNotEmpty => EntityId >= 0;
}
