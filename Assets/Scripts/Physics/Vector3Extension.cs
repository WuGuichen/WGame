using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public static class Vector3Extension
{
    private static float CIRCLE = Mathf.PI*2;
    public static float3 ToFloat3(this Vector3 v)
    {
        return new float3(v.x, v.y, v.z);
    }

    /// <summary>
    /// 获取夹角（0~π）
    /// </summary>
    /// <param name="v"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static float GetAngle(this Vector3 v, Vector3 origin)
    {
        var d = Vector3.Dot(v, origin);
        if (d > 1)
            d = 1;
        else if (d < -1)
            d = -1;
        return Mathf.Acos(d);
    }
    
    /// <summary>
    /// 获取顺时针夹角（0~2π)
    /// </summary>
    /// <param name="v"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static float GetAngle360(this Vector3 v, Vector3 origin, Vector3 upVector)
    {
        var d = Vector3.Dot(v, origin);
        if (d > 1)
            d = 1;
        else if (d < -1)
            d = -1;
        var angle = Mathf.Acos(d);
        if (v.IsClockWise(origin, upVector))
            return angle;
        return CIRCLE - angle;
    }
    
    public static Vector3 GetNormal(this Vector3 v, float len)
    {
        float divLen = len / 1;
        return v * divLen;
    }

    /// <summary>
    /// 是顺时针夹角
    /// </summary>
    /// <param name="v"></param>
    /// <param name="origin"></param>
    /// <param name="upVector"></param>
    /// <returns></returns>
    public static bool IsClockWise(this Vector3 v, Vector3 origin, Vector3 upVector)
    {
        var normal = Vector3.Cross(v, origin);
        var tmp = Vector3.Dot(normal, upVector);
        return tmp < 0;
    }
}
