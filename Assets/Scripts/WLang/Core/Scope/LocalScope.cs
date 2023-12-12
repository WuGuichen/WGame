using System.Collections.Generic;

public class LocalScope : BaseScope
{
    private static Stack<LocalScope> _pool = new();

    public static LocalScope Get(Scope parent)
    {
        if (_pool.Count > 0)
            return _pool.Pop().Init(parent);
        return new LocalScope(parent);
    }

    public static void Push(Scope sco)
    {
        var scope = sco as LocalScope;
        if (scope != null)
        {
            _pool.Push(scope.Reset());
        }
    }
    public LocalScope(Scope parent) : base(parent)
    {
    }


    protected internal LocalScope Reset()
    {
        symbols.Clear();
        return this;
    }
    protected internal LocalScope Init(Scope parent)
    {
        enclosingScope = parent;
        return this;
    }

    public override string ScopeName => "Local";
}