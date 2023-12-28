using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Tree;
using UnityEngine;
using Timer = UnityTimer.Timer;

public class Interpreter : WLangBaseVisitor<Symbol>
{
    private const int TYPE_BOOLEN = BaseDefinition.TYPE_BOOLEN;
    private const int TYPE_STRING = BaseDefinition.TYPE_STRING;
    private const int TYPE_FLOAT = BaseDefinition.TYPE_FLOAT;
    private const int TYPE_CHAR = BaseDefinition.TYPE_CHAR;
    private const int TYPE_INT = BaseDefinition.TYPE_INT;
    private const int TYPE_TABLE = BaseDefinition.TYPE_TABLE;
    private const int TYPE_METHOD = BaseDefinition.TYPE_METHOD;
    private const string RETURN = "Return";

    private Scope baseScope;
    private Scope currentScope;
    private BaseDefinition _def;
    public BaseDefinition Definition => _def;

    public bool IgnoreReturn { get; set; } = false;
    private bool isNeedReturn = false;
    private WLangParser.ExprRightContext returnVal = null;
    private Symbol returnSym = Symbol.NULL;

    private bool cacheToObject = false;

    private GameObject _treeOwner;

    private WObjectPool _objectPool;
    public WObjectPool ObjectPool => _objectPool;

    private List<int> cachedMethodList = new();
    private List<int> cachedFloatList = new();
    private List<int> cachedTableList = new();

    private object locker = new object();
    
    public bool DoImportCode { get; set; } = false;
    public Interpreter(BaseDefinition def, BaseScope baseScope)
    {
        this._def = def;
        this.baseScope = baseScope;
        _objectPool = new WObjectPool();
        currentScope = LocalScope.Get(baseScope);
        currentScope.Define("PRINT", _def.Define(Method.PRINT, true));
    }

    public WFSM BuildWFSM(WLangParser.FsmBuilderContext context, string fsmName)
    {
        cacheToObject = true;
        var inter = FSMInterpreter.Get(this, fsmName, currentScope, _def);
        var res = inter.BuildWFSM(context);
        FSMInterpreter.Push(inter);
        res.SetCacheFloat(cachedFloatList);
        res.SetCacheMethod(cachedMethodList);
        res.SetCacheTable(cachedTableList);
        cachedFloatList = new List<int>();
        cachedMethodList = new List<int>();
        cachedTableList = new List<int>();
        cacheToObject = false;
        return res;
    }

    public WBTree BuildWBree(WLangParser.BTreeBuilderContext context, string treeName)
    {
        cacheToObject = true;
        if (_treeOwner == null)
            _treeOwner = new GameObject("BTreeOwner");
        var inter = BTreeInterpreter.Get(this, _treeOwner, treeName, currentScope, _def);
        var res = inter.BuildBTree(context, treeName);
        res.SetCacheFloat(cachedFloatList);
        res.SetCacheMethod(cachedMethodList);
        res.SetCacheTable(cachedTableList);
        cachedFloatList = new List<int>();
        cachedMethodList = new List<int>();
        cachedTableList = new List<int>();
        cacheToObject = false;
        return res;
    }

    public Symbol ReVisitFile(WLangParser.FileContext context)
    {
        lock (locker)
        {
            isNeedReturn = false;
            return VisitFile(context);
        }
    }
    
    public override Symbol VisitFile(WLangParser.FileContext context)
    {
        for (int i = 0; i < context.ChildCount; i++)
        {
            context.GetChild(i).Accept(this);
            if (isNeedReturn)
            {
                break;
            }
        }
        
        _def.ClearLocalCache();
        return returnSym;
    }
    
    private Symbol DefineValue(List<Symbol> value)
    {
        if (cacheToObject)
        {
            var sym = _def.Define(value, true);
            cachedTableList.Add(sym.Value);
            return sym;
        }

        return _def.Define(value);
    }
    
    private Symbol DefineValue(Method value)
    {
        if (cacheToObject)
        {
            var sym = _def.Define(value, true);
            cachedMethodList.Add(sym.Value);
            return sym;
        }

        return _def.Define(value);
    }

