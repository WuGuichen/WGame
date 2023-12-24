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

    public static float GetAngle(this Vector3 v, Vector3 origin)
    {
        var d = Vector3.Dot(v, origin);
        if (d > 1)
            d = 1;
        else if (d < -1)
            d = -1;
        return Mathf.Acos(d);
    }
    
    public static Vector3 GetNormal(this Vector3 v, float len)
    {
        float divLen = len / 1;
        return v * divLen;
    }
}
