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

        public HateInfo Copy() => new HateInfo(ID, Rank, Value);

        public static HateInfo NULL = new HateInfo(-1, -1, 0);
    }


    private const int MAX_HATEPOINT = 360;
    private const int MAX_HATE_THREHOLD = 400;
    private const int CAN_FOCUS_TYPES = HatePointType.BeHitted | HatePointType.Spotted | HatePointType.BeHitted;
    // private const int AT_LEAST_
    private readonly Dictionary<int, HateInfo> _hatePointDict = new();
    
    private HateInfo maxHateInfo = HateInfo.NULL;
    private HateInfo newMaxHateInfo = HateInfo.NULL;
    public bool HasHateTarget => maxHateInfo.ID != HateInfo.NULL.ID;
    public bool HasNewHateTarget => newMaxHateInfo.ID != HateInfo.NULL.ID;
    public int MaxHateEntityId => maxHateInfo.ID;
    public float MaxHateEntityPoint => maxHateInfo.Value;
    public int MaxHateEntityRank => maxHateInfo.Rank;

    private Action OnHateRankUpdate;
    private Action OnHatePointUpdate;

    private bool isApplyChange = true;

    public void BeginChangeHate()
    {
        if (isApplyChange)
        {
            newMaxHateInfo = maxHateInfo.Copy();
            isApplyChange = false;
        }
        else
        {
            WLogger.Error("begin和end要成对出现");
        }
    }

    public void EndChangeHate()
    {
        if (isApplyChange)
        {
            WLogger.Error("begin和end要成对出现");
            return;
        }
        var oldRank = maxHateInfo.Rank;
        var oldID = maxHateInfo.ID;
        var oldPoint = maxHateInfo.Value;
        // 检查是否还有仇恨目标
        CheckIsHasNewHateTarget();
        var newRank = newMaxHateInfo.Rank;
        maxHateInfo = newMaxHateInfo.Copy();
        if(oldRank != newRank || oldID != newMaxHateInfo.ID)
            OnHateRankUpdate.Invoke();
        if(oldPoint != newMaxHateInfo.Value)
            OnHatePointUpdate.Invoke();
        newMaxHateInfo = HateInfo.NULL;
        isApplyChange = true;
    }

    private void RefreshNewMaxHate(int id, int rank, float value)
    {
        if (id == newMaxHateInfo.ID)
        {
            newMaxHateInfo = _hatePointDict[id];
        }
        else
        {
            if (rank > newMaxHateInfo.Rank)
            {
                newMaxHateInfo = new HateInfo(id, rank, value);
            }
            else if (rank == newMaxHateInfo.Rank)
            {
                if (value > newMaxHateInfo.Value)
                {
                    newMaxHateInfo = new HateInfo(id, rank, value);
                }
            }
        }
    }

    private void LimitLowValue(ref int rank, ref float value, int changeType)
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

    public bool TryGet(int id, out HateInfo info)
    {
        if (_hatePointDict.TryGetValue(id, out info))
        {
            return true;
        }

        return false;
    }

    public void Change(int id, float value, int type)
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
            LimitLowValue(ref rank, ref value, type);
        }
        else if (value > MAX_HATEPOINT)
        {
            // 仇恨值超过最大值了
            LimitUpValue(ref rank, ref value, type, id);
        }
        info = new HateInfo(id, rank, value);
        RefreshHateInfo(id, ref info);
        RefreshNewMaxHate(id, rank, value);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void RefreshHateInfo(int id, ref HateInfo info)
    {
        _hatePointDict[id] = info;
        if (info.Rank < HateRankType.Alert)
        {
            if (!_entity.isCamera && _entity.hasFocusEntity)
            {
                // 仇恨值过低，解除目标锁定
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

    /// <summary>
    /// 一般不用做外部调用, 请使用缓存队列
    /// </summary>
    public void Set(int id, float value, int rank)
    {
        var info = new HateInfo(id, rank, value);
        RefreshHateInfo(id, ref info);
        RefreshNewMaxHate(id, rank, value);
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

    private void CheckIsHasNewHateTarget()
    {
        if (HasNewHateTarget)
        {
            if (newMaxHateInfo.Rank == HateRankType.Null && newMaxHateInfo.Value < 0.1f)
            {
                newMaxHateInfo = HateInfo.NULL;
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