    private Symbol DefineValue(float value)
    {
        if (cacheToObject)
        {
            var sym = _def.Define(value, true);
            cachedFloatList.Add(sym.Value);
            return sym;
        }

        return _def.Define(value);
    }

    public void Define(string name, Symbol sym)
    {
        currentScope.Define(name, sym);
    }
    
    public void Define(string name, Method value, bool cache = false)
    {
        currentScope.Define(name, _def.Define(value, cache));
    }
    
    public void Define(string name, bool value, bool cache = false)
    {
        currentScope.Define(name, value ? Symbol.TRUE : Symbol.FALSE);
    }
    
    public void Define(string name, Vector3 value, bool cache = false)
    {
        currentScope.Define(name, _def.Define(new List<Symbol>()
        {
            _def.Define(value.x),
            _def.Define(value.y),
            _def.Define(value.z)
        }, cache));
    }
    
    public void Define(string name, Vector2 value, bool cache = false)
    {
        currentScope.Define(name, _def.Define(new List<Symbol>()
        {
            _def.Define(value.x),
            _def.Define(value.y),
        }, cache));
    }
    
    public void Define(string name, List<Symbol> value, bool cache = false)
    {
        currentScope.Define(name, _def.Define(value, cache));
    }
    
    public void Define(string name, float[] value, bool cache = false)
    {
        var list = new List<Symbol>();
        for(int i = 0; i < value.Length; i++)
            list.Add(_def.Define(value[i], cache));
        currentScope.Define(name, _def.Define(list, cache));
    }
    
    public void Define(string name, int[] value, bool cache = false)
    {
        var list = new List<Symbol>();
        for(int i = 0; i < value.Length; i++)
            list.Add(new Symbol(value[i]));
        currentScope.Define(name, _def.Define(list, cache));
    }
    
    public void Define(string name, float value, bool cache = false)
    {
        currentScope.Define(name, _def.Define(value, cache));
    }
    
    public void Define(string name, int value)
    {
        currentScope.Define(name, new Symbol(value));
    }
    
    public void Define(string name, string value)
    {
        currentScope.Define(name, new Symbol(value));
    }
    
    public override Symbol VisitStatAssign(WLangParser.StatAssignContext context)
    {
        var res = context.children[2].Accept(this);
        var ctx = context.children[0];
        var pointCtx = ctx as WLangParser.ExprPointContext;
        if (pointCtx != null)
        {
            Symbol symList = currentScope.Resolve(pointCtx.l.Text);
            List<Symbol> list = null;
            int point = 0;
            for (int i = 1; i < pointCtx.ChildCount; i++)
            {
                if (symList.Type == TYPE_TABLE)
                {
                    list = _def.GetTable(symList.Value);
                    point = pointCtx.children[i].Accept(this).Value;
                    symList = list[point];
                }
            }

            list[point] = res;
        }
        else
        {
            currentScope.Define(context.Start.Text, res);
        }
        return Symbol.NULL;
    }

    public override Symbol VisitPoint(WLangParser.PointContext context)
    {
        return context.children[1].Accept(this);
    }

    public override Symbol VisitExprPoint(WLangParser.ExprPointContext context)
    {
        var listSym = currentScope.Resolve(context.l.Text);
        for (int i = 1; i < context.ChildCount; i++)
        {
            var child = context.children[i];
            if (child is TerminalNodeImpl == false)
            {
                int point = child.Accept(this).Value;
                if (listSym.Type == TYPE_TABLE)
                {
                    var list = _def.GetTable(listSym.Value);
                    if (point >= list.Count)
                    {
                        listSym = Symbol.NULL;
                        WLogger.Warning("超出列表边界" + point);
                    }
                    else
                        listSym = list[point];
                }
                else
                {
                    if(listSym.IsNotNull)
                        throw new ArgumentException("point target type need table");
                }
            }
        }

        return listSym;
    }

    public override Symbol VisitStatReturn(WLangParser.StatReturnContext context)
    {
        if(!IgnoreReturn)
            isNeedReturn = true;
        returnVal = context.r;
        returnSym = returnVal.Accept(this);

        return returnSym;
    }

