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

    private struct MaxHate : IComparer<HateInfo>
    {
        public int Compare(HateInfo x, HateInfo y)
        {
            var rankComparison = x.Rank.CompareTo(y.Rank);
            if (rankComparison != 0) return rankComparison;
            return x.Value.CompareTo(y.Value);
        }
    }

    private readonly Dictionary<int, HateInfo> _hatePointDict = new();
    
    private HateInfo maxHateInfo = HateInfo.NULL;
    public int MaxHateEntityId => maxHateInfo.ID;
    public float MaxHateEntityPoint => maxHateInfo.Value;
    public float MaxHateEntityRank => maxHateInfo.Rank;


    private NativeHeap<HateInfo, MaxHate> _hateHeap = new NativeHeap<HateInfo, MaxHate>(Allocator.Persistent);

    private void RefreshMaxHate(int id, int rank, float value)
    {
        if (id == maxHateInfo.ID)
        {
            maxHateInfo = _hatePointDict[id];
            return;
        }

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
    
    public void Add(int id, float value)
    {
        int rank = 0;
        bool isContain = false;
        if (_hatePointDict.TryGetValue(id, out var info))
        {
            value = info.Value + value;
            rank = info.Rank;
            isContain = true;
        }

        if (value < 0)
            value = 0;
        else if (value > 360)
            value = 360;
        if (isContain)
        {
            // _hateHeap.Remove(info.Index);
            info = new HateInfo(id, rank, value);
        }
        else
        {
            info = new HateInfo(id, rank, value);
        }
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

    public void Dispose()
    {
        _hateHeap.Dispose();
    }
}
