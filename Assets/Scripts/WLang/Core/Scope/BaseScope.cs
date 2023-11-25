using System.Collections.Generic;

public class BaseScope : Scope
{
    private Scope enclosingScope;
    private Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

    public BaseScope(Scope parent)
    {
        enclosingScope = parent;
    }

    public virtual string ScopeName => ScopeName + symbols.Keys;
    public Scope EnclosingScope => enclosingScope;

    public void Define(string name, Symbol sym)
    {
        symbols[name] = sym;
    }

    public Symbol Resolve(string name)
    {
        if (symbols.TryGetValue(name, out var s))
        {
            return s;
        }

        if (enclosingScope != null)
            return enclosingScope.Resolve(name);

        return Symbol.NULL;
    }
}