    public override Symbol VisitStatImport(WLangParser.StatImportContext context)
    {
        string file = context.f.Text.Replace('.', '_');
        if (DoImportCode)
        {
            WLangMgr.Inst.CallCode(file, this);
        }
        else
        {
            WLangMgr.Inst.LoadCode(file, this);
        }
        return Symbol.NULL;
    }

    public override Symbol VisitStatMethod(WLangParser.StatMethodContext context)
    {
        var text = context.f.Text;
        var def = context.p;
        string[] param = null;
        if (def != null)
            param = def.GetParams();
        currentScope.Define(text
            , DefineValue(Method.Get(text, context.b.f, param)));

        if (isNeedReturn)
        {
            isNeedReturn = false;
            return returnSym;
        }

        return Symbol.NULL;
    }

    public override Symbol VisitStatCommand(WLangParser.StatCommandContext context)
    {
        var res = context.exprMethod().Accept(this);
        return res;
    }

    public override Symbol VisitExprMethod(WLangParser.ExprMethodContext context)
    {
        var text = context.d.Text;
        var method = GetMethod(text);
        var res = Symbol.NULL;
        if (method != null)
        {
            PopScope();
            Symbol caller = Symbol.NULL;
            if (context.e != null)
            {
                caller = context.e.Accept(this);
                if (caller.IsNull)
                    WLogger.Warning("方法调用者为空:" + text);
            }

            res = method.Call(ParseParameters(context.parameters(), caller), this);
            PushScope();
        }
        else
        {
            WLogger.Warning("调用空方法：" + text);
        }

        return res;
    }

    private Method GetMethod(string text)
    {
        var sym = currentScope.Resolve(text);
        if (sym.IsNotNull)
        {
            if (sym.Type == TYPE_METHOD)
                return _def.GetMethod(sym.Value);
        }

        return null;
    }

    private List<Symbol> parseRes = new List<Symbol>();
    private List<Symbol> ParseParameters(WLangParser.ParametersContext context, Symbol caller)
    {
        parseRes.Clear();
        if(caller.IsNotNull)
            parseRes.Add(caller);
        if (context == null)
            return parseRes;
        for (int i = 0; i < context.ChildCount; i++)
        {
            var child = context.children[i];
            if (child is TerminalNodeImpl == false)
            { 
                parseRes.Add(child.Accept(this));
            }
        }

        return parseRes;
    }

    public override Symbol VisitWaitStatement(WLangParser.WaitStatementContext context)
    {
        float t = 0;
        string text = context.t.Text;
        if (context.t.Type == WLangParser.ID)
            t = _def.GetFloat(currentScope.Resolve(text).Value);
        else
            t = float.Parse(text);
        Timer.Register(t, (() =>
        {
            context.b.Accept(this);
        }));
        return Symbol.NULL;
    }

    public override Symbol VisitIfStatement(WLangParser.IfStatementContext context)
    {
        for (int i = 0; i < context.ChildCount; i++)
        {
            if (context.children[i].Accept(this).IsTrue)
            {
                break;
            }
        }

        if (isNeedReturn)
        {
            if (currentScope == baseScope)
                isNeedReturn = false;
            return returnSym;
        }
        return Symbol.NULL;
    }

    public override Symbol VisitIfStat(WLangParser.IfStatContext context)
    {
        var res = context.e.Accept(this);
        if (res.IsTrue)
        {
            context.b.Accept(this);
        }
        return res;
    }

    public override Symbol VisitElseIfStat(WLangParser.ElseIfStatContext context)
    {
        var res = context.e.Accept(this);
        if (res.IsTrue)
        {
            context.b.Accept(this);
        }
        return res;
    }

    public override Symbol VisitElseStat(WLangParser.ElseStatContext context)
    {
        context.b.Accept(this);
        return Symbol.TRUE;
    }

