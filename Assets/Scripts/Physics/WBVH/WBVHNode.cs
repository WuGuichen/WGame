using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using JetBrains.Annotations;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;
using UnityEditor;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace TWY.Physics
{
    public enum Axis
    {
        X,
        Y,
        Z,
    }
    
    public class WBVHNode<T>
    {
        public AABBF box;

        public WBVHNode<T> parent { get; private set; }
        public WBVHNode<T> left { get; private set; }
        public WBVHNode<T> right { get; private set; }

        public int depth { get; private set; }
        public int nodeNumber { get; private set; }

        private List<T> _objects;

        public List<T> Objects
        {
            get { return _objects; }
            set { _objects = value; }
        }

        internal WBVHNode(WBVH<T> bvh)
        {
            _objects = null;
            left = right = null;
            parent = null;
            this.nodeNumber = bvh.nodeCount++;
        }

        internal WBVHNode(WBVH<T> bvh, List<T> goObjList) : this(bvh, null, goObjList, Axis.X, 0)
        {
            
        }

        internal WBVHNode(WBVH<T> bvh, WBVHNode<T> left, WBVHNode<T> right, WBVHNode<T> parent, List<T> objects)
        {
            _objects = objects;
            this.left = left;
            this.left.parent = this;
            this.right = right;
            this.right.parent = this;
            this.parent = parent;
            this.nodeNumber = bvh.nodeCount++;
        }

        internal WBVHNode(WBVH<T> bvh, WBVHNode<T> lparent, List<T> goObjList, Axis lastSplitAxis, int curDepth)
        {
            WBVHNodeAdapter<T> adapter = bvh.adapter;
            this.nodeNumber = bvh.nodeCount++;

            this.parent = lparent;
            this.depth = curDepth;

            if (bvh.maxDepth < curDepth)
                bvh.maxDepth = curDepth;
            
            if(goObjList == null || goObjList.Count < 1)
                throw new Exception("ssBVHNode constructed with invalid paramaters");

            if (goObjList.Count <= bvh.LEAF_OBJ_MAX)
            {
                left = null;
                right = null;

                _objects = goObjList;
                _objects.ForEach(o => adapter.MapObj2BVHLeaf(o, this));
                ComputeVolume(adapter);
                SplitIfNecessary(adapter);
            }
            else
            {
                // 物体数量超过叶子节点容量，计算体积并分割节点
                _objects = goObjList;
                ComputeVolume(adapter);
                SplitNode(adapter);
                ChildRefit(adapter, propagate: false);
            }
        }

        public override string ToString()
        {
            return string.Format("WBVHNode<{0}>:{1}", typeof(T), this.nodeNumber);
        }

        public void SetLeft(WBVHNode<T> node) => left = node;
        public void SetRight(WBVHNode<T> node) => right = node;

        public bool IsLeaf
        {
            get
            {
                bool isLeaf = (_objects != null);
                if (isLeaf && ((right != null) || (left != null)))
                {
                    throw new System.Exception("BVH Leaf has objects and left/right pointers!");
                }

                return isLeaf;
            }
        }
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static List<Axis> EachAxis
        {
            get { return new List<Axis>((Axis[])Enum.GetValues(typeof(Axis))); }
        }

        public void RefitObjectChanged(WBVHNodeAdapter<T> adapter, T obj)
        {
            if (_objects == null)
                throw new Exception("dangling leaf!");

            if (RefitVolume(adapter))
            {
                if (parent != null)
                {
                    adapter.BVH.refitNodes.Add(parent);
                }
            }
        }

        private void ExpandVolume(WBVHNodeAdapter<T> adapter, float3 objPos, float radius)
        {
            if (ExpandBox(new AABBF(objPos, new float3(radius * 2))) && parent != null)
                parent.ChildExpanded(adapter, this);
        }

        private bool ExpandBox(AABBF bounds)
        {
            // bool expanded = false;
            // float3 objectpos = bounds.center;
            // float radius = math.max(math.max(bounds.size.x, bounds.size.y), bounds.size.z);
            //
            // var aabbf = box;
            // if ((objectpos.x - radius) < aabbf.min.x)
            // {
            //     aabbf.min = new float3(objectpos.x - radius, aabbf.min.y, aabbf.min.z);
            //     expanded = true;
            // }
            //
            // if ((objectpos.x + radius) > aabbf.max.x)
            // {
            //     aabbf.max = new float3(objectpos.x + radius, aabbf.max.y, aabbf.max.z);
            //     expanded = true;
            // }
            //
            // // test min Y and max Y against the current bounding volume
            // if ((objectpos.y - radius) < aabbf.min.y)
            // {
            //     aabbf.min = new float3(aabbf.min.x, (objectpos.y - radius), aabbf.min.z);
            //     expanded = true;
            // }
            //
            // if ((objectpos.y + radius) > aabbf.max.y)
            // {
            //     aabbf.max = new float3(aabbf.max.x, (objectpos.y + radius), aabbf.max.z);
            //     expanded = true;
            // }
            //
            // // test min Z and max Z against the current bounding volume
            // if ((objectpos.z - radius) < aabbf.min.z)
            // {
            //     aabbf.min = new float3(aabbf.min.x, aabbf.min.y, (objectpos.z - radius));
            //     expanded = true;
            // }
            //
            // if ((objectpos.z + radius) > aabbf.max.z)
            // {
            //     aabbf.max = new float3(aabbf.max.x, aabbf.max.y, (objectpos.z + radius));
            //     expanded = true;
            // }
            //
            // this.box = aabbf;
            //
            // return expanded;
            float3 oldSize = box.Size;
            box.Encapsulate(bounds);
            return !oldSize.Equals(box.Size);
        }

        internal void ChildExpanded(WBVHNodeAdapter<T> adapter, WBVHNode<T> child)
        {
            bool expanded = false;

            if (child.box.Min.x < box.Min.x)
            {
                box.Min = new float3(child.box.Min.x, box.Min.y, box.Min.z);
                expanded = true;
            }

            if (child.box.Max.x > box.Max.x)
            {
                box.Max = new float3(child.box.Max.x, box.Max.y, box.Max.z);
                expanded = true;
            }

            if (child.box.Min.y < box.Min.y)
            {
                box.Min = new float3(box.Min.x, child.box.Min.y, box.Min.z);
                expanded = true;
            }

            if (child.box.Max.y > box.Max.y)
            {
                box.Max = new float3(box.Max.x, child.box.Max.y, box.Max.z);
                expanded = true;
            }

            if (child.box.Min.z < box.Min.z)
            {
                box.Min = new float3(box.Min.x, box.Min.y, child.box.Min.z);
                expanded = true;
            }

            if (child.box.Max.z > box.Max.z)
            {
                box.Max = new float3(box.Max.x, box.Max.y, child.box.Max.z);
                expanded = true;
            }

            if (expanded && parent != null) 
                parent.ChildExpanded(adapter, this);
        }

        private void AssignVolume(float3 objPos, float radius) => box = new AABBF(objPos, new float3(radius * 2));

        private void ComputeVolume(WBVHNodeAdapter<T> adapter)
        {
            AssignVolume(adapter.GetObjectPos(_objects[0]), adapter.GetRadius(_objects[0]));
            for (int i = 1; i < _objects.Count; i++)
            {
                ExpandVolume(adapter, adapter.GetObjectPos(_objects[i]), adapter.GetRadius(_objects[i]));
            }
        }

        private bool RefitVolume(WBVHNodeAdapter<T> adapter)
        {
            if (_objects.Count == 0)
                throw new NotImplementedException();

            AABBF oldBox = box;
            
            ComputeVolume(adapter);

            if (!box.Equals(oldBox))
            {
                if(parent != null)
                    parent.ChildRefit(adapter);
                return true;
            }

            return false;
        }

        internal enum Rot
        {
            NONE,
            L_RL,
            L_RR,
            R_LL,
            R_LR,
            LL_RR,
            LL_RL,
        }

        internal class RotOpt : IComparable<RotOpt>
        {
            // rotation option
            public float SAH;
            public Rot rot;

            internal RotOpt(float SAH, Rot rot)
            {
                this.SAH = SAH;
                this.rot = rot;
            }

            public int CompareTo(RotOpt other)
            {
                return SAH.CompareTo(other.SAH);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static List<Rot> EachRot
        {
            get { return new List<Rot>((Rot[])Enum.GetValues(typeof(Rot))); }
        }

        // node表面积
        internal static float SA(WBVHNode<T> node)
        {
            float3 size = node.box.Size;
            return 2.0f * (size.x * size.y + size.y * size.z + size.z * size.x);
        }

        internal static float SA(AABBF box)
        {
            float3 size = box.Size;
            return 2.0f * (size.x * size.y + size.y * size.z + size.z * size.x);
        }

        internal static float SA(ref AABBF box)
        {
            float3 size = box.Size;
            return 2.0f * ((size.x * size.y + size.y * size.z * size.z * size.x));
        }

        internal void Add(WBVHNodeAdapter<T> adapter, T newObj, ref AABBF newBox, float newSAH)
        {
            Add(adapter, this, newObj, ref newBox, newSAH);
        }

        internal static void Add(WBVHNodeAdapter<T> adapter, WBVHNode<T> curNode, T newObj, ref AABBF newBox,
            float newSAH)
        {
            // 1. 遍历节点找到最好的叶子节点
            while (curNode._objects == null)
            {
                var lNode = curNode.left;
                var rNode = curNode.right;

                AABBF leftExpand = new AABBF(lNode.box.Center, lNode.box.Size);
                AABBF rightExpand = new AABBF(rNode.box.Center, rNode.box.Size);
                leftExpand.Encapsulate(newBox);
                rightExpand.Encapsulate(newBox);
                AABBF leftAndRightPair = AABBofPair(lNode, rNode);

                // 1. send to left node (L+N, R)
                // 2. send to right node (L, R+N)
                // 3. merge and pushDown left-and-right node(L+R, N)
                float sendLeftSAH = SA(ref leftExpand) + SA(rNode);
                float sendRightSAH = SA(lNode) + SA(ref rightExpand);
                float mergedLeftAndRightSAH = SA(ref leftAndRightPair) + newSAH;

                const float MERGE_DISCOUNT = 0.3f;

                if (mergedLeftAndRightSAH < (math.min(sendLeftSAH, sendRightSAH) * MERGE_DISCOUNT))
                {
                    AddObjectPushDown(adapter, curNode, newObj);
                    return;
                }
                else
                {
                    curNode = (sendLeftSAH < sendRightSAH ? lNode : rNode);
                }

            }

            // 2. 接下来加入物体并映射到叶子结点
            curNode.Objects.Add(newObj);
            adapter.MapObj2BVHLeaf(newObj, curNode);
            curNode.RefitVolume(adapter);
            curNode.SplitIfNecessary(adapter);
        }

        internal void Remove(WBVHNodeAdapter<T> adapter, T obj)
        {
            if(_objects == null)
                throw new Exception("removeObject() called on nonLeaf!");
            
            adapter.UnmapObject(obj);
            _objects.Remove(obj);

            if (_objects.Count > 0)
                RefitVolume(adapter);
            else
            {
                if (parent != null)
                {
                    _objects = null;
                    parent.RemoveLeaf(adapter, this);
                    parent = null;
                }
            }
        }

        internal void RemoveLeaf(WBVHNodeAdapter<T> adapter, WBVHNode<T> removeLeft)
        {
            if(left == null || right == null)
                throw new Exception("bad intermediate node");

            WBVHNode<T> keepLeaf;

            if (removeLeft == left)
                keepLeaf = right;
            else if (removeLeft == right)
                keepLeaf = left;
            else
                throw new Exception("removeLeaf doesn't match any leaf!");

            // todo 考虑把这个当前节点更新的操作合并
            box = keepLeaf.box;
            left = keepLeaf.left;
            right = keepLeaf.right;
            _objects = keepLeaf._objects;

            if (_objects == null)
            {
                left.parent = this;
                right.parent = this;
                this.SetDepth(adapter, this.depth);
            }
            else
            {
                _objects.ForEach(o => adapter.MapObj2BVHLeaf(o, this));
            }

            if (parent != null)
            {
                parent.ChildRefit(adapter);
            }
        }

        internal class SplitAxisOpt<GO> : IComparable<SplitAxisOpt<GO>>
        {
            public float SAH;
            public Axis axis;
            public List<GO> left, right;

            internal SplitAxisOpt(float SAH, Axis axis, List<GO> left, List<GO> right)
            {
                this.SAH = SAH;
                this.axis = axis;
                this.left = left;
                this.right = right;
            }

            public int CompareTo(SplitAxisOpt<GO> other) => SAH.CompareTo(other.SAH);
        }

        internal void SplitIfNecessary(WBVHNodeAdapter<T> adapter)
        {
            if (_objects.Count > adapter.BVH.LEAF_OBJ_MAX)
                SplitNode(adapter);
        }

        internal void SplitNode(WBVHNodeAdapter<T> adapter)
        {
            List<T> splitList = _objects;
            // if (_objects.Count <= 1)
            // {
            //     Debug.LogError("有问题");
            //     return;
            // }
            splitList.ForEach(adapter.UnmapObject);
            int center = (int)(splitList.Count * 0.5f);

            SplitAxisOpt<T> bestSplit = EachAxis.Min((axis) =>
            {
                var orderedlist = new List<T>(splitList);
                switch (axis)
                {
                    case Axis.X:
                        orderedlist.Sort((go1, go2) =>
                            adapter.GetObjectPos(go1).x.CompareTo(adapter.GetObjectPos(go2).x));
                        break;
                    case Axis.Y:
                        orderedlist.Sort((go1, go2) =>
                            adapter.GetObjectPos(go1).y.CompareTo(adapter.GetObjectPos(go2).y));
                        break;
                    case Axis.Z:
                        orderedlist.Sort((go1, go2) =>
                            adapter.GetObjectPos(go1).z.CompareTo(adapter.GetObjectPos(go2).z));
                        break;
                    default:
                        throw new NotImplementedException("unknown split axis: " + axis.ToString());
                }

                var left_s = orderedlist.GetRange(0, center);
                var right_s = orderedlist.GetRange(center, splitList.Count - center);

                float SAH = SAofList(adapter, left_s) * left_s.Count + SAofList(adapter, right_s) * right_s.Count;
                return new SplitAxisOpt<T>(SAH, axis, left_s, right_s);
            });

            // 分割
            _objects = null;
            // 左边分割
            this.left = new WBVHNode<T>(adapter.BVH, this, bestSplit.left, bestSplit.axis, this.depth + 1);
            // 右边分割
            this.right = new WBVHNode<T>(adapter.BVH, this, bestSplit.right, bestSplit.axis, this.depth + 1);

        }

        internal float SAofList(WBVHNodeAdapter<T> adapter, List<T> list)
        {
            // todo 这里AABB的中点都是零点，不知道有没有问题
            if (list.Count <= 0)
                Debug.LogError("VAR");
            var box = new AABBF(float3.zero, new float3(adapter.GetRadius(list[0])));
            
            list.ToList<T>().GetRange(1, list.Count - 1).ForEach(obj =>
            {
                var newBox = new AABBF(float3.zero, new float3(adapter.GetRadius(obj)));
                box.Encapsulate(newBox);
            });

            return SA(box);
        }

        /// <summary>
        /// 加入物体并下降
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="curNode"></param>
        /// <param name="newObj"></param>
        internal static void AddObjectPushDown(WBVHNodeAdapter<T> adapter, WBVHNode<T> curNode, T newObj)
        {
            var lNode = curNode.left;
            var rNode = curNode.right;

            // merge and pushDown left and right as a new node
            var mergedSubNode = new WBVHNode<T>(adapter.BVH, lNode, rNode, curNode, null);
            mergedSubNode.ChildRefit(adapter, propagate: false);
            
            // make new subNode for obj
            var newSubNode = new WBVHNode<T>(adapter.BVH)
            {
                parent = curNode,
                Objects = new List<T>() { newObj }
            };
            adapter.MapObj2BVHLeaf(newObj, newSubNode);
            newSubNode.ComputeVolume(adapter);
            
            // make assignments
            curNode.left = mergedSubNode;
            curNode.right = newSubNode;
            curNode.SetDepth(adapter, curNode.depth);
            curNode.ChildRefit(adapter);
        }

        private static AABBF AABBofPair(WBVHNode<T> nodeA, WBVHNode<T> nodeB)
        {
            // var box = nodeA.box;
            var box = new AABBF(nodeA.box.Center, nodeA.box.Size);
            box.Encapsulate(nodeB.box);
            return box;
        }
        
        internal void TryRotate(WBVH<T> bvh)
        {
            WBVHNodeAdapter<T> adapter = bvh.adapter;

            // 把叶节点的祖父加入, 因为它们的子节点一定不是叶节点
            if (left.IsLeaf && right.IsLeaf && parent != null)
            {
                bvh.refitNodes.Add(parent);
                return;
            }

            // SAH, 左右相加，不考虑有交集的情况
            float mySA = SA(left) + SA(right);

            // 把每个方向的rotate都试一次
            RotOpt bestRot = EachRot.Min((rot) =>
            {
                switch (rot)
                {
                    case Rot.NONE: return new RotOpt(mySA, Rot.NONE);
                    // child to grandchild rotations
                    case Rot.L_RL:
                        if (right.IsLeaf) return new RotOpt(float.MaxValue, Rot.NONE);
                        else return new RotOpt(SA(right.left) + SA(AABBofPair(left, right.right)), rot);
                    case Rot.L_RR:
                        if (right.IsLeaf) return new RotOpt(float.MaxValue, Rot.NONE);
                        else return new RotOpt(SA(right.right) + SA(AABBofPair(left, right.left)), rot);
                    case Rot.R_LL:
                        if (left.IsLeaf) return new RotOpt(float.MaxValue, Rot.NONE);
                        else return new RotOpt(SA(AABBofPair(right, left.right)) + SA(left.left), rot);
                    case Rot.R_LR:
                        if (left.IsLeaf) return new RotOpt(float.MaxValue, Rot.NONE);
                        else return new RotOpt(SA(AABBofPair(right, left.left)) + SA(left.right), rot);
                    // grandchild to grandchild rotations
                    case Rot.LL_RR:
                        if (left.IsLeaf || right.IsLeaf) return new RotOpt(float.MaxValue, Rot.NONE);
                        else
                            return new RotOpt(
                                SA(AABBofPair(right.right, left.right)) + SA(AABBofPair(right.left, left.left)), rot);
                    case Rot.LL_RL:
                        if (left.IsLeaf || right.IsLeaf) return new RotOpt(float.MaxValue, Rot.NONE);
                        else
                            return new RotOpt(
                                SA(AABBofPair(right.left, left.right)) + SA(AABBofPair(left.left, right.right)), rot);
                    // unknown...
                    default:
                        throw new NotImplementedException(
                            "missing implementation for BVH Rotation SAH Computation .. " + rot.ToString());
                }
            });

            if (bestRot.rot == Rot.NONE)
            {
                if (parent != null)
                {
                    // 限制一下refit次数
                    if (Random.Range(0, 100) < 2)
                    {
                        bvh.refitNodes.Add(parent);
                    }
                }
            }
            else
            {
                if (parent != null) bvh.refitNodes.Add(parent);

                // 看优化了多少，以此判断是否需要旋转
                if ((mySA - bestRot.SAH) < mySA * 0.1f)
                    return;
                // UnityEngine.Debug.Log(String.Format("BVH swap {0} from {1} to {2}", bestRot.rot.ToString(), mySA,
                    // bestRot.SAH));

                // in order to swap we need to:
                //  1. swap the node locations
                //  2. update the depth (if child-to-grandchild)
                //  3. update the parent pointers
                //  4. refit the boundary box
                WBVHNode<T> swap = null;
                switch (bestRot.rot)
                {
                    case Rot.NONE: break;
                    // child to grandchild rotations
                    case Rot.L_RL:
                        swap = left;
                        left = right.left;
                        left.parent = this;
                        right.left = swap;
                        swap.parent = right;
                        right.ChildRefit(adapter, propagate: false);
                        break;
                    case Rot.L_RR:
                        swap = left;
                        left = right.right;
                        left.parent = this;
                        right.right = swap;
                        swap.parent = right;
                        right.ChildRefit(adapter, propagate: false);
                        break;
                    case Rot.R_LL:
                        swap = right;
                        right = left.left;
                        right.parent = this;
                        left.left = swap;
                        swap.parent = left;
                        left.ChildRefit(adapter, propagate: false);
                        break;
                    case Rot.R_LR:
                        swap = right;
                        right = left.right;
                        right.parent = this;
                        left.right = swap;
                        swap.parent = left;
                        left.ChildRefit(adapter, propagate: false);
                        
                        break;

                    // grandchild to grandchild rotations
                    case Rot.LL_RR:
                        swap = left.left;
                        left.left = right.right;
                        right.right = swap;
                        left.left.parent = left;
                        swap.parent = right;
                        left.ChildRefit(adapter, propagate: false);
                        right.ChildRefit(adapter, propagate: false);
                        break;
                    case Rot.LL_RL:
                        swap = left.left;
                        left.left = right.left;
                        right.left = swap;
                        left.left.parent = left;
                        swap.parent = right;
                        left.ChildRefit(adapter, propagate: false);
                        right.ChildRefit(adapter, propagate: false);
                        break;

                    // unknown...
                    default:
                        throw new NotImplementedException("missing implementation for BVH Rotation .. " +
                                                          bestRot.rot.ToString());
                }
                
                switch (bestRot.rot)
                {
                    case Rot.L_RL:
                    case Rot.L_RR:
                    case Rot.R_LL:
                    case Rot.R_LR:
                        this.SetDepth(adapter, this.depth);
                        break;
                }
                
            }
        }

        void SetDepth(WBVHNodeAdapter<T> adapter, int newDepth = -999)
        {
            this.depth = newDepth;

            if (newDepth > adapter.BVH.maxDepth)
                adapter.BVH.maxDepth = newDepth;

            if (_objects == null)
            {
                left.SetDepth(adapter, newDepth + 1);
                right.SetDepth(adapter, newDepth + 1);
            }
        }

        private void ChildRefit(WBVHNodeAdapter<T> adapter, bool propagate = true)
        {
            ChildRefit(adapter, this, propagate: propagate);
        }

        private static void ChildRefit(WBVHNodeAdapter<T> adapter, WBVHNode<T> curNode, bool propagate = true)
        {
            do
            {
                // AABBF oldbox = curNode.box;
                WBVHNode<T> left = curNode.left;
                WBVHNode<T> right = curNode.right;

                // start with the left box
                AABBF newBox = left.box;

                newBox.Encapsulate(right.box);

                // now set our box to the newly created box
                curNode.box = newBox;

                // and walk up the tree
                curNode = curNode.parent;
            } while (propagate && curNode != null);
        }

        internal void DrawAllBounds(float depth = 0)
        {
            if (depth <= 4)
            {
                float tintVal = depth / 4;
                Gizmos.color = new Color(1, tintVal,0);
            }
            else if (depth <= 8)
            {
                float tintVal = (depth - 4) / 4;
                Gizmos.color = new Color(1 - tintVal, 1.0f, 0);
            }
            else
            {
                float tintVal = (depth - 8) / 4;
                Gizmos.color = new Color(0, 1.0f, tintVal);
            }

            float3 center = this.box.Center;
            float3 size = this.box.Size;
            Bounds thisBounds = new Bounds(new Vector3(center.x, center.y, center.z),
                new Vector3(size.x, size.y, size.z));
            Gizmos.DrawWireCube(thisBounds.center, thisBounds.size);

            depth++;
            if (left != null)
                left.DrawAllBounds(depth);
            if (right != null)
                right.DrawAllBounds(depth);

            Gizmos.color = Color.white;
        }

    }
}