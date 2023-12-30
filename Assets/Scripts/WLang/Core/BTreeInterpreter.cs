using System.Collections.Generic;
using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using UnityEngine;

public class BTreeInterpreter : WLangBaseVisitor<Symbol>
{
    private static Stack<BTreeInterpreter> _pool = new Stack<BTreeInterpreter>();

    public static BTreeInterpreter Get(Interpreter interpreter, GameObject owner, string treeName, Scope scope, BaseDefinition definition)
    {
        if (_pool.Count > 0)
        {
            return _pool.Pop().Init(interpreter, owner, treeName,scope, definition);
        }

        return new BTreeInterpreter(interpreter, owner, treeName, scope, definition);
    }

    public static void Push(BTreeInterpreter interpreter)
    {
        interpreter.Release();
        _pool.Push(interpreter);
    }
    
    private Scope currentScope;
    private BaseDefinition _def;
    private BehaviorTreeBuilder _builder;
    private string treeName;
    private Interpreter interpreter;
    private GameObject _treeOwner;

    private const int TYPE_FLOAT = BaseDefinition.TYPE_FLOAT;
    private const int TYPE_CHAR = BaseDefinition.TYPE_CHAR;
    private const int TYPE_INT = BaseDefinition.TYPE_INT;
    private const int TYPE_TABLE = BaseDefinition.TYPE_TABLE;
    private const int TYPE_METHOD = BaseDefinition.TYPE_METHOD;

    public BTreeInterpreter(Interpreter interpreter, GameObject owner, string treeName, Scope scope, BaseDefinition definition)
    {
        Init(interpreter, owner, treeName, scope, definition);
    }
    
    private BTreeInterpreter Init(Interpreter interpreter, GameObject owner, string treeName, Scope scope, BaseDefinition definition)
    {
        currentScope = scope;
        this.treeName = treeName;
        _def = definition;
        this.interpreter = interpreter;
        _treeOwner = owner;
        return this;
    }
    
    private void Release()
    {
        currentScope = null;
        _def = null;
        _builder = null;
        interpreter = null;
    }
    
    public WBTree BuildBTree(WLangParser.BTreeBuilderContext context, string treeName)
    {
        var code = context.fileCode();
        if (code != null)
        {
            interpreter.CacheCodeImporter = new WLangImporter()
            {
                name = treeName,
                type = ImporterType.BTree,
            };
            interpreter.DoImportCode = true;
            interpreter.IgnoreReturn = true;
            context.Accept(interpreter);
            interpreter.IgnoreReturn = false;
            interpreter.DoImportCode = false;
        }
        string name = treeName;
        var tree = interpreter.ObjectPool.GetWBTree(_treeOwner);
        _builder = tree.TREE_BUILDER;
        var blocks = context.treeBlock();
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].Accept(this);
        }
        WBTreeMgr.Inst.AddTree(name, _builder);
        return tree;
    }
    
    public override Symbol VisitTreeSelector(WLangParser.TreeSelectorContext context)
    {
        _builder.Selector();
        context.children[1].Accept(this);
        _builder.End();
        return Symbol.NULL;
    }

    public override Symbol VisitTreeContent(WLangParser.TreeContentContext context)
    {
        var blocks = context.treeBlock();
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].Accept(this);
        }
        return Symbol.NULL;
    }

    public override Symbol VisitTreeSequence(WLangParser.TreeSequenceContext context)
    {
        _builder.Sequence();
        context.children[1].Accept(this);
        _builder.End();
        return Symbol.NULL;
    }

    public override Symbol VisitTreeDo(WLangParser.TreeDoContext context)
    {
        var inter = interpreter;
        _builder.Do(() =>
        {
            inter.IsLocalNeedReturn = true;
            var val = context.children[1].Accept(inter);
            inter.IsLocalNeedReturn = false;
            return (TaskStatus)val.Value;
        });
        return Symbol.NULL;
    }

    public override Symbol VisitTreeWait(WLangParser.TreeWaitContext context)
    {
        var p = context.numParam().s;
        if (p.Type == WLangParser.INT)
            _builder.Wait(int.Parse(p.Text));
        // else if(p.Type == WLangParser.FLOAT)
        //     _builder.WaitTime(float.Parse(p.Text));
        else
        {
            WLogger.Error("数据错误");
        }
        return Symbol.NULL;
    }

    public override Symbol VisitTreeWaitTime(WLangParser.TreeWaitTimeContext context)
    {
        var p = context.numParam().Accept(interpreter);
        if (p.Type == TYPE_METHOD)
            _builder.WaitTime(interpreter.Definition.GetMethod(p.Value).Call(interpreter).Value);
        else
        {
            _builder.WaitTime(p.ToFloat(interpreter.Definition));
        }
        return Symbol.NULL;
    }
}