    public override Symbol VisitWhileStatement(WLangParser.WhileStatementContext context)
    {
        int c = 0;
        while (context.e.Accept(this).IsTrue)
        {
            context.b.Accept(this);
            c++;
            if (c > 100)
                break;
        }

        return Symbol.NULL;
    }

    public override Symbol VisitExprList(WLangParser.ExprListContext context)
    {
        List<Symbol> list;
        if (context.Start.Type == WLangParser.OPENBRACK)
        {
            list = new List<Symbol>();
            for (int i = 0; i < context.ChildCount; i++)
            {
                var child = context.children[i];
                if(child is TerminalNodeImpl == false)
                    list.Add(child.Accept(this));
            }
        }
        else
        {
            list = new List<Symbol>();
            var start = context.children[0].Accept(this);
            var end = context.children[2].Accept(this);
            int span = 1;
            if (context.ChildCount > 3)
                span = context.children[4].Accept(this).Value;
            list.Add(start.Copy());
            int tmp = 0;
            if (end.Value > start.Value)
            {
                tmp = start.Value + span;
                while (tmp <= end.Value)
                {
                    list.Add(new Symbol(tmp, TYPE_INT, tmp.ToString()));
                    tmp += span;
                }
            }
            else
            {
                if (span >= 0)
                    span = -1;
                tmp = start.Value + span;
                while (tmp >= end.Value)
                {
                    list.Add(new Symbol(tmp, TYPE_INT, tmp.ToString()));
                    tmp += span;
                }
            }
        }

        return DefineValue(list);
    }

    public override Symbol VisitForStatement(WLangParser.ForStatementContext context)
    {
        var id = context.i;
        var listCtx = context.children[3];

        List<Symbol> list;
        if (listCtx is TerminalNodeImpl)
            list = _def.GetTable(currentScope.Resolve(listCtx.GetText()).Value);
        else
            list = _def.GetTable(listCtx.Accept(this).Value);

        PopScope();
        for (int i = 0; i < list.Count; i++)
        {
            Define(id.Text, list[i]);
            context.b.Accept(this);
            if (isNeedReturn)
                break;
        }
        PushScope();

        return Symbol.NULL;
    }

    public override Symbol VisitBlock(WLangParser.BlockContext context)
    {
        isNeedReturn = false;
        for (int i = 0; i < context.f.ChildCount; i++)
        {
            context.f.GetChild(i).Accept(this);
            if (isNeedReturn)
            {
                break;
            }
        }
        
        return returnSym;
    }

    public override Symbol VisitExprUnary(WLangParser.ExprUnaryContext context)
    {
        string op = context.o.Text;
        Symbol v = context.GetChild(1).Accept(this);
        int type = v.Type;
        switch (op)
        {
            case "-":
                if (type == TYPE_INT)
                    return new Symbol(-v.Value, TYPE_INT);
                if (type == TYPE_FLOAT)
                {
                    return DefineValue(-_def.GetFloat(v.Value));
                }
                break;
            case "!":
                if (type == TYPE_BOOLEN)
                    return new Symbol(v.Value > 0 ? 0 : 1, TYPE_BOOLEN);
                break;
        }

        return Symbol.ERROR;
    }

