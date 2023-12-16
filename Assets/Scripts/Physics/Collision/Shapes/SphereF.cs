using Unity.Mathematics;

namespace TWY.Physics
{
    public struct SphereF : WCollider
    {
        public SphereF(float3 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
            this.SqrRadius = radius * radius;
        }

        public float3 Center { get; set; }

        public float Radius { get; set; }

        public float SqrRadius { get; set; }

        /// <summary>
        /// 光线与球是否相交
        /// </summary>
        /// <param name="point">光线起点</param>
        /// <param name="dir">光线标准化的的方向</param>
        /// <param name="t">辨别式的值</param>
        /// <param name="pos">交点</param>
        /// <returns>是否相交</returns>
        public bool IntersectRay(float3 point, float3 dir, ref float t, ref float3 pos)
        {
            float3 m = point - this.Center;
            float b = math.dot(m, dir);
            float c = math.dot(m, m) - this.Radius * this.Radius;
            if (c > 0.0f && b > 0.0f) return false;
            float discr = b * b - c;
            if (discr < 0.0f) return false;
            t = -b - math.sqrt(discr);
            if (t < 0.0f) return false;
            pos = point + t * dir;
            return true;
        }

        public bool IntersectRay(float3 point, float3 dir)
        {
            float3 m = point - this.Center;
            float c = math.dot(m, m) - this.Radius * this.Radius;
            if (c <= 0.0f) return false;
            float b = math.dot(m, dir);
            if (b > 0.0f) return false;
            float discr = b * b - c;
            if (discr < 0.0f) return false;
            return true;
        }

        public float3 Support(float3 direction)
        {
            return Center + direction * Radius;
        }
    }
}