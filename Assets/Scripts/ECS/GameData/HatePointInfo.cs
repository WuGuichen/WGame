using System;
using System.Collections.Generic;
using Unity.Collections;

public class HatePointInfo
{
    public struct HateInfo
    {
        public int ID { get; private set; }
        public int Rank { get; private set; }
        public float Value { get; private set; }

        public HateInfo(int id, int rank, float value)
        {
            ID = id;
            Rank = rank;
            Value = value;
        }
        // public HateInfo(int id, int rank, float value, NativeHeapIndex index)
        // {
        //     ID = id;
        //     Rank = rank;
        //     Value = value;
        // }

        public static HateInfo NULL = new HateInfo(-1, -1, 0);
    }


    private const int MAX_HATEPOINT = 360;
    private readonly Dictionary<int, HateInfo> _hatePointDict = new();
    
    private HateInfo maxHateInfo = HateInfo.NULL;
    public int MaxHateEntityId => maxHateInfo.ID;
    public float MaxHateEntityPoint => maxHateInfo.Value;
    public int MaxHateEntityRank => maxHateInfo.Rank;

    private Action OnHateRankUpdate;

    // private NativeHeap<HateInfo, MaxHate> _hateHeap = new NativeHeap<HateInfo, MaxHate>(Allocator.Persistent);

    private void RefreshMaxHate(int id, int rank, float value)
    {
        var oldRank = maxHateInfo.Rank;
        if (id == maxHateInfo.ID)
        {
            maxHateInfo = _hatePointDict[id];
        }
        else
        {
            if (rank > maxHateInfo.Rank)
            {
                maxHateInfo = new HateInfo(id, rank, value);
            }
            else if (rank == maxHateInfo.Rank)
            {
                if (value > maxHateInfo.Value)
                {
                    maxHateInfo = new HateInfo(id, rank, value);
                }
            }
        }
        var newRank = maxHateInfo.Rank;
        if(oldRank != newRank)
            OnHateRankUpdate?.Invoke();
    }
    
    public void Add(int id, float value, int type)
    {
        int rank = 0;
        if (_hatePointDict.TryGetValue(id, out var info))
        {
            value = info.Value + value;
            rank = info.Rank;
        }

        if (value < 0)
        {
            if (type == HatePointType.OutSign)
            {
                if (rank > HateRankType.Null)
                {
                    while (rank > HateRankType.Null && value < 0)
                    {
                        rank -= 1;
                        value = MAX_HATEPOINT - value;
                    }

                    if (value < 0)
                        value = 0;
                }
                else
                    value = 0;
            }
            else
            {
                value = 0;
            }
        }
        else if (value > MAX_HATEPOINT)
        {
            if (rank < HateRankType.Focus)
            {
                if ((type & (HatePointType.BeHitted | HatePointType.Spotted | HatePointType.BeHitted)) > 0)
                {
                    while (rank < HateRankType.Focus && value > MAX_HATEPOINT)
                    {
                        value = value - MAX_HATEPOINT;
                        rank += 1;
                    }

                    if (value > MAX_HATEPOINT)
                        value = MAX_HATEPOINT;
                }
            }
            else
            {
                value = MAX_HATEPOINT;
            }
        }
        info = new HateInfo(id, rank, value);
        _hatePointDict[id] = info;
        RefreshMaxHate(id, rank, value);
    }

    public void Set(int id, float value, int rank)
    {
        var info = new HateInfo(id, rank, value);
        _hatePointDict[id] = info;
        RefreshMaxHate(id, rank, value);
    }

    public void Remove(int id)
    {
        if (_hatePointDict.TryGetValue(id, out var info))
        {
            _hatePointDict.Remove(id);
            if (maxHateInfo.ID == id)
            {
                maxHateInfo = HateInfo.NULL;
            }
        }
    }

    public void RegisterOnHateRankChanged(Action action)
    {
        OnHateRankUpdate -= action;
        OnHateRankUpdate += action;
    }

    public void Dispose()
    {
        // _hateHeap.Dispose();
        OnHateRankUpdate = null;
    }
}