    public override Symbol VisitExprBinary(WLangParser.ExprBinaryContext context)
    {
        Symbol res = Symbol.ERROR;
        Symbol l = context.children[0].Accept(this);
        Symbol r = context.children[2].Accept(this);
        string op = context.o.Text;
        if (l.IsNull || r.IsNull)
        {
            switch (op)
            {
                case "==":
                    break;
                case "!=":
                    break;
                default:
                    var builder = new StringBuilder();
                    builder.Append("非法的二元运算：");
                    if (l.IsNull)
                        builder.Append("左值为NULL: ");
                    if (r.IsNull)
                        builder.Append("右值为NULL: ");
                    builder.Append(context.GetText());
                    throw new ArgumentException(builder.ToString());
            }
        }
        switch (op)
        {
            case "+":
                if (l.Type == r.Type)
                {
                    if(l.Type == TYPE_INT)
                        res = new Symbol(l.Value + r.Value, TYPE_INT);
                    else if(l.Type == TYPE_STRING)
                        res = new Symbol(l.Text + r.Text, TYPE_STRING);
                    else
                        res = DefineValue(_def.GetFloat(l.Value) + _def.GetFloat(r.Value));
                }
                else
                {
                    if(l.Type == TYPE_INT)
                        res = DefineValue(l.Value + _def.GetFloat(r.Value));
                    else
                        res = DefineValue(_def.GetFloat(l.Value) + r.Value);
                }
                break;
            case "-":
                if (l.Type == r.Type)
                {
                    if(l.Type == TYPE_INT)
                        res = new Symbol(l.Value - r.Value, TYPE_INT);
                    else
                        res = DefineValue(_def.GetFloat(l.Value) - _def.GetFloat(r.Value));
                }
                else
                {
                    if(l.Type == TYPE_INT)
                        res = DefineValue(l.Value - _def.GetFloat(r.Value));
                    else
                        res = DefineValue(_def.GetFloat(l.Value) - r.Value);
                }
                break;
            case "*":
                if (l.Type == r.Type)
                {
                    if(l.Type == TYPE_INT)
                        res = new Symbol(l.Value * r.Value, TYPE_INT);
                    else
                        res = DefineValue(_def.GetFloat(l.Value) * _def.GetFloat(r.Value));
                }
                else
                {
                    if(l.Type == TYPE_INT && r.Type == TYPE_FLOAT)
                        res = DefineValue(l.Value * _def.GetFloat(r.Value));
                    else if (l.Type == TYPE_FLOAT && r.Type == TYPE_INT)
                        res = DefineValue(_def.GetFloat(l.Value) * r.Value);
                    else if (l.Type == TYPE_TABLE)
                    {
                        MultiplyTableAndNumber(ref l, ref r);
                        res = l;
                    }
                    else if (r.Type == TYPE_TABLE)
                    {
                        MultiplyTableAndNumber(ref r, ref l);
                        res = r;
                    }
                    else
                        WLogger.Warning("两种类型不能相乘");
                }
                break;
            case "/":
                if (l.Type == r.Type)
                {
                    if(l.Type == TYPE_INT)
                        res = new Symbol(l.Value / r.Value, TYPE_INT);
                    else
                        res = DefineValue(_def.GetFloat(l.Value) / _def.GetFloat(r.Value));
                }
                else
                {
                    if(l.Type == TYPE_INT)
                        res = DefineValue(l.Value / _def.GetFloat(r.Value));
                    else
                        res = DefineValue(_def.GetFloat(l.Value) / r.Value);
                }
                break;
            case ">":
                if (l.Type == r.Type)
                {
                    if (l.Type == TYPE_INT)
                        res = l.Value > r.Value ? Symbol.TRUE : Symbol.FALSE;
                    else
                        res = _def.GetFloat(l.Value) > _def.GetFloat(r.Value) ? Symbol.TRUE : Symbol.FALSE;
                }
                else
                {
                    if(l.Type == TYPE_FLOAT)
                        res = _def.GetFloat(l.Value) > r.Value ? Symbol.TRUE : Symbol.FALSE;
                    else
                        res = l.Value > _def.GetFloat(r.Value) ? Symbol.TRUE : Symbol.FALSE;
                }
                break;
            case ">=":
                if (l.Type == r.Type)
                {
                    if (l.Type == TYPE_INT)
                        res = l.Value >= r.Value ? Symbol.TRUE : Symbol.FALSE;
                    else
                        res = _def.GetFloat(l.Value) >= _def.GetFloat(r.Value) ? Symbol.TRUE : Symbol.FALSE;
                }
                else
                {
                    if(l.Type == TYPE_FLOAT)
                        res = _def.GetFloat(l.Value) >= r.Value ? Symbol.TRUE : Symbol.FALSE;
                    else
                        res = l.Value >= _def.GetFloat(r.Value) ? Symbol.TRUE : Symbol.FALSE;
                }
                break;
            case "<":
                if (l.Type == r.Type)
                {
                    if (l.Type == TYPE_INT)
                        res = l.Value < r.Value ? Symbol.TRUE : Symbol.FALSE;
                    else
                        res = _def.GetFloat(l.Value) < _def.GetFloat(r.Value) ? Symbol.TRUE : Symbol.FALSE;
                }
                else
                {
                    if(l.Type == TYPE_FLOAT)
                        res = _def.GetFloat(l.Value) < r.Value ? Symbol.TRUE : Symbol.FALSE;
                    else
                        res = l.Value < _def.GetFloat(r.Value) ? Symbol.TRUE : Symbol.FALSE;
                }
                break;
            case "<=":
                if (l.Type == r.Type)
                {
                    if (l.Type == TYPE_INT)
                        res = l.Value <= r.Value ? Symbol.TRUE : Symbol.FALSE;
                    else
                        res = _def.GetFloat(l.Value) <= _def.GetFloat(r.Value) ? Symbol.TRUE : Symbol.FALSE;
                }
                else
                {
                    if(l.Type == TYPE_FLOAT)
                        res = _def.GetFloat(l.Value) <= r.Value ? Symbol.TRUE : Symbol.FALSE;
                    else
                        res = l.Value <= _def.GetFloat(r.Value) ? Symbol.TRUE : Symbol.FALSE;
                }
                break;
            case "==":
                if (l.Type == r.Type)
                {
                    if (l.IsNull)
                        res = Symbol.FALSE;
                    else if (l.Type == TYPE_FLOAT)
                        res = _def.GetFloat(l.Value) == _def.GetFloat(r.Value) ? Symbol.TRUE : Symbol.FALSE;
                    else
                        res = l.Value == r.Value ? Symbol.TRUE : Symbol.FALSE;
                }
                else
                {
                    if (l.IsNull || r.IsNull)
                        res = Symbol.FALSE;
                    else if(l.Type == TYPE_FLOAT)
                        res = _def.GetFloat(l.Value) == r.Value ? Symbol.TRUE : Symbol.FALSE;
                    else
                        res = l.Value == _def.GetFloat(r.Value) ? Symbol.TRUE : Symbol.FALSE;
                }
                break;
            case "!=":
                if (l.Type == r.Type)
                {
                    if (l.IsNull)
                        res = Symbol.FALSE;
                    else if (l.Type == TYPE_INT)
                        res = l.Value != r.Value ? Symbol.TRUE : Symbol.FALSE;
                    else
                        res = _def.GetFloat(l.Value) != _def.GetFloat(r.Value) ? Symbol.TRUE : Symbol.FALSE;
                }
                else
                {
                    if (l.IsNull || r.IsNull)
                        res = Symbol.TRUE;
                    else if (l.Type == TYPE_FLOAT)
                        res = _def.GetFloat(l.Value) != r.Value ? Symbol.TRUE : Symbol.FALSE;
                    else
                        res = l.Value != _def.GetFloat(r.Value) ? Symbol.TRUE : Symbol.FALSE;
                }
                break;
            case "and":
                if (l.Type == r.Type)
                {
                    if (l.Type == TYPE_BOOLEN)
                    {
                        res = (l.Value > 0 && r.Value > 0) ? Symbol.TRUE : Symbol.FALSE;
                    }
                }
                break;
            case "or":
                if (l.Type == r.Type)
                {
                    if (l.Type == TYPE_BOOLEN)
                    {
                        res = (l.Value > 0 || r.Value > 0) ? Symbol.TRUE : Symbol.FALSE;
                    }
                }
                break;
            default:
                WLogger.Error("运算失败");
                res = Symbol.ERROR;
                break;
        }
        return res;
    }

