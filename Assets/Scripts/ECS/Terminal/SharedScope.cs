using System;
using System.Collections.Generic;

public class SharedScope : BaseScope
{
    private static readonly System.Lazy<SharedScope> _lazy = new System.Lazy<SharedScope>(() => new SharedScope());

    public static SharedScope Inst => _lazy.Value;
    public MethodDefine methodDef;
    public ConstDefine constDef;
    
    public SharedScope() : base(null)
    {
        methodDef = new MethodDefine();
        constDef = new ConstDefine();
        methodDef.BindAll(Define);
        methodDef.BindAll(DefineFunc);
        constDef.BindInt(Define);
    }

    private void Define(string name, Action<List<Symbol>, Interpreter> action)
    {
        Define(name, SharedDefinition.Inst.Define(Method.Get(name, action)));
    }

    private void DefineFunc(string name, WLangFunc action)
    {
        Define(name, SharedDefinition.Inst.Define(Method.Get(name, action)));
    }
    
    private void Define(string name, int value)
    {
        Define(name, new Symbol(value));
    }
    
    private void Define(string name, float value)
    {
        Define(name, SharedDefinition.Inst.Define(value));
    }
}
