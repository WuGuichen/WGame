using System.Collections.Generic;

public class GlobalDefinition
{
    private static Dictionary<string, Symbol> definedSymbols;

    private static Scope scope;

    public static Scope Scope
    {
        get
        {
            if (scope == null)
                scope = new GlobalScope();
            return scope;
        }
    }

    public static void Init(Dictionary<string, Symbol> symbols)
    {
        definedSymbols = symbols;
        scope = new GlobalScope();
    }

    public static void Define(string name, Symbol sym)
    {
        definedSymbols[name] = sym;
    }

    public static Symbol Get(string name)
    {
        if (definedSymbols.TryGetValue(name, out var sym))
        {
            return sym;
        }

        return Symbol.NULL;
    }
}
