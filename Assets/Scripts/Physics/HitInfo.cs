public struct HitInfo
{
    public int EntityId { get; set; }
    public float SqrDist { get; set; }

    public HitInfo(int id, float sqrDist)
    {
        EntityId = id;
        SqrDist = sqrDist;
    }
}
