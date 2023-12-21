// using System.Collections;
// using System.Collections.Generic;
// using Unity.Mathematics;
// using UnityEngine;
//
// namespace TWY.Physics
// {
//     public class WBVHGameObjectAdapter : WBVHNodeAdapter<GameObject>
//     {
//         public WBVH<GameObject> BVH { get; set; }
//
//         private Dictionary<GameObject, WBVHNode<GameObject>> gameObjectToLeafMap = new();
//
//         public float3 GetObjectPos(GameObject obj)
//         {
//             Vector3 pos = obj.transform.position;
//             return new float3(pos.x, pos.y, pos.z);
//         }
//
//         public HitInfo GetHitInfo(GameObject obj, float sqrDist)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public float GetRadius(GameObject obj)
//         {
//             AABBF bounds = GetBounds(obj);
//             return math.max(bounds.Radius.x, math.max(bounds.Radius.y, bounds.Radius.z));
//         }
//
//         public float3 GetSize(GameObject obj)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public AABBF GetBounds(GameObject obj)
//         {
//             Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
//             AABBF bounds = new AABBF(GetObjectPos(obj), float3.zero);
//             for (int i = 0; i < renderers.Length; i++)
//             {
//                 var b = renderers[i].bounds;
//                 bounds.Encapsulate(new AABBF(b.center, b.size));
//             }
//
//             return bounds;
//         }
//
//         public void MapObj2BVHLeaf(GameObject obj, WBVHNode<GameObject> leaf) => gameObjectToLeafMap[obj] = leaf;
//
//         public void OnPositionOrSizeChanged(GameObject changed) => gameObjectToLeafMap[changed].RefitObjectChanged(this, changed);
//
//         public void UnmapObject(GameObject obj) => gameObjectToLeafMap.Remove(obj);
//
//         public void CheckMap(GameObject obj)
//         {
//             if (!gameObjectToLeafMap.ContainsKey(obj))
//                 throw new System.Exception("missing map for shuffled child");
//         }
//
//         public WBVHNode<GameObject> GetLeaf(GameObject obj) => gameObjectToLeafMap[obj];
//         public void Optimize()
//         {
//             for (int i = BVH.maxDepth; i >= 0; i--)
//             {
//                 
//             }
//         }
//     }
// }