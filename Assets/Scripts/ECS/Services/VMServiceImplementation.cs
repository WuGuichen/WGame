using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using UnityEngine;
using WGame.Runtime;

public class VMServiceImplementation : IVMService
{
    private Interpreter visitor;
    private GameEntity _entity;
    public GameEntity Entity => _entity;
    
    private static object locker = new object();

    public VMServiceImplementation(GameEntity entity)
    {
        _entity = entity;
        visitor = new Interpreter(new BaseDefinition(SharedDefinition.Inst), SharedScope.Inst);
        Set("E_SELF", entity.instanceID.ID);
    }
    
    public void DoString(string str)
    {
        AntlrInputStream stream = new AntlrInputStream(str);
        var lexer = new WLangLexer(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new WLangParser(tokens);
        visitor.ReVisitFile(parser.file());
    }

    public void Call(string name)
    {
        WLangMgr.Inst.CallCode(name, visitor);
    }

    public void Set(string name, int value)
    {
        visitor.Define(name, value);
    }

    public void Set(string name, bool value)
    {
        visitor.Define(name, value);
    }

    public void Set(string name, float value, bool cached = true)
    {
        visitor.Define(name, value, cached);
    }

    public void Set(string name, string value)
    {
        visitor.Define(name, value);
    }

    public void Set(string name, int[] value, bool cached = true)
    {
        visitor.Define(name, value, cached);
    }

    public void Set(string name, float[] value, bool cached = true)
    {
        visitor.Define(name, value, cached);
    }

    public void Set(string name, Symbol[] value, bool cached = true)
    {
        visitor.Define(name, value.ToList(), cached);
    }

    public void Set(string name, Vector2 value, bool cached = true)
    {
        visitor.Define(name, value, cached);
    }

    public void Set(string name, Vector3 value, bool cached = true)
    {
        visitor.Define(name, value, cached);
    }
    
    public void Set(string name, Method value, bool cached = true)
    {
        visitor.Define(name, value, cached);
    }

    public void TriggerEvent(int id, List<Symbol> param)
    {
        lock (locker)
        {
            SharedScope.Inst.Define("E_P" , SharedDefinition.Inst.Define(param));
            EventCenter.Trigger(id);
        }
    }

    public WBTree AppendBehaviorTree(string name, GameObject obj)
    {
        lock (locker)
        {
            return WLangMgr.Inst.GenBehaviorTree(name, visitor);
        }
    }

    public WFSM GetFSM(string name)
    {
        lock (locker)
        {
            return WLangMgr.Inst.GenFSM(name, visitor);
        }
    }

    public void ReleaseWObject(WObject obj)
    {
        visitor.ReleaseWObject(obj);
    }


    public void CleanUp()
    {
        visitor.Definition.ClearLocalCache();
    }
}
