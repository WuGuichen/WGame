using System.Collections.Generic;

public interface ISharedDefinition
{
    int FloatNum { get; }
    int MethodNum { get; }
    int TableNum { get; }

    Symbol Define(float value);
    Symbol Define(Method value);
    Symbol Define(List<Symbol> value);

    float GetFloat(int key);
    Method GetMethod(int key);
    List<Symbol> GetTable(int key);
}

public class DefaultSharedDefinition : ISharedDefinition
{
    public int FloatNum => 1000;
    public int MethodNum => 1000;
    public int TableNum => 1000;
    public Symbol Define(float value)
    {
        throw new System.NotImplementedException();
    }

    public Symbol Define(Method value)
    {
        throw new System.NotImplementedException();
    }

    public Symbol Define(List<Symbol> value)
    {
        throw new System.NotImplementedException();
    }

    public float GetFloat(int key)
    {
        throw new System.NotImplementedException();
    }

    public Method GetMethod(int key)
    {
        throw new System.NotImplementedException();
    }

    public List<Symbol> GetTable(int key)
    {
        throw new System.NotImplementedException();
    }
}