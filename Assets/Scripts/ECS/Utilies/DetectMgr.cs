using System.Collections.Generic;
using UnityEngine;
using WGame.Runtime;

public class DetectMgr : Singleton<DetectMgr>
{
    private readonly Dictionary<int, HatePointInfo> _hatePointInfoDict = new();
    private readonly Stack<HatePointInfo> _hatePointPool = new();

    private Dictionary<int, float> _distanceDict = new();

    private const float MAX_FLOAT = float.MaxValue - 1f;

    public HatePointInfo RegisterHatePoint(int instId)
    {
        HatePointInfo hate;
        if (_hatePointPool.Count > 0)
            hate = _hatePointPool.Pop();
        else
            hate = new HatePointInfo(EntityUtils.GetGameEntity(instId));
        _hatePointInfoDict.Add(instId, hate);
        return hate;
    }
    
    public void CancelHatePoint(int instId)
    {
        var hate = _hatePointInfoDict[instId];
        hate.Dispose();
        _hatePointPool.Push(hate);
        _hatePointInfoDict.Remove(instId);
    }

    public HatePointInfo GetHatePointInfo(int instId)
    {
        return _hatePointInfoDict[instId];
    }

    public void UpdateDistance(int id1, int id2, float distance)
    {
        _distanceDict[Utils.PairInt(id1, id2)] = distance;
    }

    public float GetDistance(GameEntity entity1, GameEntity entity2)
    {
        if (_distanceDict.TryGetValue(Utils.PairInt(entity1.instanceID.ID, entity2.instanceID.ID), out float dist))
        {
            return dist;
        }

        return float.MaxValue;
    }

    public float GetDistance(int id1, int id2)
    {
        if (_distanceDict.TryGetValue(Utils.PairInt(id1, id2), out float dist))
        {
            return dist;
        }

        var res = (EntityUtils.GetGameEntity(id1).position.value - EntityUtils.GetGameEntity(id2).position.value)
            .magnitude;
        return res;
    }
    
    public float GetAngle(int originId, int targetId, bool is360 = false)
    {
        var oriEntity = EntityUtils.GetGameEntity(originId);
        var dir = EntityUtils.GetGameEntity(targetId).position.value - oriEntity.position.value;
        float dist = GetDistance(originId, targetId);
        var normalDir = dir / dist;
        if (is360)
            return normalDir.GetAngle360(oriEntity.gameViewService.service.Model.forward, oriEntity.gameViewService.service.Model.up);
        return normalDir.GetAngle(oriEntity.gameViewService.service.Model.forward);
    }

    public float GetAngle(int originId, int targetId, out float dist, bool is360 = false)
    {
        var oriEntity = EntityUtils.GetGameEntity(originId);
        var dir = EntityUtils.GetGameEntity(targetId).position.value - oriEntity.position.value;
        dist = GetDistance(originId, targetId);
        var normalDir = dir / dist;
        if (is360)
            return normalDir.GetAngle360(oriEntity.gameViewService.service.Model.forward, oriEntity.gameViewService.service.Model.up);
        return normalDir.GetAngle(oriEntity.gameViewService.service.Model.forward);
    }

    public float GetAngle(int originId, int targetId, Vector3 dir, out float dist, bool is360 = false)
    {
        var oriEntity = EntityUtils.GetGameEntity(originId);
        dist = GetDistance(originId, targetId);
        var normalDir = dir / dist;
        if (is360)
            return normalDir.GetAngle360(oriEntity.gameViewService.service.Model.forward, oriEntity.gameViewService.service.Model.up);
        return normalDir.GetAngle(oriEntity.gameViewService.service.Model.forward);
    }
    
    public float GetAngle(int originId, int targetId, Vector3 dir, bool is360 = false)
    {
        var oriEntity = EntityUtils.GetGameEntity(originId);
        float dist = GetDistance(originId, targetId);
        var normalDir = dir / dist;
        if (is360)
            return normalDir.GetAngle360(oriEntity.gameViewService.service.Model.forward, oriEntity.gameViewService.service.Model.up);
        return normalDir.GetAngle(oriEntity.gameViewService.service.Model.forward);
    }
}