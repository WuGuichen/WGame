using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class Vector3Extension
{
    public static float3 ToFloat3(this Vector3 v)
    {
        return new float3(v.x, v.y, v.z);
    }
}
