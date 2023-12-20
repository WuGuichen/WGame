using System.Collections.Generic;
using WGame.Runtime;

public class DetectMgr : Singleton<DetectMgr>
{
    private readonly Dictionary<int, HatePointInfo> _hatePointInfoDict = new();
    private readonly Stack<HatePointInfo> _hatePointPool = new();

    private Dictionary<int, float> _distanceDict = new();

    public HatePointInfo RegisterHatePoint(int instId)
    {
        HatePointInfo hate;
        if (_hatePointPool.Count > 0)
            hate = _hatePointPool.Pop();
        else
            hate = new HatePointInfo();
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

        return float.MaxValue;
    }
}