    public override Symbol VisitExprGroup(WLangParser.ExprGroupContext context)
    {
        return context.children[1].Accept(this);
    }

    public override Symbol VisitPrimaryID(WLangParser.PrimaryIDContext context)
    {
        return currentScope.Resolve(context.i.Text);
    }

    public override Symbol VisitPrimaryINT(WLangParser.PrimaryINTContext context)
    {
        var res = new Symbol(int.Parse(context.i.Text), TYPE_INT);
        return res;
    }

    public override Symbol VisitPrimaryFLOAT(WLangParser.PrimaryFLOATContext context)
    {
        return DefineValue(float.Parse(context.i.Text));
    }

    public override Symbol VisitPrimarySTRING(WLangParser.PrimarySTRINGContext context)
    {
        return new Symbol(context.i.Text.Trim('"'), TYPE_STRING);
    }

    public override Symbol VisitPrimaryBOOL(WLangParser.PrimaryBOOLContext context)
    {
        string text = context.i.Text;
        return text == "true" ? Symbol.TRUE : Symbol.FALSE;
    }

    public override Symbol VisitPrimaryCHAR(WLangParser.PrimaryCHARContext context)
    {
        return new Symbol(context.i.Text, TYPE_CHAR);
    }
    
    public override Symbol VisitPrimaryNULL(WLangParser.PrimaryNULLContext context)
    {
        return Symbol.NULL;
    }

