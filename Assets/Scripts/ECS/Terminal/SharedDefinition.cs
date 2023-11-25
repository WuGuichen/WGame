using System;
using System.Collections.Generic;
using WGame.Runtime;

public class SharedDefinition : Singleton<SharedDefinition>, ISharedDefinition
{
    private float[] listFloat;
    private int idxFloat;
    private Method[] listMethod;
    private int idxMethod;
    private List<Symbol>[] listTable;
    private int idxTable;
    public SharedDefinition()
    {
        listFloat = new float[16];
        listTable = new List<Symbol>[16];
        listMethod = new Method[16];
        idxMethod = 0;
        idxFloat = 0;
        idxTable = 0;
    }

    public int FloatNum => 10000;
    public int MethodNum => 10000;
    public int TableNum => 10000;
    public Symbol Define(float value)
    {
        EnsureCapacity(ref listFloat);
        listFloat[idxFloat] = value;
        return new Symbol(idxFloat++, BaseDefinition.TYPE_FLOAT, value.ToString());
    }

    public Symbol Define(Method value)
    {
        EnsureCapacity(ref listMethod);
        listMethod[idxMethod] = value;
        return new Symbol(idxMethod++, BaseDefinition.TYPE_METHOD, value.Name);
    }

    public Symbol Define(List<Symbol> value)
    {
        EnsureCapacity(ref listTable);
        listTable[idxTable] = value;
        return new Symbol(idxTable++, BaseDefinition.TYPE_TABLE, "table");
    }

    public float GetFloat(int key)
    {
        return listFloat[key];
    }

    public Method GetMethod(int key)
    {
        return listMethod[key];
    }

    public List<Symbol> GetTable(int key)
    {
        return listTable[key];
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
        if (idxMethod >= code.Length)
        {
            int newSize = code.Length * 2;
            var newCode = new float[newSize];
            Array.Copy(code, newCode, code.Length);
            code = newCode;
        }
    }
    
    private void EnsureCapacity(ref List<Symbol>[] code)
    {
        if (idxMethod >= code.Length)
        {
            int newSize = code.Length * 2;
            var newCode = new List<Symbol>[newSize];
            Array.Copy(code, newCode, code.Length);
            code = newCode;
        }
    }
}