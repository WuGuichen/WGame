using System;
using System.Collections.Generic;
using System.Linq;
using Oddworm.Framework;
using Unity.Mathematics;
using UnityEngine;

namespace TWY.Physics
{
    struct Support
    {
        public float3 point;
        public float3 supportA;
        public float3 supportB;

        public Support(float3 p, float3 sa, float3 sb)
        {
            point = p;
            supportA = sa;
            supportB = sb;
        }
    }
    struct Simplex
    {
        private Support[] list;
        public Support[] List => list;
        public int Count;

        public Simplex(Support x)
        {
            list = new Support[] { x };
            Count = 1;
        }

        public Simplex(Support x, Support y)
        {
            list = new Support[] { x , y};
            Count = 2;
        }
        public Simplex(Support x, Support y, Support z)
        {
            list = new Support[] { x , y, z};
            Count = 3;
        }
        public Simplex(Support x, Support y, Support z, Support w)
        {
            list = new Support[] { x , y, z, w};
            Count = 4;
        }

        public Support this[int i] => list[i];
    }
    public class ShapeGJK
    {
        private const int MAX_ITERATIONS = 32;
        private const float epsilon = float.Epsilon;
        private static float3 MinkowskiDiffSupport(in WShape shapeA, in WShape shapeB, in float3 dir) => shapeA.Support(dir) - shapeB.Support(-dir);

        private static void MinkowskiDiffSupport(in WShape shapeA, in WShape shapeB, in float3 dir, out Support support)
        {
            var sA = shapeA.Support(dir);
            var sB = shapeB.Support(-dir);
            support = new Support(sA-sB, sA, sB);
        }

        private static int EPA(WShape poly1, WShape poly2, Simplex simplex, out float3 normal, out float depth,
            out float3 point)
        {
            int cnt = 0;
            List<int> faces = new List<int>
            {
                0, 1, 2,
                0, 3, 1,
                0, 2, 3,
                1, 3, 2
            };

            var polytope = simplex.List;
            //we do not know the signs of the projections of the origin onto the normals, but they must be the same
            var (normals, minFace) = GetFaceNormals(polytope, faces);
            var minNormal = F4to3(normals[minFace]);
            var minDist = float.MaxValue;

            while ((minDist == float.MaxValue) && cnt < MAX_ITERATIONS)
            {
                minNormal = math.normalize(F4to3(normals[minFace]));
                minDist = normals[minFace].w;

                MinkowskiDiffSupport(poly1, poly2, minNormal, out var support);
                var sDist = math.dot(minNormal, support.point);

                if (math.abs(sDist - minDist) > epsilon)
                {
                    minDist = float.MaxValue;
                    List<(int, int)> uniqueEdges = new List<(int, int)>();
                    for (int i = 0; i < normals.Count; i++)
                    {
                        if ((math.dot(F4to3(normals[i]), support.point) - normals[i].w) > 0)
                        {
                            int f = i * 3;
                            AddIfUniqueEdge(ref uniqueEdges, faces, f, f + 1);
                            AddIfUniqueEdge(ref uniqueEdges, faces, f + 1, f + 2);
                            AddIfUniqueEdge(ref uniqueEdges, faces, f + 2, f);
                            faces[f + 2] = faces.Last<int>();
                            faces.RemoveAt(faces.Count - 1);
                            faces[f + 1] = faces.Last<int>();
                            faces.RemoveAt(faces.Count - 1);
                            faces[f] = faces.Last<int>();
                            faces.RemoveAt(faces.Count - 1);

                            normals[i] = normals.Last<float4>();
                            normals.RemoveAt(normals.Count - 1);

                            i--;
                        }
                    }
                    if (uniqueEdges.Count == 0)
                        WLogger.Print("000");

                    //It is possible to encounter a situation where, due to invalid points(zeros, NaN), uniqueEdges does not contain any edges, which needs to be addressed.                

                    List<int> newFaces = new List<int>();
                    for (int i = 0; i < uniqueEdges.Count; i++)
                    {
                        (int, int) f = uniqueEdges[i];
                        newFaces.Add(f.Item1);
                        newFaces.Add(f.Item2);
                        newFaces.Add(polytope.Length);

                        //The direction is maintained counterclockwise.
                    }

                    var len = polytope.Length;
                    var tmp = new Support[len + 1];
                    for (int i = 0; i < len; i++)
                    {
                        tmp[i] = polytope[i];
                    }

                    tmp[len] = support;
                    polytope = tmp;
                    var (newNormals, newMinFace) = GetFaceNormals(polytope, newFaces);
                    float oldMinDistance = float.MaxValue;
                    for (int i = 0; i < normals.Count; i++)
                    {
                        if (normals[i].w < oldMinDistance)
                        {
                            oldMinDistance = normals[i].w;
                            minFace = i;
                        }
                    }

                    WLogger.Print(newNormals.Count);
                    if (newNormals[newMinFace].w < oldMinDistance)
                    {
                        minFace = newMinFace + normals.Count;
                    }

                    faces.AddRange(newFaces);
                    normals.AddRange(newNormals);
                }

                cnt++;
            }

            var a = polytope[faces[minFace * 3]];
            var b = polytope[faces[minFace * 3 + 1]];
            var c = polytope[faces[minFace * 3 + 2]];

            // Finding the projection of the origin onto the plane of the triangle
            float distance = math.dot(a.point, minNormal);
            var projectedPoint = -distance * minNormal;

            // Getting the barycentric coordinates of this projection within the triangle belonging to the simplex
            (float u, float v, float w) = GetBarycentricCoordinates(projectedPoint, a.point, b.point, c.point);

            // Taking the corresponding triangle from the first polyhedron
            var a1 = a.supportA;
            var b1 = b.supportA;
            var c1 = c.supportA;

        // Taking the corresponding triangle from the second polyhedron
            var contactPoint1 = u * a1 + v * b1 + w * c1;
            var a2 = a.supportB;
            var b2 = b.supportB;
            var c2 = c.supportB;
            
            // Contact point on the second polyhedron
            var contactPoint2 = u * a2 + v * b2 + w * c2;

            point = (contactPoint1 + contactPoint2) / 2; // Returning the midpoint
            normal = minNormal;
            depth = minDist + 0.001f;

            return cnt;
        }
        
