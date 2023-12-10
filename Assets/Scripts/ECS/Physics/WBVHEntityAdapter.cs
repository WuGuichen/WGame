using System.Collections;
using System.Collections.Generic;
using TWY.Physics;
using Unity.Mathematics;
using UnityEngine;

public class WBVHEntityAdapter : WBVHNodeAdapter<IGameViewService>
{
    public WBVH<IGameViewService> BVH { get; set; }

    private Dictionary<int, WBVHNode<IGameViewService>> idToLeafMap = new();

    private List<WBVHNode<IGameViewService>>[] bucket = new List<WBVHNode<IGameViewService>>[16];

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

    public void Optimize()
    {
        var entities = EntityUtils.GetGameEntities();
        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            if (entity.isEnabled && entity.hasGameViewService)
            {
                var node = GetLeaf(entity.gameViewService.service);
                if (node.needRefit)
                {
                    var list = bucket[node.depth];
                    if (list == null)
                        list = new List<WBVHNode<IGameViewService>>();
                    list.Add(node);
                    bucket[node.depth] = list;
                }
            }
        }

        for (int i = BVH.maxDepth-1; i >= 0; i--)
        {
            var data = bucket[i];
            if(data != null)
            {for (int j = 0; j < data.Count; j++)
            {
                data[j].TryRotate(BVH);
            }
            }
        }
    }
}
