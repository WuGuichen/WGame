using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TWY.Physics
{
    public class WPointOctreeNode<T>
    {
        public float3 Center { get; private set; }

        public float SideLength { get; private set; }

        private float minSize;

        private AABBF bounds = default(AABBF);

        private readonly List<WOctreeObject> _objects = new List<WOctreeObject>();

        private WPointOctreeNode<T>[] children = null;

        bool HasChildren
        {
            get { return children != null; }
        }

        private AABBF[] childBounds;

        // If there are already NUM_OBJECTS_ALLOWED in a node, we split it into children
        // A generally good number seems to be something around 8-15
        const int NUM_OBJECTS_ALLOWED = 8;

        // For reverting the bounds size after temporary changes
        float3 actualBoundsSize;

        // An object in the octree
        class WOctreeObject
        {
            public T Obj;
            public float3 Pos;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseLengthVal">Length of this node, not taking looseness into account.</param>
        /// <param name="minSizeVal">Minimum size of nodes in this octree.</param>
        /// <param name="centerVal">Centre position of this node.</param>
        public WPointOctreeNode(float baseLengthVal, float minSizeVal, float3 centerVal)
        {
            SetValues(baseLengthVal, minSizeVal, centerVal);
        }

        // #### PUBLIC METHODS ####

        /// <summary>
        /// Add an object.
        /// </summary>
        /// <param name="obj">Object to add.</param>
        /// <param name="objPos">Position of the object.</param>
        /// <returns></returns>
        public bool Add(T obj, float3 objPos)
        {
            if (!Encapsulates(bounds, objPos))
            {
                return false;
            }

            SubAdd(obj, objPos);
            return true;
        }

        /// <summary>
        /// Remove an object. Makes the assumption that the object only exists once in the tree.
        /// </summary>
        /// <param name="obj">Object to remove.</param>
        /// <returns>True if the object was removed successfully.</returns>
        public bool Remove(T obj)
        {
            bool removed = false;

            for (int i = 0; i < _objects.Count; i++)
            {
                if (_objects[i].Obj.Equals(obj))
                {
                    removed = _objects.Remove(_objects[i]);
                    break;
                }
            }

            if (!removed && children != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    removed = children[i].Remove(obj);
                    if (removed) break;
                }
            }

            if (removed && children != null)
            {
                // Check if we should merge nodes now that we've removed an item
                if (ShouldMerge())
                {
                    Merge();
                }
            }

            return removed;
        }

        /// <summary>
        /// Removes the specified object at the given position. Makes the assumption that the object only exists once in the tree.
        /// </summary>
        /// <param name="obj">Object to remove.</param>
        /// <param name="objPos">Position of the object.</param>
        /// <returns>True if the object was removed successfully.</returns>
        public bool Remove(T obj, float3 objPos)
        {
            if (!Encapsulates(bounds, objPos))
            {
                return false;
            }

            return SubRemove(obj, objPos);
        }

        public void GetNearby(ref RayF ray, float maxDistance, List<T> result)
        {
            // Does the ray hit this node at all?
            // Note: Expanding the bounds is not exactly the same as a real distance check, but it's fast.
            // TODO: Does someone have a fast AND accurate formula to do this check?
            bounds.Expand(maxDistance);
            bool intersected = bounds.IntersectRay(ray);
            bounds.Size = actualBoundsSize;
            if (!intersected)
            {
                return;
            }

            // Check against any _objects in this node
            for (int i = 0; i < _objects.Count; i++)
            {
                if (SqrDistanceToRay(ray, _objects[i].Pos) <= (maxDistance * maxDistance))
                {
                    result.Add(_objects[i].Obj);
                }
            }

            // Check children
            if (children != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    children[i].GetNearby(ref ray, maxDistance, result);
                }
            }
        }

        /// <summary>
        /// Return _objects that are within <paramref name="maxDistance"/> of the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="maxDistance">Maximum distance from the position to consider.</param>
        /// <param name="result">List result.</param>
        /// <returns>Objects within range.</returns>
        public void GetNearby(ref float3 position, float maxDistance, List<T> result)
        {
            float sqrMaxDistance = maxDistance * maxDistance;

#if UNITY_2017_1_OR_NEWER
            // Does the node intersect with the sphere of center = position and radius = maxDistance?
            if (bounds.ClosestPointSqDist(position) > sqrMaxDistance)
            {
                return;
            }
#else
		// Does the ray hit this node at all?
		// Note: Expanding the bounds is not exactly the same as a real distance check, but it's fast
		// TODO: Does someone have a fast AND accurate formula to do this check?
		bounds.Expand(new float3(maxDistance * 2, maxDistance * 2, maxDistance * 2));
		bool contained = bounds.Contains(position);
		bounds.size = actualBoundsSize;
		if (!contained) {
			return;
		}
#endif

            // Check against any _objects in this node
            for (int i = 0; i < _objects.Count; i++)
            {
                if (math.lengthsq(position - _objects[i].Pos) <= sqrMaxDistance)
                {
                    result.Add(_objects[i].Obj);
                }
            }

            // Check children
            if (children != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    children[i].GetNearby(ref position, maxDistance, result);
                }
            }
        }

        /// <summary>
        /// Return all _objects in the tree.
        /// </summary>
        /// <returns>All _objects.</returns>
        public void GetAll(List<T> result)
        {
            // add directly contained_objects 
            result.AddRange(_objects.Select(o => o.Obj));

            // add children_objects 
            if (children != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    children[i].GetAll(result);
                }
            }
        }

        /// <summary>
        /// Set the 8 children of this octree.
        /// </summary>
        /// <param name="childOctrees">The 8 new child nodes.</param>
        public void SetChildren(WPointOctreeNode<T>[] childOctrees)
        {
            if (childOctrees.Length != 8)
            {
                Debug.LogError("Child octree array must be length 8. Was length: " + childOctrees.Length);
                return;
            }

            children = childOctrees;
        }

        /// <summary>
        /// Draws node boundaries visually for debugging.
        /// Must be called from OnDrawGizmos externally. See also: DrawAllObjects.
        /// </summary>
        /// <param name="depth">Used for recurcive calls to this method.</param>
        public void DrawAllBounds(float depth = 0)
        {
            float tintVal = depth / 7; // Will eventually get values > 1. Color rounds to 1 automatically
            Gizmos.color = new Color(tintVal, 0, 1.0f - tintVal);

            AABBF thisBounds = new AABBF(Center, new float3(SideLength, SideLength, SideLength));
            Gizmos.DrawWireCube(Convert2Vector3(thisBounds.Center), Convert2Vector3(thisBounds.Size));

            if (children != null)
            {
                depth++;
                for (int i = 0; i < 8; i++)
                {
                    children[i].DrawAllBounds(depth);
                }
            }

            Gizmos.color = Color.white;
        }

        /// <summary>
        /// Draws the bounds of all _objects in the tree visually for debugging.
        /// Must be called from OnDrawGizmos externally. See also: DrawAllBounds.
        /// NOTE: marker.tif must be placed in your Unity /Assets/Gizmos subfolder for this to work.
        /// </summary>
        public void DrawAllObjects()
        {
            float tintVal = SideLength / 20;
            Gizmos.color = new Color(0, 1.0f - tintVal, tintVal, 0.25f);

            foreach (WOctreeObject obj in _objects)
            {
                // Gizmos.DrawIcon(obj.Pos, "marker.tif", true);
                Gizmos.DrawSphere(Convert2Vector3(obj.Pos), 2);
            }

            if (children != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    children[i].DrawAllObjects();
                }
            }

            Gizmos.color = Color.white;
        }

        /// <summary>
        /// We can shrink the octree if:
        /// - This node is >= double minLength in length
        /// - All _objects in the root node are within one octant
        /// - This node doesn't have children, or does but 7/8 children are empty
        /// We can also shrink it if there are no _objects left at all!
        /// </summary>
        /// <param name="minLength">Minimum dimensions of a node in this octree.</param>
        /// <returns>The new root, or the existing one if we didn't shrink.</returns>
        public WPointOctreeNode<T> ShrinkIfPossible(float minLength)
        {
            if (SideLength < (2 * minLength))
            {
                return this;
            }

            if (_objects.Count == 0 && (children == null || children.Length == 0))
            {
                return this;
            }

            // Check _objects in root
            int bestFit = -1;
            for (int i = 0; i < _objects.Count; i++)
            {
                WOctreeObject curObj = _objects[i];
                int newBestFit = BestFitChild(curObj.Pos);
                if (i == 0 || newBestFit == bestFit)
                {
                    if (bestFit < 0)
                    {
                        bestFit = newBestFit;
                    }
                }
                else
                {
                    return this; // Can't reduce - _objects fit in different octants
                }
            }

            // Check _objects in children if there are any
            if (children != null)
            {
                bool childHadContent = false;
                for (int i = 0; i < children.Length; i++)
                {
                    if (children[i].HasAnyObjects())
                    {
                        if (childHadContent)
                        {
                            return this; // Can't shrink - another child had content already
                        }

                        if (bestFit >= 0 && bestFit != i)
                        {
                            return
                                this; // Can't reduce - _objects in root are in a different octant to _objects in child
                        }

                        childHadContent = true;
                        bestFit = i;
                    }
                }
            }

            // Can reduce
            if (children == null)
            {
                // We don't have any children, so just shrink this node to the new size
                // We already know that everything will still fit in it
                SetValues(SideLength / 2, minSize, childBounds[bestFit].Center);
                return this;
            }

            // We have children. Use the appropriate child as the new root node
            return children[bestFit];
        }

        /// <summary>
        /// Find which child node this object would be most likely to fit in.
        /// </summary>
        /// <param name="objPos">The object's position.</param>
        /// <returns>One of the eight child octants.</returns>
        public int BestFitChild(float3 objPos)
        {
            return (objPos.x <= Center.x ? 0 : 1) + (objPos.y >= Center.y ? 0 : 4) + (objPos.z <= Center.z ? 0 : 2);
        }

        /// <summary>
        /// Checks if this node or anything below it has something in it.
        /// </summary>
        /// <returns>True if this node or any of its children, grandchildren etc have something in them</returns>
        public bool HasAnyObjects()
        {
            if (_objects.Count > 0) return true;

            if (children != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (children[i].HasAnyObjects()) return true;
                }
            }

            return false;
        }

        /*
        /// <summary>
        /// Get the total amount of objects in this node and all its children, grandchildren etc. Useful for debugging.
        /// </summary>
        /// <param name="startingNum">Used by recursive calls to add to the previous total.</param>
        /// <returns>Total objects in this node and its children, grandchildren etc.</returns>
        public int GetTotalObjects(int startingNum = 0) {
            int totalObjects = startingNum + objects.Count;
            if (children != null) {
                for (int i = 0; i < 8; i++) {
                    totalObjects += children[i].GetTotalObjects();
                }
            }
            return totalObjects;
        }
        */

        // #### PRIVATE METHODS ####

        /// <summary>
        /// Set values for this node. 
        /// </summary>
        /// <param name="baseLengthVal">Length of this node, not taking looseness into account.</param>
        /// <param name="minSizeVal">Minimum size of nodes in this octree.</param>
        /// <param name="centerVal">Centre position of this node.</param>
        void SetValues(float baseLengthVal, float minSizeVal, float3 centerVal)
        {
            SideLength = baseLengthVal;
            minSize = minSizeVal;
            Center = centerVal;

            // Create the bounding box.
            actualBoundsSize = new float3(SideLength, SideLength, SideLength);
            bounds = new AABBF(Center, actualBoundsSize);

            float quarter = SideLength / 4f;
            float childActualLength = SideLength / 2;
            float3 childActualSize = new float3(childActualLength, childActualLength, childActualLength);
            childBounds = new AABBF[8];
            childBounds[0] = new AABBF(Center + new float3(-quarter, quarter, -quarter), childActualSize);
            childBounds[1] = new AABBF(Center + new float3(quarter, quarter, -quarter), childActualSize);
            childBounds[2] = new AABBF(Center + new float3(-quarter, quarter, quarter), childActualSize);
            childBounds[3] = new AABBF(Center + new float3(quarter, quarter, quarter), childActualSize);
            childBounds[4] = new AABBF(Center + new float3(-quarter, -quarter, -quarter), childActualSize);
            childBounds[5] = new AABBF(Center + new float3(quarter, -quarter, -quarter), childActualSize);
            childBounds[6] = new AABBF(Center + new float3(-quarter, -quarter, quarter), childActualSize);
            childBounds[7] = new AABBF(Center + new float3(quarter, -quarter, quarter), childActualSize);
        }

        /// <summary>
        /// Private counterpart to the public Add method.
        /// </summary>
        /// <param name="obj">Object to add.</param>
        /// <param name="objPos">Position of the object.</param>
        void SubAdd(T obj, float3 objPos)
        {
            // We know it fits at this level if we've got this far

            // We always put things in the deepest possible child
            // So we can skip checks and simply move down if there are children aleady
            if (!HasChildren)
            {
                // Just add if few _objects are here, or children would be below min size
                if (_objects.Count < NUM_OBJECTS_ALLOWED || (SideLength / 2) < minSize)
                {
                    WOctreeObject newObj = new WOctreeObject { Obj = obj, Pos = objPos };
                    _objects.Add(newObj);
                    return; // We're done. No children yet
                }

                // Enough _objects in this node already: Create the 8 children
                int bestFitChild;
                if (children == null)
                {
                    Split();
                    if (children == null)
                    {
                        Debug.LogError("Child creation failed for an unknown reason. Early exit.");
                        return;
                    }

                    // Now that we have the new children, move this node's existing _objects into them
                    for (int i = _objects.Count - 1; i >= 0; i--)
                    {
                        WOctreeObject existingObj = _objects[i];
                        // Find which child the object is closest to based on where the
                        // object's center is located in relation to the octree's center
                        bestFitChild = BestFitChild(existingObj.Pos);
                        children[bestFitChild].SubAdd(existingObj.Obj, existingObj.Pos); // Go a level deeper					
                        _objects.Remove(existingObj); // Remove from here
                    }
                }
            }

            // Handle the new object we're adding now
            int bestFit = BestFitChild(objPos);
            children[bestFit].SubAdd(obj, objPos);
        }

        /// <summary>
        /// Private counterpart to the public <see cref="Remove(T, float3)"/> method.
        /// </summary>
        /// <param name="obj">Object to remove.</param>
        /// <param name="objPos">Position of the object.</param>
        /// <returns>True if the object was removed successfully.</returns>
        bool SubRemove(T obj, float3 objPos)
        {
            bool removed = false;

            for (int i = 0; i < _objects.Count; i++)
            {
                if (_objects[i].Obj.Equals(obj))
                {
                    removed = _objects.Remove(_objects[i]);
                    break;
                }
            }

            if (!removed && children != null)
            {
                int bestFitChild = BestFitChild(objPos);
                removed = children[bestFitChild].SubRemove(obj, objPos);
            }

            if (removed && children != null)
            {
                // Check if we should merge nodes now that we've removed an item
                if (ShouldMerge())
                {
                    Merge();
                }
            }

            return removed;
        }

        /// <summary>
        /// Splits the octree into eight children.
        /// </summary>
        void Split()
        {
            float quarter = SideLength / 4f;
            float newLength = SideLength / 2;
            children = new WPointOctreeNode<T>[8];
            children[0] = new WPointOctreeNode<T>(newLength, minSize, Center + new float3(-quarter, quarter, -quarter));
            children[1] = new WPointOctreeNode<T>(newLength, minSize, Center + new float3(quarter, quarter, -quarter));
            children[2] = new WPointOctreeNode<T>(newLength, minSize, Center + new float3(-quarter, quarter, quarter));
            children[3] = new WPointOctreeNode<T>(newLength, minSize, Center + new float3(quarter, quarter, quarter));
            children[4] =
                new WPointOctreeNode<T>(newLength, minSize, Center + new float3(-quarter, -quarter, -quarter));
            children[5] = new WPointOctreeNode<T>(newLength, minSize, Center + new float3(quarter, -quarter, -quarter));
            children[6] = new WPointOctreeNode<T>(newLength, minSize, Center + new float3(-quarter, -quarter, quarter));
            children[7] = new WPointOctreeNode<T>(newLength, minSize, Center + new float3(quarter, -quarter, quarter));
        }

        /// <summary>
        /// Merge all children into this node - the opposite of Split.
        /// Note: We only have to check one level down since a merge will never happen if the children already have children,
        /// since THAT won't happen unless there are already too many _objects to merge.
        /// </summary>
        void Merge()
        {
            // Note: We know children != null or we wouldn't be merging
            for (int i = 0; i < 8; i++)
            {
                WPointOctreeNode<T> curChild = children[i];
                int numObjects = curChild._objects.Count;
                for (int j = numObjects - 1; j >= 0; j--)
                {
                    WOctreeObject curObj = curChild._objects[j];
                    _objects.Add(curObj);
                }
            }

            // Remove the child nodes (and the _objects in them - they've been added elsewhere now)
            children = null;
        }

        /// <summary>
        /// Checks if outerBounds encapsulates the given point.
        /// </summary>
        /// <param name="outerBounds">Outer bounds.</param>
        /// <param name="point">Point.</param>
        /// <returns>True if innerBounds is fully encapsulated by outerBounds.</returns>
        static bool Encapsulates(AABBF outerBounds, float3 point)
        {
            return outerBounds.Contains(point);
        }

        /// <summary>
        /// Checks if there are few enough _objects in this node and its children that the children should all be merged into this.
        /// </summary>
        /// <returns>True there are less or the same abount of objects in this and its children than numObjectsAllowed.</returns>
        bool ShouldMerge()
        {
            int totalObjects = _objects.Count;
            if (children != null)
            {
                foreach (WPointOctreeNode<T> child in children)
                {
                    if (child.children != null)
                    {
                        // If any of the *children* have children, there are definitely too many to merge,
                        // or the child woudl have been merged already
                        return false;
                    }

                    totalObjects += child._objects.Count;
                }
            }

            return totalObjects <= NUM_OBJECTS_ALLOWED;
        }

        /// <summary>
        /// Returns the closest distance to the given ray from a point.
        /// </summary>
        /// <param name="ray">The ray.</param>
        /// <param name="point">The point to check distance from the ray.</param>
        /// <returns>Squared distance from the point to the closest point of the ray.</returns>
        public static float SqrDistanceToRay(RayF ray, float3 point)
        {
            float3 d = math.cross(ray.direction, point - ray.origin);
            return math.dot(d, d);
        }

        Vector3 Convert2Vector3(float3 v) => new Vector3(v.x, v.y, v.z);
    }
}