    public static (float u, float v, float w) GetBarycentricCoordinates(float3 p, float3 a, float3 b, float3 c)
    {
        // Vectors from vertex A to vertices B and C
        float3 v0 = b - a, v1 = c - a, v2 = p - a;

        // Compute dot products
        float d00 = math.dot(v0, v0);
        float d01 = math.dot(v0, v1);
        float d11 = math.dot(v1, v1);
        float d20 = math.dot(v2, v0);
        float d21 = math.dot(v2, v1);
        float denom = d00 * d11 - d01 * d01;

        // Check for a zero denominator before division
        if (math.abs(denom) <= epsilon)
        {
            throw new InvalidOperationException("Cannot compute barycentric coordinates for a degenerate triangle.");
        }

        // Compute barycentric coordinates
        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;

        return (u, v, w);
    }

        private static void AddIfUniqueEdge(ref List<(int, int)> edges, List<int> faces, int a, int b)
    {
        //      0--<--3
        //     / \ B /   A: 2-0
        //    / A \ /    B: 0-2
        //   1-->--2
        (int, int) reverse = (0, 0);
        bool found = false;
        foreach ((int, int) f in edges)
        {
            if ((f.Item1 == faces[b]) && (f.Item2 == faces[a]))
            {
                reverse = f;
                found = true;
                break;
            }
        }

        if (found)
        {
            edges.Remove(reverse);
        }
        else
        {
            edges.Add((faces[a], faces[b]));
        }
    }
        
    private static float3 F4to3(float4 v)
    {
        return new float3(v.x, v.y, v.z);
    }

        private static (List<float4>, int) GetFaceNormals(Support[] polytope, List<int> faces)
    {
        List<float4> normals = new List<float4>();
        int minTriangle = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < faces.Count; i += 3)
        {
            var a = polytope[faces[i]].point;
            var b = polytope[faces[i + 1]].point;
            var c = polytope[faces[i + 2]].point;

            var normal = math.cross(b - a, c - a);
            float l = math.length(normal);
            float distance = float.MaxValue;

            //If vectors is colliniar we have degenerate face
            if (l < epsilon)
            {
                normal = float3.zero;
                distance = float.MaxValue;
            }
            else
            {
                normal = normal / l;
                distance = math.dot(normal, a);
            }

            //If origin outside the polytope
            if (distance < 0)
            {
                normal *= -1;
                distance *= -1;
            }
            
            // if(normal.Equals(new float3(float.NaN, float.NaN, float.NaN)))
            //     WLogger.Print("Error");

            normals.Add(new float4(normal, distance));

            if (distance < minDistance)
            {
                minTriangle = i / 3;
                minDistance = distance;
            }
        }

