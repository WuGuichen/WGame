using System.Collections;
using System.Collections.Generic;
using BaseData.Character;
using Entitas;
using TWY.Physics;
using Unity.Mathematics;

public class WBVHEntityAdapter : WBVHNodeAdapter<IGameViewService>
{
    public WBVH<IGameViewService> BVH { get; set; }

    private Dictionary<int, WBVHNode<IGameViewService>> idToLeafMap = new();

    private List<WBVHNode<IGameViewService>>[] bucket = new List<WBVHNode<IGameViewService>>[16];

    private readonly IGroup<GameEntity> _entityGroup;

    public WBVHEntityAdapter(Camp camp)
    {
        if (camp == Camp.Red)
        {
            _entityGroup = Contexts.sharedInstance.game.GetGroup(GameMatcher.CampRed);
        }
    }

    public float3 GetObjectPos(IGameViewService obj)
    {
        var pos = obj.Position;
        return new float3(pos.x, pos.y+obj.HalfHeight, pos.z);
    }

    public HitInfo GetHitInfo(IGameViewService obj, float sqrDist)
    {
        return new HitInfo(obj.InstanceID, sqrDist);
    }

    public AABBF GetBounds(IGameViewService obj)
    {
        return obj.Bounds;
    }

    public float GetRadius(IGameViewService obj)
    {
        return obj.HalfHeight;
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
        foreach (var entity in _entityGroup)
        {
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
