using System.Collections.Generic;

public class SparseSet<T>
{
    private List<T> packed = new List<T>();
    private int[] checkList = new int[32];

    public void Add(int key, T value)
    {
        if (checkList[key] > 0)
        {
            return;
        }
        
        packed.Add(value);
        checkList[key] = packed.Count;
    }

    public void Remove(int key)
    {
        if (checkList[key] > 0)
        {
            packed[checkList[key] - 1] = packed[^1];
            checkList[packed.Count] = checkList[key];
            checkList[key] = 0;
            packed.RemoveAt(packed.Count - 1);
        }
    }

    public bool IsContain(int key) => checkList[key] > 0;
    public bool IsEmpty => packed.Count == 0;

    public T this[int key]
    {
        get
        {
            if (checkList[key] > 0)
            {
                return packed[checkList[key] - 1];
            }
            return default;
        }
    }

    public bool TryGet(int key, out T value)
    {
        if (checkList[key] > 0)
        {
            value = packed[checkList[key] - 1];
            return true;
        }
        value = default;
        return false;
    }

    public void Clear()
    {
        this.packed.Clear();
        this.checkList = new int[32];
    }
}
