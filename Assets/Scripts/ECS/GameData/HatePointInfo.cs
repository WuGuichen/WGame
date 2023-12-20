using System.Collections.Generic;

public class HatePointInfo
{
    private readonly Dictionary<int, float> _hatePointDict = new();
    
    private int maxHateEntityId = 0;
    public int MaxHateEntityId => maxHateEntityId;
    private float maxHateEntityPoint = float.MinValue;
    public float MaxHateEntityPoint => maxHateEntityPoint;
    
    public void Add(int id, float value)
    {
        if (_hatePointDict.TryGetValue(id, out var point))
        {
            value = point + value;
        }

        if (value < 0)
            value = 0;
        else if (value > 1000)
            value = 1000;
        _hatePointDict[id] = value;
        RefreshMaxTarget(id, value);
    }

    public void Set(int id, float value)
    {
        _hatePointDict[id] = value;
        RefreshMaxTarget(id, value);
    }
    
    private void RefreshMaxTarget(int id, float value)
    {
        if (id == maxHateEntityId)
        {
            maxHateEntityPoint = value;
        }
        else
        {
            if (value > maxHateEntityPoint)
            {
                maxHateEntityPoint = value;
                maxHateEntityId = id;
            }
        }
    }

    public void Dispose()
    {
        
    }
}
