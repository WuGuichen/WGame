using System.Collections;
using System.Collections.Generic;
using TWY.Physics;
using Unity.Mathematics;
using UnityEngine;

public class WBVHEntityAdapter : WBVH<int>
{
    public WBVH<int> BVH { get; set; }
    private Dictionary<int, WBVHNode<IGameViewService>> entityIdToLeafMap = new();
    public WBVHNode<IGameViewService> GetLeaf(int instID) => entityIdToLeafMap[instID];

    public WBVHEntityAdapter(WBVHNodeAdapter<int> nodeAdapter, List<int> objs, int leafObjMax = 1) : base(nodeAdapter, objs, leafObjMax)
    {
    }
}
