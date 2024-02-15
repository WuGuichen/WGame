using System;
using System.Collections.Generic;

public class HatePointInfo
{
    private GameEntity _entity;
    public HatePointInfo(GameEntity entity)
    {
        _entity = entity;
    }
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

        public static HateInfo NULL = new HateInfo(-1, -1, 0);
    }


    private const int MAX_HATEPOINT = 360;
    private const int MAX_HATE_THREHOLD = 400;
    private const int CAN_FOCUS_TYPES = HatePointType.BeHitted | HatePointType.Spotted | HatePointType.BeHitted;
    private readonly Dictionary<int, HateInfo> _hatePointDict = new();
    
    private HateInfo maxHateInfo = HateInfo.NULL;
    public bool HasHateTarget => maxHateInfo.ID != HateInfo.NULL.ID;
    public int MaxHateEntityId => maxHateInfo.ID;
    public float MaxHateEntityPoint => maxHateInfo.Value;
    public int MaxHateEntityRank => maxHateInfo.Rank;

    private Action OnHateRankUpdate;
    private Action OnHatePointUpdate;

    private void RefreshMaxHate(int id, int rank, float value)
    {
        var oldRank = maxHateInfo.Rank;
        var oldID = maxHateInfo.ID;
        var oldPoint = maxHateInfo.Value;
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
        if(oldRank != newRank || oldID != maxHateInfo.ID)
            OnHateRankUpdate.Invoke();
        if(oldPoint != maxHateInfo.Value)
            OnHatePointUpdate.Invoke();
    }

    private void LimitLowValue(ref int rank, ref float value)
    {
        if (value >= 0)
            return;
        if (rank > HateRankType.Null)
        {
            // 降低仇恨等级
            while (rank > HateRankType.Null && value < 0)
            {
                rank -= 1;
                value = MAX_HATEPOINT - value;
            }

            if (value < 0)
                value = 0;
        }
        else
        {
            // 无仇恨等级
            value = 0;
        }
    }

    private void LimitUpValue(ref int rank, ref float value, int type, int id)
    {
        // 仇恨值超越上限
        if (rank < HateRankType.Focus)
        {
            // 还没到锁定状态
            if (CanEnterFocus(type))
            {
                // 增加仇恨等级
                while (rank < HateRankType.Focus && value > MAX_HATEPOINT)
                {
                    value -= MAX_HATEPOINT;
                    rank += 1;
                }
            }
        }

        if (rank == HateRankType.Focus)
        {
            // 已到达锁定状态
            CheckHateThreshold(id, ref value);
        }
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
            // 仇恨值小于0了
            LimitLowValue(ref rank, ref value);
        }
        else if (value > MAX_HATEPOINT)
        {
            LimitUpValue(ref rank, ref value, type, id);
        }
        info = new HateInfo(id, rank, value);
        RefreshHateInfo(id, ref info);
        RefreshMaxHate(id, rank, value);
    }

    private void RefreshHateInfo(int id, ref HateInfo info)
    {
        _hatePointDict[id] = info;
        if (info.Rank < HateRankType.Alert)
        {
            if (!_entity.isCamera && _entity.hasFocusEntity)
            {
                var tarEntity = _entity.focusEntity.entity;
                if (tarEntity.instanceID.ID == info.ID)
                {
                    tarEntity.gameViewService.service.BeFocused(false);
                    _entity.RemoveFocusEntity();
                }
            }
        }
    }
    
    private bool CanEnterFocus(int type) => (type & CAN_FOCUS_TYPES) > 0;

    /// <summary>
    /// 对锁定状态下目标做仇恨值控制和仇恨转移
    /// </summary>
    private void CheckHateThreshold(int id, ref float value)
    {
        // 阈值上限
        var maxValue = MAX_HATEPOINT + MAX_HATE_THREHOLD;
        if (IsMaxInfo(id))
        {
            if (value > maxValue)
                value = maxValue;
        }
        else
        {
            if (value > maxValue)
            {
                // 成功突破阈值
                value = maxValue;
                // 只将当前仇恨目标仇恨重置
                Set(maxHateInfo.ID, 0, 0);
            }
            else
            {
                // 没有突破
                value = MAX_HATEPOINT;
            }
        }
    }

    private bool IsMaxInfo(int id) => maxHateInfo.ID == id;

    public void Set(int id, float value, int rank)
    {
        var info = new HateInfo(id, rank, value);
        RefreshHateInfo(id, ref info);
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

    public void RegisterOnHateRankChanged(Action onRankUpdate, Action onPointUpdate)
    {
        OnHateRankUpdate -= onRankUpdate;
        OnHateRankUpdate += onRankUpdate;
        OnHatePointUpdate -= onPointUpdate;
        OnHatePointUpdate += onPointUpdate;
    }

    public void CheckIsHasHateTarget()
    {
        if (HasHateTarget)
        {
            if (maxHateInfo.Rank == HateRankType.Null)
            {
                maxHateInfo = HateInfo.NULL;
                OnHateRankUpdate.Invoke();
                OnHatePointUpdate.Invoke();
            }
        }
    }

    public void Dispose()
    {
        // _hateHeap.Dispose();
        OnHateRankUpdate = null;
        OnHatePointUpdate = null;
    }
}
