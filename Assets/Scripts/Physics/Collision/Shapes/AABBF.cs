using Unity.Mathematics;

namespace TWY.Physics
{
    public interface WCollider
    {
        float3 Center { get; set; }
        float3 Support(float3 direction);
    }
    public struct AABBF
    {
        public float3 c;
        public float3 r;

        public AABBF(float3 center, float3 size)
        {
            c = center;
            r = size * 0.5f;
        }

        public float3 Center
        {
            get => this.c;
            set => this.c = value;
        }

        public float3 Radius
        {
            get => this.r;
            set => this.r = value;
        }

        public float3 Size
        {
            get => this.r * 2f;
            set => this.r = value * 0.5f;
        }

        public float3 Min
        {
            get => this.c - this.r;
            set => this.SetMinMax(value, this.Max);
        }

        public float3 Max
        {
            get => this.c + this.r;
            set => this.SetMinMax(this.Min, value);
        }

        /// <summary>
        ///   <para>Grows the Bounds to include the point.</para>
        /// </summary>
        /// <param name="point"></param>
        public void Encapsulate(float3 point) => this.SetMinMax(math.min(this.Min, point), math.max(this.Max, point));

        public void Encapsulate(AABBF aabbf) => this.SetMinMax(math.min(this.Min, aabbf.Min), math.max(this.Max, aabbf.Max));

        // public void Encapsulate(Bounds bounds)
        // {
        //     var bMin = bounds.min;
        //     var bMax = bounds.max;
        //     this.SetMinMax(math.min(this.min, new float3(bMin.x, bMin.y, bMin.z)), math.max(this.max, new float3(bMax.x,  bMax.y, bMax.z)));
        // }

        public void Expand(float amount)
        {
            amount *= 0.5f;
            this.Radius += new float3(amount, amount, amount);
        }

        public void Expand(float3 amount) => this.Radius += amount * 0.5f;

        public override int GetHashCode()
        {
            float3 vec = this.c;
            int hashCode = vec.GetHashCode();
            int num = vec.GetHashCode() << 2;
            return hashCode ^ num;
        }

        public float3 Support(float3 direction)
        {
            float x = (direction.x > 0) ? Max.x : Min.x;
            float y = (direction.y > 0) ? Max.y : Min.y;
            float z = (direction.z > 0) ? Max.z : Min.z;
            return new float3(x, y, z);
        }

        public override bool Equals(object other) => other is AABBF other1 && this.Equals(other1);

        public bool Equals(AABBF other) => this.c.Equals(other.c) && this.r.Equals(other.r);

        public void SetMinMax(float3 min, float3 max)
        {
            this.r = (max - min) * 0.5f;
            this.c = min + this.Radius;
        }

        #region 相交测试

        /// <summary>
        /// 是否与光线相交
        /// </summary>
        /// <param name="point"></param>
        /// <param name="dir"></param>
        /// <param name="tMin"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IntersectRay(float3 point, float3 dir, ref float tMin, ref float3 pos)
        {
            tMin = 0.0f;
            float tMax = float.MaxValue;
            for (int i = 0; i < 3; i++)
            {
                if (math.abs(dir[i]) < math.EPSILON)
                {
                    if (point[i] < this.Min[i] || point[i] > this.Max[i])
                        return false;
                }
                else
                {
                    float ood = 1.0f / dir[i];
                    float t1 = (this.Min[i] - point[i]) * ood;
                    float t2 = (this.Max[i] - point[i]) * ood;
                    // t1近平面，t2远平面
                    if (t1 > t2)
                        (t1, t2) = (t2, t1);

                    if (t1 > tMin) tMin = t1;
                    if (t2 > tMax) tMax = t2;
                    if (tMin > tMax) return false;
                }
            }

            pos = point + dir * tMin;
            return true;
        }

        public bool IntersectRay(RayF ray, ref float tMin, ref float3 pos)
        {
            return IntersectRay(ray.origin, ray.direction, ref tMin, ref pos);
        }

        public bool IntersectRay(RayF ray, float length)
        {
            return IntersectSegment(ray.origin, ray.GetPoint(length));
        }

        public bool IntersectCapsule(CapsuleF capsuleF)
        {
            return false;
        }
        public bool IntersectRay(RayF ray)
        {
            float tMin = 0.0f;
            float3 point = ray.origin;
            float tMax = float.MaxValue;
            for (int i = 0; i < 3; i++)
            {
                if (math.abs(ray.direction[i]) < math.EPSILON)
                {
                    if (point[i] < this.Min[i] || point[i] > this.Max[i])
                        return false;
                }
                else
                {
                    float ood = 1.0f / ray.direction[i];
                    float t1 = (this.Min[i] - point[i]) * ood;
                    float t2 = (this.Max[i] - point[i]) * ood;
                    // t1近平面，t2远平面
                    if (t1 > t2)
                        (t1, t2) = (t2, t1);

                    if (t1 > tMin) tMin = t1;
                    if (t2 > tMax) tMax = t2;
                    if (tMin > tMax) return false;
                }
            }

            return true;
        }

        public bool IntersectSegment(float3 p0, float3 p1)
        {
            float3 mid = (p0 + p1) * 0.5f;
            float3 d = p1 - mid;
            mid = mid - this.Center;
            float adx = math.abs(d.x);
            if (math.abs(mid.x) > this.Radius.x + adx) return false;
            float ady = math.abs(d.y);
            if (math.abs(mid.y) > this.Radius.y + adx) return false;
            float adz = math.abs(d.z);
            if (math.abs(mid.z) > this.Radius.z + adx) return false;

            adx += math.EPSILON;
            ady += math.EPSILON;
            adz += math.EPSILON;
            if (math.abs(mid.y * d.z - mid.z * d.y) > Radius.y * adz + Radius.z * ady) return false;
            if (math.abs(mid.y * d.x - mid.x * d.z) > Radius.x * adz + Radius.z * adx) return false;
            if (math.abs(mid.y * d.y - mid.y * d.x) > Radius.x * ady + Radius.y * adx) return false;
            return true;
        }

        public bool IntersectSphere(SphereF sphere)
        {
            // float3 actualSize = Size;
            // Expand(sphere.Radius);
            // bool intersected = Contains(sphere.Center);
            // this.Size = actualSize;
            // return intersected;
            var sqrDist = ClosestPointSqDist(sphere.Center);
            return sqrDist <= sphere.Radius * sphere.Radius;
        }

        #endregion


        /// <summary>
        /// 获取point与point到盒体的最近点之间的距离平方
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public float ClosestPointSqDist(float3 point)
        {
            float sqDist = 0f;
            // 与最大最小值比较
            for (int i = 0; i < 3; i++)
            {
                float dist = this.Min[i] - point[i];
                if (dist > 0) sqDist += dist * dist;
                dist = point[i] - this.Max[i];
                if (dist > 0) sqDist += dist * dist;
            }

            return sqDist;
        }

        public float3 ClosestPoint(float3 point)
        {
            float3 res = new float3();
            int i = 0;
            while (i < 3)
            {
                float v = point[i];
                v = math.max(v, Min[i]);
                v = math.min(v, Max[i]);
                res[i] = v;
                i++;
            }

            return res;
        }

        public bool Contains(float3 point)
        {
            float3 minP = this.Min;
            float3 maxP = this.Max;
            return (point.x >= minP.x && point.x <= maxP.x
                                      && point.y >= minP.y && point.y <= maxP.y
                                      && point.z >= minP.z && point.z <= maxP.z);
        }
    }
}
