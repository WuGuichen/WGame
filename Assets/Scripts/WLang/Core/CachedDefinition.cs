using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachedDefinition : ISharedDefinition
{
    private float[] listFloat;
    private int idxFloat;
    private Stack<int> emptyFloatIdx;
    private Method[] listMethod;
    private int idxMethod;
    private Stack<int> emptyMethodIdx;
    private List<Symbol>[] listTable;
    private int idxTable;
    private Stack<int> emptyTableIdx;
    public int FloatNum => 1000;
    public int MethodNum => 1000;
    public int TableNum => 1000;

    public CachedDefinition()
    {
        listFloat = new float[8];
        listTable = new List<Symbol>[4];
        listMethod = new Method[4];
        emptyMethodIdx = new Stack<int>();
        emptyTableIdx = new Stack<int>();
        emptyFloatIdx = new Stack<int>();
        idxMethod = 0;
        idxFloat = 0;
        idxTable = 0;
    }
    
    public Symbol Define(float value)
    {
        if (emptyFloatIdx.Count > 0)
        {
            int id = emptyFloatIdx.Pop();
            listFloat[id] = value;
            return new Symbol(id + FloatNum, BaseDefinition.TYPE_FLOAT, value.ToString());
        }
        EnsureCapacity(ref listFloat);
        listFloat[idxFloat] = value;
        int cachedId = idxFloat + FloatNum;
        idxFloat++;
        return new Symbol(cachedId, BaseDefinition.TYPE_FLOAT, value.ToString());
    }
    
    public Symbol Define(float value, int cacheId)
    {
        var index = cacheId - FloatNum;
        listFloat[index] = value;
        return new Symbol(cacheId, BaseDefinition.TYPE_FLOAT, value.ToString());
    }

    public Symbol Define(Method value)
    {
        if (emptyMethodIdx.Count > 0)
        {
            int id = emptyMethodIdx.Pop();
            listMethod[id] = value;
            return new Symbol(id + MethodNum, BaseDefinition.TYPE_METHOD, value.Name);
        }
        EnsureCapacity(ref listMethod);
        listMethod[idxMethod] = value;
        int cachedId = idxMethod + MethodNum;
        idxMethod++;
        return new Symbol(cachedId, BaseDefinition.TYPE_METHOD, value.Name);
    }
    
    public Symbol Define(Method value, int cacheId)
    {
        var index = cacheId - MethodNum;
        listMethod[index] = value;
        return new Symbol(cacheId, BaseDefinition.TYPE_METHOD, value.Name);
    }

    public Symbol Define(List<Symbol> value)
    {
        if (emptyTableIdx.Count > 0)
        {
            int id = emptyTableIdx.Pop();
            listTable[id] = value;
            return new Symbol(id + TableNum, BaseDefinition.TYPE_TABLE, "table");
        }
        EnsureCapacity(ref listTable);
        listTable[idxTable] = value;
        int cachedId = idxTable + TableNum;
        idxTable++;
        return new Symbol(cachedId, BaseDefinition.TYPE_TABLE, "table");
    }
    
    public Symbol Define(List<Symbol> value, int cacheId)
    {
        var index = cacheId - TableNum;
        listTable[index] = value;
        return new Symbol(cacheId, BaseDefinition.TYPE_TABLE, "table");
    }

    public float GetFloat(int key)
    {
        return listFloat[key-FloatNum];
    }

    public Method GetMethod(int key)
    {
        return listMethod[key-MethodNum];
    }

    public List<Symbol> GetTable(int key)
    {
        return listTable[key-TableNum];
    }
    
    private void EnsureCapacity(ref Method[] code)
    {
        if (idxMethod >= code.Length)
        {
            int newSize = code.Length * 2;
            var newCode = new Method[newSize];
            Array.Copy(code, newCode, code.Length);
            code = newCode;
        }
    }
    
    private void EnsureCapacity(ref float[] code)
    {
        if (idxFloat >= code.Length)
        {
            int newSize = code.Length * 2;
            var newCode = new float[newSize];
            Array.Copy(code, newCode, code.Length);
            code = newCode;
        }
    }
    
    private void EnsureCapacity(ref List<Symbol>[] code)
    {
        if (idxTable >= code.Length)
        {
            int newSize = code.Length * 2;
            var newCode = new List<Symbol>[newSize];
            Array.Copy(code, newCode, code.Length);
            code = newCode;
        }
    }

    public void ReleaseMethod(int key)
    {
        emptyMethodIdx.Push(key-MethodNum);
    }

    public void ReleaseFloat(int key)
    {
        emptyFloatIdx.Push(key-FloatNum);
    }

    public void ReleaseTable(int key)
    {
        emptyTableIdx.Push(key-TableNum);
    }
}