    public override Symbol VisitExprIntID(WLangParser.ExprIntIDContext context)
    {
        return currentScope.Resolve(context.i.Text);
    }

    public override Symbol VisitExprIntINT(WLangParser.ExprIntINTContext context)
    {
        return new Symbol(int.Parse(context.i.Text), TYPE_INT);
    }

    public override Symbol VisitExprExpr(WLangParser.ExprExprContext context)
    {
        return context.children[0].Accept(this);
    }

    public override Symbol VisitExprTable(WLangParser.ExprTableContext context)
    {
        return context.l.Accept(this);
    }
    
    public override Symbol VisitExprLambdaRef(WLangParser.ExprLambdaRefContext context)
    {
        return context.children[0].Accept(this);
    }

    public override Symbol VisitExprLambda(WLangParser.ExprLambdaContext context)
    {
        var m = Method.Get((list, interpreter) =>
        {
            SetRetrun(context.b.Accept(this));
        });
        return DefineValue(m);
    }

    public override Symbol VisitExprMethodRefNull(WLangParser.ExprMethodRefNullContext context)
    {
        return Symbol.NULL;
    }

    public override Symbol VisitExprMethodRefID(WLangParser.ExprMethodRefIDContext context)
    {
        return currentScope.Resolve(context.i.Text);
    }

    public override Symbol VisitExprMethodRefLambda(WLangParser.ExprMethodRefLambdaContext context)
    {
        return context.l.Accept(this);
    }

    public override Symbol VisitNumParam(WLangParser.NumParamContext context)
    {
        if (context.s.Type == WLangParser.ID)
            return currentScope.Resolve(context.s.Text);
        else if (context.s.Type == WLangParser.INT)
            return new Symbol(int.Parse(context.s.Text));
        else
            return DefineValue(float.Parse(context.s.Text));
        return Symbol.NULL;
    }


    public override Symbol VisitExprID(WLangParser.ExprIDContext context)
    {
        return currentScope.Resolve(context.i.Text);
    }
    public override Symbol VisitExprCommand(WLangParser.ExprCommandContext context)
    {
        return context.children[0].Accept(this);
    }

    private void PopScope()
    {
        var scope = LocalScope.Get(currentScope);
        currentScope = scope;
    }

    private void PushScope()
    {
        LocalScope.Push(currentScope);
        currentScope = currentScope.EnclosingScope;
    }

    public void SetRetrun(Symbol value)
    {
        Define(RETURN, value);
    }
    public void SetRetrun(bool value)
    {
        Define(RETURN, value);
    }
    public void SetRetrun(int value)
    {
        Define(RETURN, value);
    }
    public void SetRetrun(float value)
    {
        Define(RETURN, value);
    }
    public void SetRetrun(float[] value)
    {
        Define(RETURN, value);
    }
    public void SetRetrun(Vector3 value)
    {
        Define(RETURN, value);
    }
    
    public void SetRetrun(Vector2 value)
    {
        Define(RETURN, value);
    }
    
