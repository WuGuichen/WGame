using System.Collections.Generic;
using Oddworm.Framework;
using Unity.Mathematics;
using UnityEngine;

namespace TWY.Physics
{
    public struct Simplex
    {
        private float3 x;
        private float3 y;
        private float3 z;
        public int Count;

        public Simplex(float3 x)
        {
            this.x = x;
            this.y = x;
            this.z = x;
            Count = 1;
        }

        public Simplex(float3 x, float3 y)
        {
            this.x = x;
            this.y = y;
            this.z = x;
            Count = 2;
        }
        public Simplex(float3 x, float3 y, float3 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            Count = 3;
        }
        public float3 this[int i] => i switch
        {
            0 => x,
            1 => y,
            2 => z,
            _ => float3.zero
        };
    }
    public class ShapeGJK
    {
        private const int MAX_ITERATIONS = 32;
        private const float epsilon = float.Epsilon;
        private static float3 MinkowskiDiffSupport(in WShape shapeA, in WShape shapeB, in float3 dir) => shapeA.Support(dir) - shapeB.Support(-dir);
        
        public static bool GJK(in AABBF shapeA, in CapsuleF shapeB, int step = 100)
        {
            DbgDraw.Sphere(Vector3.zero, quaternion.identity, new Vector3(0.2f, 0.2f, 0.2f), Color.black);
            var dir = math.normalize(shapeA.Center - shapeB.Center);
            // 随便用一个方向算初始支撑点
            var c = MinkowskiDiffSupport(shapeA, shapeB, dir);
            // DbgDraw.Sphere(c.ToVector3(), quaternion.identity, new Vector3(0.2f, 0.2f, 0.2f), Color.red/step);
            // DbgDraw.Sphere(shapeA.Support(dir).ToVector3(), quaternion.identity, new Vector3(0.2f, 0.2f, 0.2f), Color.green/step);
            // DbgDraw.Sphere(shapeB.Support(-dir).ToVector3(), quaternion.identity, new Vector3(0.2f, 0.2f, 0.2f), Color.blue/step);
            // step--;
            // if (step <= 0)
            //     return false;
            
            // 如果和原点方向相反，返回false
            if (math.dot(c, dir) < 0)
                return false;

            dir = -dir;
            var b = MinkowskiDiffSupport(shapeA, shapeB, dir);
            // DbgDraw.Sphere(b.ToVector3(), quaternion.identity, new Vector3(0.2f, 0.2f, 0.2f), Color.red/step);
            // DbgDraw.Sphere(shapeA.Support(dir).ToVector3(), quaternion.identity, new Vector3(0.2f, 0.2f, 0.2f), Color.green/step);
            // DbgDraw.Sphere(shapeB.Support(-dir).ToVector3(), quaternion.identity, new Vector3(0.2f, 0.2f, 0.2f), Color.blue/step);
            // step--;
            // if (step <= 0)
            //     return false;
            
            // 如果和原点方向相反，返回false
            if (math.dot(b, dir) < 0.0f)
                return false;
            
            step--;
            if (step < 0)
                return false;
            
            // 方向调整为垂直该线
            dir = AxBxA(c - b, -b);
            // b,c
            var simplex = new Simplex(b,c);

            float3 newPoint;
            for (int i = 0; i < MAX_ITERATIONS; i++)
            {
                dir = math.normalize(dir);
                newPoint = MinkowskiDiffSupport(shapeA, shapeB, dir);
                // DbgDraw.Sphere(newPoint.ToVector3(), quaternion.identity, new Vector3(0.2f, 0.2f, 0.2f), Color.cyan);
                // step--;
                // if (step <= 0)
                //     return false;
                // 如果和原点方向相反，返回false
                var p = math.dot(newPoint, dir);
                if (math.dot(newPoint, dir) < 0.0f)
                    return false;
                
                // 如果单形体是包含原点的四面体，返回true
                if (DoSimplex(newPoint, ref simplex, ref dir))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool DoSimplex(float3 newPoint, ref Simplex simplex, ref float3 dir)
        {
            switch (simplex.Count)
            {
                case 1:
                    // 线段
                    return DoSimplexLine(newPoint, ref simplex, ref dir);
                case 2:
                    // 三角形
                    return DoSimplexTri(newPoint, ref simplex, ref dir);
                case 3:
                    // 四面体
                    return DoSimplexTetra(newPoint, ref simplex, ref dir);
                default:
                    Debug.LogError("Simplex error, Count = " + simplex.Count);
                    return false;
            }
        }

        private static bool DoSimplexLine(float3 a, ref Simplex simplex, ref float3 dir)
        {
            var b = simplex[0];

            var ab = b - a;
            var ao = -a;

            if (math.dot(ab, ao) > 0)
            {
                // 原点在A,B之间
                simplex = new Simplex(a, b);
                dir = math.cross(ab, ao);
            }
            else
            {
                // 原点在ao以上。移除b点
                simplex = new Simplex(a);
                dir = ao;
            }

            return false;
        }
        private static bool DoSimplexTri(float3 a, ref Simplex simplex, ref float3 dir)
        {
            var b = simplex[0];
            var c = simplex[1];

            var ao = -a;

            var ab = b - a;
            var ac = c - a;

            // 三角形法线
            var nABC = math.cross(ab, ac);
            
            // 垂直于三角形AB的垂线
            var pAB = math.cross(ab, nABC);
            
            // 垂线和ao不同向，移除c点
            if (math.dot(pAB, ao) > epsilon)
            {
                simplex = new Simplex(a, b);
                dir = AxBxA(ab, ao);
                return false;
            }
            
            // 垂直于AC的垂线
            var pAC = math.cross(nABC, ac);
            if (math.dot(pAC, ao) > 0)
            {
                simplex = new Simplex(a, c);

                dir = AxBxA(ac, ao);
                return false;
            }

            if (math.dot(nABC, ao) > epsilon)
            {
                simplex = new Simplex( a, b, c );
                dir = nABC;
            }
            else
            {
                simplex = new Simplex( a, c, b );
                dir = -nABC;
            }

            return false;
        }
        private static bool DoSimplexTetra(float3 a, ref Simplex simplex, ref float3 dir)
        {
            var b = simplex[0];
            var c = simplex[1];
            var d = simplex[2];

            var ao = -a;

            var ab = b - a;
            var ac = c - a;
            var ad = d - a;

            //三个平面指向外面的法线
            var nABC = math.cross(ab, ac);
            var nACD = math.cross(ac, ad);
            var nADB = math.cross(ad, ab);

            bool over_ABC = math.dot(nABC, ao) > epsilon;
            bool over_ACD = math.dot(nACD, ao) > epsilon;
            bool over_ADB = math.dot(nADB, ao) > epsilon;

            var ob = math.dot(nABC, ao);
            var oc = math.dot(nACD, ao);
            var od = math.dot(nADB, ao);

            var rotA = a;
            var rotB = b;
            var rotC = c;
            var rotD = d;
            var rotAB = ab;
            var rotAC = ac;
            var rotAD = ad;
            var rotABC = nABC;
            var rotACD = nACD;

            if (!over_ABC && !over_ACD && !over_ADB)
            {
                //原点在三个面内部，所以原点在闵科夫斯基差之内
                //所以碰撞返回true
                // DbgDraw.Sphere(a.ToVector3(), quaternion.identity, new Vector3(0.2f, 0.2f, 0.2f), Color.red);
                // DbgDraw.Sphere(b.ToVector3(), quaternion.identity, new Vector3(0.2f, 0.2f, 0.2f), Color.yellow);
                // DbgDraw.Sphere(c.ToVector3(), quaternion.identity, new Vector3(0.2f, 0.2f, 0.2f), Color.blue);
                // DbgDraw.Sphere(d.ToVector3(), quaternion.identity, new Vector3(0.2f, 0.2f, 0.2f), Color.green);
                return true;
            }
            else if (over_ABC && !over_ACD && !over_ADB)
            {
                //the origin is over ABC, but not ACD or ADB

                rotA = a;
                rotB = b;
                rotC = c;

                rotAB = ab;
                rotAC = ac;

                rotABC = nABC;

                goto check_one_face;
            }
            else if (!over_ABC && over_ACD && !over_ADB)
            {
                //the origin is over ACD, but not ABC or ADB

                rotA = a;
                rotB = c;
                rotC = d;

                rotAB = ac;
                rotAC = ad;

                rotABC = nACD;

                goto check_one_face;
            }
            else if (!over_ABC && !over_ACD && over_ADB)
            {
                //the origin is over ADB, but not ABC or ACD

                rotA = a;
                rotB = d;
                rotC = b;

                rotAB = ad;
                rotAC = ab;

                rotABC = nADB;

                goto check_one_face;
            }
            else if (over_ABC && over_ACD && !over_ADB)
            {
                rotA = a;
                rotB = b;
                rotC = c;
                rotD = d;

                rotAB = ab;
                rotAC = ac;
                rotAD = ad;

                rotABC = nABC;
                rotACD = nACD;

                goto check_two_faces;
            }
            else if (!over_ABC && over_ACD && over_ADB)
            {
                rotA = a;
                rotB = c;
                rotC = d;
                rotD = b;

                rotAB = ac;
                rotAC = ad;
                rotAD = ab;

                rotABC = nACD;
                rotACD = nADB;

                goto check_two_faces;
            }
            else if (over_ABC && !over_ACD && over_ADB)
            {
                rotA = a;
                rotB = d;
                rotC = b;
                rotD = c;

                rotAB = ad;
                rotAC = ab;
                rotAD = ac;

                rotABC = nADB;
                rotACD = nABC;

                goto check_two_faces;
            }

        check_one_face:

            if (math.dot(math.cross(rotABC, rotAC), ao) > 0)
            {
                simplex = new Simplex(rotA, rotC);

                //新搜索方向 AC x AO x AC
                dir = AxBxA(rotAC, ao);

                return false;
            }

        check_one_face_part_2:

            if (math.dot(math.cross(rotAB, rotABC), ao) > 0)
            {
                simplex = new Simplex(rotA, rotB);

                //新搜索方向 AB x AO x AB
                dir = AxBxA(rotAB, ao);

                return false;
            }

            simplex = new Simplex(rotA, rotB, rotC);

            dir = rotABC;

            return false;

        check_two_faces:

            if (math.dot(math.cross(rotABC, rotAC), ao) > 0)
            {
                rotB = rotC;
                rotC = rotD;

                rotAB = rotAC;
                rotAC = rotAD;

                rotABC = rotACD;

                goto check_one_face;
            }

            goto check_one_face_part_2;
        }

        private static float3 AxBxA(float3 A, float3 B)
        {
            return math.cross(math.cross(A, B), A);
        }
    }
}