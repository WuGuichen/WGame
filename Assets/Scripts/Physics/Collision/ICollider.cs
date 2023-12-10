using UnityEngine;
using Unity.Mathematics;

namespace TWY.Physics
{
    public struct PointF
    {
        public float x;
        public float y;
        public float z;

        public PointF(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }


    public struct PlaneF
    {
        public float3 n;
        public float d;

        public float3 ProjectPointOn(float3 point)
        {
            return point - n * (math.dot(point, n) + d);
        }
    }
}