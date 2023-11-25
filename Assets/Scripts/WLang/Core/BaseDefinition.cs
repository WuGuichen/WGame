using System;
using System.Collections.Generic;

public class BaseDefinition
{
    private float[] listFloat;
    private int idxFloat;
    private Method[] listMethod;
    private int idxMethod;
    private List<Symbol>[] listTable;
    private int idxTable;

    private int realIdx = 0;
    
    public const int TYPE_BOOLEN = 0x1;
    public const int TYPE_STRING = 0x2;
    public const int TYPE_FLOAT = 0x3;
    public const int TYPE_CHAR = 0x4;
    public const int TYPE_INT = 0x5;
    public const int TYPE_TABLE = 0x6;
    public const int TYPE_METHOD = 0x7;

    // 分段[shared -- cached -- this]
    public ISharedDefinition shared;
    private CachedDefinition _cached;
    public CachedDefinition Cached => _cached;
    private readonly int sharedMethodNum;
    private readonly int sharedFloatNum;
    private readonly int sharedTableNum;
    
    private readonly int cachedMethodNum;
    private readonly int cachedFloatNum;
    private readonly int cachedTableNum;

    public BaseDefinition()
    {
        shared = new DefaultSharedDefinition();
        _cached = new CachedDefinition();
        listFloat = new float[2];
        listMethod = new Method[2];
        listTable = new List<Symbol>[2];
        sharedMethodNum = shared.MethodNum;
        sharedFloatNum = shared.FloatNum;
        sharedTableNum = shared.TableNum;
        cachedMethodNum = _cached.MethodNum + sharedMethodNum;
        cachedFloatNum = _cached.FloatNum + sharedFloatNum;
        cachedTableNum = _cached.TableNum + sharedTableNum;
        idxMethod = cachedMethodNum;
        idxFloat = cachedFloatNum;
        idxTable = cachedTableNum;
    }
    public BaseDefinition(ISharedDefinition definition)
    {
        listFloat = new float[8];
        listMethod = new Method[4];
        listTable = new List<Symbol>[4];
        shared = definition;
        _cached = new CachedDefinition();
        sharedMethodNum = shared.MethodNum;
        sharedFloatNum = shared.FloatNum;
        sharedTableNum = shared.TableNum;
        cachedMethodNum = _cached.MethodNum + sharedMethodNum;
        cachedFloatNum = _cached.FloatNum + sharedFloatNum;
        cachedTableNum = _cached.TableNum + sharedTableNum;
        idxMethod = cachedMethodNum;
        idxFloat = cachedFloatNum;
        idxTable = cachedTableNum;
    }

    public Symbol Define(float value, bool cached = false)
    {
        if (cached)
            return _cached.Define(value);
        else
            realIdx = idxFloat - cachedFloatNum;
        EnsureCapacity(ref listFloat);
        listFloat[realIdx] = value;
        return new Symbol(idxFloat++, TYPE_FLOAT, value.ToString());
    }
    
    public Symbol Define(Method value, bool cached = false)
    {
        if (cached)
            return _cached.Define(value);
        else
            realIdx = idxMethod - cachedMethodNum;
        EnsureCapacity(ref listMethod);
        listMethod[realIdx] = value;
        return new Symbol(idxMethod++, TYPE_METHOD, value.Name);
    }

    public Symbol Define(List<Symbol> value, bool cached = false)
    {
        if (cached)
            return _cached.Define(value);
        else
            realIdx = idxTable - cachedTableNum;
        EnsureCapacity(ref listTable);
        listTable[realIdx] = value;
        return new Symbol(idxTable++, TYPE_TABLE, "table");
    }

    public float GetFloat(int key)
    {
        if (key > idxFloat)
            WLogger.Error("有错误");
        if(key >= cachedFloatNum)
            return listFloat[key -cachedFloatNum];
        if (key >= _cached.FloatNum)
            return _cached.GetFloat(key);
        return shared.GetFloat(key);
    }

    public Method GetMethod(int key)
    {
        if (key > idxMethod)
            WLogger.Error("有错误" + listMethod[key-cachedMethodNum].Name);
        if(key >= cachedMethodNum)
            return listMethod[key - cachedMethodNum];
        if (key >= _cached.MethodNum)
            return _cached.GetMethod(key);
        return shared.GetMethod(key);
    }

    public List<Symbol> GetTable(int key)
    {
        if (key > idxTable)
            WLogger.Error("有错误");
        if(key >= cachedTableNum)
            return listTable[key-cachedTableNum];
        if (key >= _cached.TableNum)
            return _cached.GetTable(key);
        return shared.GetTable(key);
    }

    private void EnsureCapacity(ref Method[] code)
    {
        if (realIdx >= code.Length)
        {
            int newSize = code.Length * 2;
            var newCode = new Method[newSize];
            Array.Copy(code, newCode, code.Length);
            code = newCode;
        }
    }
    
    private void EnsureCapacity(ref float[] code)
    {
        if (realIdx >= code.Length)
        {
            int newSize = code.Length * 2;
            var newCode = new float[newSize];
            Array.Copy(code, newCode, code.Length);
            code = newCode;
        }
    }
    
    private void EnsureCapacity(ref List<Symbol>[] code)
    {
        if (realIdx >= code.Length)
        {
            int newSize = code.Length * 2;
            var newCode = new List<Symbol>[newSize];
            Array.Copy(code, newCode, code.Length);
            code = newCode;
        }
    }

    public void ClearLocalCache()
    {
        idxFloat = cachedFloatNum;
        idxMethod = cachedMethodNum;
        idxTable = cachedTableNum;
    }
}