        if(normals.Count == 0)
            WLogger.Print("error");
        return (normals, minTriangle);
    }
        
        public static bool GJK(in AABBF shapeA, in CapsuleF shapeB, out float3 contactNormal, out float contactDepth, out float3 contactPoint)
        {
            contactPoint = contactNormal = float3.zero;
            contactDepth = 0;
            
            var dir = math.normalize(shapeA.Center - shapeB.Center);
            // 随便用一个方向算初始支撑点
            MinkowskiDiffSupport(shapeA, shapeB, dir, out var c);
            
            // 如果和原点方向相反，返回false
            if (math.dot(c.point, dir) < 0)
                return false;

            dir = -dir;
            MinkowskiDiffSupport(shapeA, shapeB, dir, out var b);
            
            // 如果和原点方向相反，返回false
            if (math.dot(b.point, dir) < 0.0f)
                return false;
            
            // 方向调整为垂直该线
            dir = AxBxA(c.point - b.point, -b.point);
            // b,c
            var simplex = new Simplex(b,c);

            for (int i = 0; i < MAX_ITERATIONS; i++)
            {
                dir = math.normalize(dir);
                MinkowskiDiffSupport(shapeA, shapeB, dir, out var newPoint);
                
                // 如果和原点方向相反，返回false
                var p = math.dot(newPoint.point, dir);
                if (math.dot(newPoint.point, dir) < 0.0f)
                    return false;
                
                // 如果单形体是包含原点的四面体，返回true
                if (DoSimplex(newPoint, ref simplex, ref dir))
                {
                    // simplex = new Simplex(simplex[0], simplex[1], simplex[2], newPoint);
                    // int epa_cnt = EPA(shapeA, shapeB, simplex, out contactNormal, out contactDepth, out contactPoint);
                    // WLogger.Print(epa_cnt);
                    return true;
                }
            }

            return false;
        }

        private static bool DoSimplex(Support newPoint, ref Simplex simplex, ref float3 dir)
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

        private static bool DoSimplexLine(Support a, ref Simplex simplex, ref float3 dir)
        {
            var b = simplex[0];

            var ab = b.point - a.point;
            var ao = -a.point;

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
        private static bool DoSimplexTri(Support a, ref Simplex simplex, ref float3 dir)
        {
            var b = simplex[0];
            var c = simplex[1];

            var ao = -a.point;

            var ab = b.point - a.point;
            var ac = c.point - a.point;

            // 三角形法线
            var nABC = math.cross(ab, ac);
            
            // 垂直于三角形AB的垂线
            var pAB = math.cross(ab, nABC);
            
            // 垂线和ao不同向，移除c点
            if (math.dot(pAB, ao) > 0.0f)
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

            if (math.dot(nABC, ao) > 0.0f)
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
        private static bool DoSimplexTetra(Support a, ref Simplex simplex, ref float3 dir)
        {
            var b = simplex[0];
            var c = simplex[1];
            var d = simplex[2];

            var ao = -a.point;

            var ab = b.point - a.point;
            var ac = c.point - a.point;
            var ad = d.point - a.point;

            //三个平面指向外面的法线
            var nABC = math.cross(ab, ac);
            var nACD = math.cross(ac, ad);
            var nADB = math.cross(ad, ab);

            bool over_ABC = math.dot(nABC, ao) > 0.0f;
            bool over_ACD = math.dot(nACD, ao) > 0.0f;
            bool over_ADB = math.dot(nADB, ao) > 0.0f;

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

            if (math.dot(math.cross(rotABC, rotAC), ao) > 0.0f)
            {
                simplex = new Simplex(rotA, rotC);

                //新搜索方向 AC x AO x AC
                dir = AxBxA(rotAC, ao);

                return false;
            }

            check_one_face_part_2:

            if (math.dot(math.cross(rotAB, rotABC), ao) > 0.0f)
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

            if (math.dot(math.cross(rotABC, rotAC), ao) > 0.0f)
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