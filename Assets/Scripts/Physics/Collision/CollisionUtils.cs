using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace TWY.Physics
{
    public class CollisionUtils
    {
        #region 计算

        /// <summary>
        /// 三点计算一个平面
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public PlaneF ComputePlaneF(float3 a, float3 b, float3 c)
        {
            PlaneF p;
            p.n = math.normalize(math.cross(b - a, c - a));
            p.d = math.dot(p.n, a); // 平面上任意一点乘以法向量
            return p;
        }

        /// <summary>
        /// 计算点集pts中距离最远的两个点的位置
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="pts"></param>
        void MostSeparatedPointsOnAABB(out int min, out int max, float3[] pts)
        {
            int[] maxIndex = new int[3];
            int[] minIndex = new int[3];
            for (int i = 0; i < pts.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (pts[i][j] < pts[minIndex[j]][j]) minIndex[j] = i;
                    if (pts[i][j] > pts[maxIndex[j]][j]) maxIndex[j] = i;
                }
            }

            float[] dists = new float[3];
            for (int i = 0; i < 3; i++)
            {
                float3 l = pts[maxIndex[i]] - pts[minIndex[i]];
                dists[i] = math.dot(l, l);
            }

            // 找到最大的dist
            float maxDist = dists[0];
            int index = 0;
            for (int i = 1; i < 3; i++)
            {
                if (dists[i] > maxDist)
                {
                    maxDist = dists[i];
                    index = i;
                }
            }

            min = minIndex[index];
            max = maxIndex[index];
        }

        void SphereFromDistantPoints(ref SphereF s, float3[] pts)
        {
            int min, max;
            MostSeparatedPointsOnAABB(out min, out max, pts);
            s.Center = (pts[min] + pts[max]) * 0.5f;
            float3 r = pts[max] - s.Center;
            s.Radius = math.sqrt(math.dot(r, r));
        }

        /// <summary>
        /// 球体更新为包含p的新球体
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        void SphereOfSphereAndPoint(ref SphereF s, float3 p)
        {
            float3 d = p - s.Center;
            float dist2 = math.dot(d, d);

            if (dist2 > s.Radius * s.Radius)
            {
                float dist = math.sqrt(dist2);
                float newRadius = (s.Radius + dist) * 0.5f;
                float k = (newRadius - s.Radius) / dist;
                s.Radius = newRadius;
                s.Center += d * k;
            }
        }

        /// <summary>
        /// 计算逼近包围球
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pts"></param>
        public void RitterSphere(ref SphereF s, float3[] pts)
        {
            // 获取两个近似点
            SphereFromDistantPoints(ref s, pts);

            // 增加球的范围
            for (int i = 0; i < pts.Length; i++)
                SphereOfSphereAndPoint(ref s, pts[i]);
        }

        float Variance(float[] x, int n)
        {
            return 0;
        }

        public void Barycentric(float3 a, float3 b, float3 c, float3 p, ref float u, ref float v, ref float w)
        {
            var v0 = b - a;
            var v1 = c - a;
            var v2 = p - a;
            var d00 = math.dot(v0, v0);
            var d01 = math.dot(v0, v1);
            var d11 = math.dot(v1, v1);
            var d20 = math.dot(v2, v0);
            var d21 = math.dot(v2, v1);
            var denom = 1/(d00 * d11 - d01 * d01);
            v = (d11 * d20 - d01 * d21) * denom;
            w = (d00 * d21 - d01 * d20) * denom;
            u = 1.0f - v - w;
        }

        #endregion

        #region 球、点、AABB相交检测

        /// <summary>
        /// AABB上离点最近的点
        /// </summary>
        /// <param name="p"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public float[] ClosestPtPointAABB(float3 p, AABBF b)
        {
            float[] res = new float[3];
            int i = 0;
            while (i < 3)
            {
                float v = p[i];
                v = math.max(v, b.Center[i] - b.Radius[i]);
                v = math.min(v, b.Center[i] + b.Radius[i]);
                res[i] = v;
                i++;
            }

            return res;
        }

        /// <summary>
        /// 点到AABB的距离平方
        /// </summary>
        /// <param name="p"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public float SqDistPointAABB(float3 p, AABBF b)
        {
            float sqDist = 0f;
            // 与最大最小值比较
            for (int i = 0; i < 3; i++)
            {
                float dist = b.Center[i] - b.Radius[i] - p[i];
                if (dist > 0) sqDist += dist * dist;
                dist = p[i] - (b.Center[i] + b.Radius[i]);
                if (dist > 0) sqDist += dist * dist;
            }

            return sqDist;
        }

        /// <summary>
        /// AABB与AABB是否相交
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool TestAABBAABB(AABBF a, AABBF b)
        {
            for (int i = 0; i < 3; i++)
            {
                if (math.abs(a.Center[i] - b.Center[i]) > (a.Radius[i] + b.Radius[i]))
                    return false;
            }

            return true;
        }

        public bool TestSphereSphere(SphereF a, SphereF b)
        {
            float3 d = a.Center - b.Center;
            float dist2 = math.dot(d, d);
            float radiusSum = a.Radius + b.Radius;
            return dist2 <= radiusSum * radiusSum;
        }

        /// <summary>
        /// 球和AABB是否相交
        /// </summary>
        /// <param name="s"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool TestSphereAABB(SphereF s, AABBF b)
        {
            float sqDist = SqDistPointAABB(s.Center, b);
            return sqDist <= s.Radius * s.Radius;
        }

        /// <summary>
        /// 球和AABB是否相交并返回交点
        /// </summary>
        /// <param name="s"></param>
        /// <param name="b"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool TestSphereAABBPoint(SphereF s, AABBF b, ref float[] p)
        {
            p = ClosestPtPointAABB(s.Center, b);
            float3 v = new float3(p[0] - s.Center[0], p[1] - s.Center[1], p[2] - s.Center[2]);
            return math.dot(v, v) <= s.Radius * s.Radius;
        }

        // public bool TestRaySphere(float3 point, float3 dir, SphereF s)
        // {
        //     float3 m = point - s.center;
        // }

        #endregion

        #region 运动球体与平面的相交检测

        /// <summary>
        /// 运动球体与平面相交检测
        /// </summary>
        /// <param name="s">球体</param>
        /// <param name="v">移动向量</param>
        /// <param name="p">平面</param>
        /// <param name="time">相交时间[0,1]</param>
        /// <param name="point">相交点</param>
        /// <returns></returns>
        public bool IntersectMovingSpherePlane(SphereF s, float3 v, PlaneF p, ref float time, ref float3 point)
        {
            // 球心与平面的距离
            float dist = math.dot(p.n, s.Center) - p.d;
            if (math.abs(dist) < s.Radius)
            {
                // 球与平面相交
                // 设置当前时刻为0，交点为球心
                time = 0f;
                point = s.Center;
                return true;
            }
            else
            {
                float denom = math.dot(p.n, v);
                if (denom * dist >= 0.0f)
                {
                    // 球体背向平面运动，不可能相交
                    return false;
                }
                else
                {
                    // 球体朝向平面运动
                    // 减去球的半径
                    float r = dist > 0.0f ? s.Radius : -s.Radius;
                    time = (r - dist) / denom;
                    point = s.Center + time * v - r * p.n;
                    return true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a">起点</param>
        /// <param name="b">终点</param>
        /// <param name="r">半径</param>
        /// <param name="p">平面</param>
        /// <returns></returns>
        public bool TestMovingSpherePlane(float3 a, float3 b, float r, PlaneF p)
        {
            // 点到面的距离(p.n为单位向量)
            float aDist = math.dot(a, p.n) - p.d;
            float bDist = math.dot(b, p.n) - p.d;

            // 在平面两侧
            if (aDist * bDist < 0.0f) return true;
            // 起点或终点与平面相交
            if (math.abs(aDist) <= r || math.abs(bDist) <= r) return true;
            return false;
        }

        #endregion

        #region 运动AABB与平面的相交测试

        #endregion
    }
}