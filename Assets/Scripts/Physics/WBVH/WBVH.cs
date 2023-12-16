using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace TWY.Physics
{
    public delegate bool WNodeTest(AABBF box);

    public class WBVHHelper
    {
        public static WNodeTest RadialNodeTest(float3 center, float radius)
        {
            return (AABBF bounds) => bounds.ClosestPointSqDist(center) < (radius * radius);
        }
    }

    internal class BVHNodeCmp<T> : IEqualityComparer<WBVHNode<T>>
    {
        public bool Equals(WBVHNode<T> x, WBVHNode<T> y)
        {
            return x.GetHashCode() == y.GetHashCode();
        }

        public int GetHashCode(WBVHNode<T> obj)
        {
            return obj.GetHashCode();
        }
    }

    public class WBVH<T>
    {
        private Material _debugMat = null;

        public WBVHNode<T> root;
        public WBVHNodeAdapter<T> adapter;
        internal readonly int LEAF_OBJ_MAX;
        public int nodeCount = 0;
        public int maxDepth = 0;

        // private float3 actualBoxSize;
        public WBVH(WBVHNodeAdapter<T> nodeAdapter, List<T> objs, int leafObjMax = 1)
        {
            this.LEAF_OBJ_MAX = leafObjMax;
            nodeAdapter.BVH = this;
            this.adapter = nodeAdapter;

            if (objs.Count > 0)
                root = new WBVHNode<T>(this, objs);
            else
            {
                root = new WBVHNode<T>(this);
                root.Objects = new List<T>();
            }
        }
        
        private void Traverse(WBVHNode<T> curNode, CapsuleF capsule, List<WBVHNode<T>> hitList)
        {
            if (curNode == null)
                return;

            // 检查hitPoint是否在box中
            if (curNode.box.IntersectCapsule(capsule))
            {
                hitList.Add(curNode);
                Traverse(curNode.left, capsule, hitList);
                Traverse(curNode.right, capsule, hitList);
            }
        }
        
        private void Traverse(WBVHNode<T> curNode, SphereF sphere, List<HitInfo> hitList)
        {
            if (curNode == null)
                return;

            // 检查hitPoint是否在box中
            if (curNode.box.IntersectSphere(sphere, out float sqrDist))
            {
                if (curNode.IsLeaf)
                {
                    hitList.Add(adapter.GetHitInfo(curNode.Objects[0], sqrDist));
                }
                Traverse(curNode.left, sphere, hitList);
                Traverse(curNode.right, sphere, hitList);
            }
        }

        private void Traverse(WBVHNode<T> curNode, SphereF sphere, List<WBVHNode<T>> hitList)
        {
            if (curNode == null)
                return;

            // 检查hitPoint是否在box中
            if (curNode.box.IntersectSphere(sphere))
            {
                hitList.Add(curNode);
                Traverse(curNode.left, sphere, hitList);
                Traverse(curNode.right, sphere, hitList);
            }
        }

        public void TestHitSphereNonAlloc(SphereF sphere, List<WBVHNode<T>> hitList)
        {
            hitList.Clear();
            this.Traverse(root, sphere, hitList);
        }
        
        public void TestHitSphereNonAlloc(SphereF sphere, ref List<HitInfo> hitList)
        {
            hitList.Clear();
            Traverse(root, sphere, hitList);
        }
        
        public void TestHitCapsuleNonAlloc(CapsuleF capsuleF, List<WBVHNode<T>> hitList)
        {
            hitList.Clear();
            this.Traverse(root, capsuleF, hitList);
        }

        public void Optimize()
        {
            if (LEAF_OBJ_MAX != 1)
            {
                throw new System.Exception("In order to use optimize, you must set LEAF_OBJ_MAX=1");
            }

            // var list = refitNodes.ToList();
            //
            // var sweepNodes = refitNodes.Where(n => n.depth == maxDepth).ToList();
            // sweepNodes.ForEach(n => refitNodes.Remove(n));
            // 就是从最深层开始进行tryRotate
            // sweepNodes.ForEach(n => n.TryRotate(this));
            adapter.Optimize();
        }

        public void Add(T newObj)
        {
            AABBF box = new AABBF(adapter.GetObjectPos(newObj), new float3(adapter.GetRadius(newObj)));
            float sah = WBVHNode<T>.SA(ref box);
            root.Add(adapter, newObj, ref box, sah);
        }

        public void Remove(T obj)
        {
            var leaf = adapter.GetLeaf(obj);
            leaf.Remove(adapter, obj);
        }

        /// <summary>
        /// 标记要更新这个物体
        /// </summary>
        /// <param name="toUpdate"></param>
        public void MarkForUpdate(T toUpdate) => adapter.OnPositionOrSizeChanged(toUpdate);

        #region Visulize

        //Bounds rendering ( for debugging )
        //=======\/=======\/=======\/=======\/=======\/=======\/=======\/=======\/=======\/=======\/=======\/=======\/

        private static readonly List<Vector3> vertices = new List<Vector3>
        {
            new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
        };

        private static readonly int[] indices =
        {
            0, 1, 1, 2, 2, 3, 3, 0, // face1
            4, 5, 5, 6, 6, 7, 7, 4, // face2
            0, 4, 1, 5, 2, 6, 3, 7 // interconnects
        };

        public void GetAllNodeMatricesRecursive(WBVHNode<T> n, ref List<Matrix4x4> matrices, int depth)
        {
            Vector3 center = new Vector3(n.box.Center.x, n.box.Center.y, n.box.Center.z);
            Vector3 size = new Vector3(n.box.Size.x, n.box.Size.y, n.box.Size.z);
            Matrix4x4 matrix = Matrix4x4.Translate(center) * Matrix4x4.Scale(size);
            matrices.Add(matrix);
            
            if(n.right != null)
                GetAllNodeMatricesRecursive(n.right, ref matrices, depth + 1);
            if (n.left != null)
                GetAllNodeMatricesRecursive(n.left, ref matrices, depth + 1);
        }

        public void RenderDebug()
        {
            if(!SystemInfo.supportsInstancing)
                Debug.LogError("[BVH] Cannot render BVH. Mesh instancing not supported by system");
            else
            {
                List<Matrix4x4> matrices = new List<Matrix4x4>();
                
                GetAllNodeMatricesRecursive(root, ref matrices, 0);

                Mesh mesh = new Mesh();
                mesh.SetVertices(vertices);
                mesh.SetIndices(indices, MeshTopology.Lines, 0);

                if (_debugMat == null)
                {
                    _debugMat = new Material(Shader.Find("Standard"))
                    {
                        enableInstancing = true
                    };
                }

                Graphics.DrawMeshInstanced(mesh, 0, _debugMat, matrices);
            }
        }
        
        public void DrawAllBounds()
        {
            root.DrawAllBounds();
        }

        #endregion
    }
}