using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TWY.Physics
{
    public interface WBVHNodeAdapter<T>
    {
        WBVH<T> BVH { get; set; }
        float3 GetObjectPos(T obj);
        float GetRadius(T obj);
        void MapObj2BVHLeaf(T obj, WBVHNode<T> leaf);
        void OnPositionOrSizeChanged(T changed);
        void UnmapObject(T obj);
        void CheckMap(T obj);
        WBVHNode<T> GetLeaf(T obj);
        void Optimize();
    }
}