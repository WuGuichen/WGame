using Unity.Mathematics;
using UnityEngine;

namespace TWY.Physics
{
    public struct CapsuleF : WShape
    {
        private float3 pointA;
        private float3 pointB;
        private float radius;
        private float height;

        public float3 PointA
        {
            get => pointA;
            set => pointA = value;
        }

        public float3 PointB
        {
            get => pointB;
            set => pointB = value;
        }

        public float Radius
        {
            get => radius;
            set => radius = value;
        }

        public float Height
        {
            get => height;
            set => height = math.distance(PointB, PointA) + radius * 2;
        }

        public float3 Center
        {
            get => (PointA + pointB) / 2;
            set {}
        }

        public CapsuleF(float3 ptA, float3 ptB, float radius)
        {
            this.pointA = ptA;
            this.pointB = ptB;
            this.radius = radius;
            this.height = math.distance(ptB , ptA) + this.radius * 2;
        }

        #region 相交测试

        /// <summary>
        /// 点到ab线段的平方距离
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public float SqDistSegmentToPoint(float3 pointC)
        {
            float3 ab = pointB - pointA;
            float3 ac = pointC - pointA;
            float3 bc = pointB - pointC;

            float e = math.dot(ac, ab);
            // c点投影在ab之外
            if (e <= 0.0f) return math.dot(ac, ac);
            float f = math.dot(ab, ab);
            if (e >= f) return math.dot(bc, bc);
            // c点投影在ab上
            return math.dot(ac, ac) - e * e / f;
        }

        /// <summary>
        /// 线段cd到线段ab上最近的点c1和c2, 以及系数s, t
        /// </summary>
        /// <returns>c1和c2的距离平方</returns>
        public float ClosestPtSegment(float3 pointC, float3 pointD, out float s, out float t,
            out float3 c1, out float3 c2)
        {
            float3 d1 = pointB - pointA;
            float3 d2 = pointD - pointC;
            float3 r = pointA - pointC;

            float a = math.dot(d1, d1);
            float e = math.dot(d2, d2);

            float f = math.dot(d2, r);

            if (a <= math.EPSILON && e <= math.EPSILON)
            {
                // 两个线段退化成两个点的情况
                s = t = 0f;
                c1 = pointA;
                c2 = pointC;
                return math.dot(c1 - c2, c1 - c2);
            }

            if (a <= math.EPSILON)
            {
                // ab退化成点
                s = 0.0f;
                t = f / e;
                t = math.clamp(t, 0.0f, 1.0f);
            }
            else
            {
                float c = math.dot(d1, r);
                if (e <= math.EPSILON)
                {
                    // cd退化成点
                    t = 0.0f;
                    s = math.clamp(-c / a, 0.0f, 1.0f);
                }
                else
                {
                    //todo 可优化除法操作
                    float b = math.dot(d1, d2);
                    float denom = a * e - b * b;

                    if (denom != 0.0f)
                        s = math.clamp((b * f - c * e) / denom, 0.0f, 1.0f);
                    else
                        s = 0.0f;

                    t = (b * s + f) / e;

                    if (t < 0.0f)
                    {
                        t = 0.0f;
                        s = math.clamp(-c / a, 0.0f, 1.0f);
                    }
                    else if(t > 1.0f)
                    {
                        t = 1.0f;
                        s = math.clamp((b - c) / a, 0.0f, 1.0f);
                    }
                }
            }

            c1 = pointA + d1 * s;
            c2 = pointB + d2 * t;
            return math.dot(c1 - c2, c1 - c2);
        }

        public bool IntersectSphere(SphereF sphere)
        {
            float dist2 = SqDistSegmentToPoint(sphere.Center);
            float r = sphere.Radius + radius;
            return dist2 <= r * r;
        }
        
        public bool IntersectPoint(float3 point)
        {
            float dist2 = SqDistSegmentToPoint(point);
            return dist2 <= Radius*Radius;
        }

        public bool IntersectCapsule(CapsuleF capsule)
        {
            float s, t;
            float3 c1, c2;
            float dist2 = ClosestPtSegment(capsule.pointA, capsule.pointB, out s, out t, out c1, out c2);

            float r = radius + capsule.Radius;
            return dist2 <= r * r;
        }

        public void Draw()
        {
            Gizmos.color = Color.cyan;
            // Gizmos.DrawWireSphere(new Vector3(pointA.x, pointA.y, pointA.z), radius);
            // Gizmos.DrawWireSphere(new Vector3(pointB.x, pointB.y, pointB.z), radius);
            // Gizmos.DrawLine(new Vector3(pointA.x, pointA.y, pointA.z), new Vector3(pointB.x, pointB.y, pointB.z));
            DrawCapsule(new Vector3(pointA.x, pointA.y, pointA.z),new Vector3(pointB.x, pointB.y, pointB.z), radius );
        }

        #endregion

        public float3 Support(float3 direction)
        {
            var center = (PointA + PointB) / 2;
            // todo 不旋转的可以优化
            var rotation = Quaternion.FromToRotation(Vector3.up, PointB - PointA);
            var localDirection = Quaternion.Inverse(rotation) * direction;
            float halfLength = Height * 0.5f;
            var res = localDirection * radius;
            res.y += (localDirection.y > 0) ? (halfLength - radius) : (radius - halfLength);
            return (rotation * res + center.ToVector3()).ToFloat3();
            // float Dy = math.dot(direction, new float3(0, 1, 0));
            // var absY = math.abs(Dy);
            // if (absY > 0.9999f)
            // {
            //     // 平行的
            //     return (Dy > 0.0f ? pointB : pointA);
            // }
            // else if (absY < 0.0001f)
            // {
            //     return (Center - Radius * direction);
            // }
            // else
            // {
            //     if (Dy < 0.0f)
            //     {
            //         return pointB + Radius * direction;
            //     }
            //     else
            //     {
            //         return pointA + Radius * direction;
            //     }
            // }
        }

        private void DrawCapsule(Vector3 point1, Vector3 point2, float radius)
        {
            Vector3 centerTop = point2;
            Vector3 centerBottom = point1;

            Gizmos.DrawWireSphere(centerTop, radius);
            Gizmos.DrawWireSphere(centerBottom, radius);

            Gizmos.DrawLine(centerTop + radius * Vector3.right, centerTop + radius * Vector3.forward);
            Gizmos.DrawLine(centerTop + radius * Vector3.forward, centerTop - radius * Vector3.right);
            Gizmos.DrawLine(centerTop - radius * Vector3.right, centerTop - radius * Vector3.forward);
            Gizmos.DrawLine(centerTop - radius * Vector3.forward, centerTop + radius * Vector3.right);

            // Draw bottom cap
            Gizmos.DrawLine(centerBottom + radius * Vector3.right, centerBottom + radius * Vector3.forward);
            Gizmos.DrawLine(centerBottom + radius * Vector3.forward, centerBottom - radius * Vector3.right);
            Gizmos.DrawLine(centerBottom - radius * Vector3.right, centerBottom - radius * Vector3.forward);
            Gizmos.DrawLine(centerBottom - radius * Vector3.forward, centerBottom + radius * Vector3.right);

            // Draw connecting lines
            Gizmos.DrawLine(centerTop + radius * Vector3.right, centerBottom + radius * Vector3.right);
            Gizmos.DrawLine(centerTop + radius * Vector3.forward, centerBottom + radius * Vector3.forward);
            Gizmos.DrawLine(centerTop - radius * Vector3.right, centerBottom - radius * Vector3.right);
            Gizmos.DrawLine(centerTop - radius * Vector3.forward, centerBottom - radius * Vector3.forward);
        }
    }
}