    public void SetRetrun(int[] value)
    {
        Define(RETURN, value);
    }
    public void SetRetrun(string value)
    {
        Define(RETURN, value);
    }
    public Symbol GetReturn()
    {
        return currentScope.Resolve(RETURN);
    }

    private void MultiplyTableAndNumber(ref Symbol table, ref Symbol num)
    {
        if (num.Type == TYPE_INT)
        {
            var list = _def.GetTable(table.Value);
            for (int i = 0; i < list.Count; i++)
            {
                var sym = list[i];
                if (sym.Type == TYPE_INT)
                    list[i] = new Symbol(sym.Value * num.Value);
                else if (sym.Type == TYPE_FLOAT)
                    list[i] = DefineValue(_def.GetFloat(sym.Value) * num.Value);
            }
        }
        else if (num.Type == TYPE_FLOAT)
        {
            var list = _def.GetTable(table.Value);
            for (int i = 0; i < list.Count; i++)
            {
                var sym = list[i];
                if (sym.Type == TYPE_INT)
                    list[i] = DefineValue(sym.Value * _def.GetFloat(num.Value));
                else if (sym.Type == TYPE_FLOAT)
                    list[i] = DefineValue(_def.GetFloat(sym.Value) * _def.GetFloat(num.Value));
            }
        }
    }
    
    public void ReleaseWObject(WObject obj)
    {
        _objectPool.PushObj(obj, Definition.Cached);
    }
    #region 解析参数方法

    private static bool CheckParamFail(in List<Symbol> param, int index, out Symbol sym)
    {
        if (index >= param.Count)
        {
            throw WLogger.ThrowArgumentError("参数数量不对,应该至少为: "+(index+1));
        }
        sym = param[index];
        return false;
    }
    
    public int ParseInt(in List<Symbol> param, int index)
    {
        if (CheckParamFail(param, index, out var sym)) return 0;
        if (sym.Type == TYPE_INT)
            return sym.Value;
        if (sym.Type == TYPE_FLOAT)
            return (int)_def.GetFloat(sym.Value);
        throw WLogger.ThrowArgumentError("参数类型错误");
    }

    public float ParseFloat(in List<Symbol> param, int index)
    {
        if (CheckParamFail(param, index, out var sym)) return 0;
        return sym.ToFloat(_def);
    }
    
    public Vector2 ParseVector2(in List<Symbol> param, int index)
    {
        if (CheckParamFail(param, index, out var sym)) return Vector2.zero;
        if (sym.Type == TYPE_TABLE)
        {
            var tbl = _def.GetTable(sym.Value);
            if(tbl.Count < 2)
                throw WLogger.ThrowArgumentError("参数类型错误");
            return new Vector2(tbl[0].ToFloat(_def), tbl[1].ToFloat(_def));
        }
        throw WLogger.ThrowArgumentError("参数类型错误");
    }
    
    public Vector3 ParseVector3(in List<Symbol> param, int index)
    {
        if (CheckParamFail(param, index, out var sym)) return Vector3.zero;
        if (sym.Type == TYPE_TABLE)
        {
            return sym.ToVector3(_def);
        }
        throw WLogger.ThrowArgumentError("参数类型错误");
    }

    public static bool ParseBool(in List<Symbol> param, int index)
    {
        if (CheckParamFail(param, index, out var sym)) return false;
        if (sym.Type == TYPE_BOOLEN)
        {
            return sym.IsTrue;
        }
        throw WLogger.ThrowArgumentError("参数类型错误");
    }

    public Method ParseMethod(in List<Symbol> param, int index)
    {
        if (CheckParamFail(param, index, out var sym)) return null;
        if (sym.Type == TYPE_METHOD)
        {
            return sym.ToMethod(_def);
        }
        
        throw WLogger.ThrowArgumentError("参数类型错误");
    }

    public Quaternion ParseQuaternion(in List<Symbol> param, int index)
    {
        if (CheckParamFail(param, index, out var sym)) return Quaternion.identity;
        if (sym.Type == TYPE_TABLE)
        {
            return sym.ToQuaternion(_def);
        }
        throw WLogger.ThrowArgumentError("参数类型错误");
    }

    #endregion
}
