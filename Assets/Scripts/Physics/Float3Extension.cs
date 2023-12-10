using UnityEngine;
using Unity.Mathematics;

public static class Float3Extension
{
    public static Vector3 ToVector3(this float3 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }
}
