using System.Collections;
using System.Collections.Generic;
using TWY.Physics;
using Unity.Mathematics;
using UnityEngine;

public class WBVHEntityAdapter : WBVHNodeAdapter<IGameViewService>
{
    public WBVH<IGameViewService> BVH { get; set; }

    private Dictionary<int, WBVHNode<IGameViewService>> idToLeafMap = new();
    public float3 GetObjectPos(IGameViewService obj)
    {
        var pos = obj.Position;
        return new float3(pos.x, pos.y, pos.z);
    }

    public float GetRadius(IGameViewService obj)
    {
        return obj.Height;
    }

    public void MapObj2BVHLeaf(IGameViewService obj, WBVHNode<IGameViewService> leaf)
    {
        idToLeafMap[obj.InstanceID] = leaf;
    }

    public void OnPositionOrSizeChanged(IGameViewService changed)
    {
        idToLeafMap[changed.InstanceID].RefitObjectChanged(this, changed);
    }

    public void UnmapObject(IGameViewService obj)
    {
        idToLeafMap.Remove(obj.InstanceID);
    }

    public void CheckMap(IGameViewService obj)
    {
        if(!idToLeafMap.ContainsKey(obj.InstanceID))
            throw new System.Exception("missing map for shuffled child");
    }

    public WBVHNode<IGameViewService> GetLeaf(IGameViewService obj)
    {
        return idToLeafMap[obj.InstanceID];
    }
}
