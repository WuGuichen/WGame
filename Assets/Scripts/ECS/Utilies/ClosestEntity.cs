using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ClosestEntity : IComparer<IGameViewService>
{
    public int Compare(IGameViewService x, IGameViewService y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y)) return 1;
        if (ReferenceEquals(null, x)) return -1;
        var instanceIDComparison = x.InstanceID.CompareTo(y.InstanceID);
        if (instanceIDComparison != 0) return instanceIDComparison;
        var heightComparison = x.Height.CompareTo(y.Height);
        if (heightComparison != 0) return heightComparison;
        return x.HalfHeight.CompareTo(y.HalfHeight);
    }
}
