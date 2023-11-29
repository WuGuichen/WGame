using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using UnityHFSM;

public class FSMInterpreter : WLangBaseVisitor<Symbol>
{
    private static Stack<FSMInterpreter> _pool = new Stack<FSMInterpreter>();

    public static FSMInterpreter Get(Interpreter interpreter,string fsmName, Scope scope, BaseDefinition definition)
    {
        if (_pool.Count > 0)
        {
            return _pool.Pop().Init(interpreter, fsmName,scope, definition);
        }

        return new FSMInterpreter(interpreter, fsmName, scope, definition);
    }

    public static void Push(FSMInterpreter interpreter)
    {
        interpreter.Release();
        _pool.Push(interpreter);
    }
    
    public const int TYPE_METHOD = BaseDefinition.TYPE_METHOD;

    private Scope currentScope;
    private BaseDefinition _def;
    private StateMachine<int,int,int> curFSM;
    private string fsmName;
    private Interpreter interpreter;
    
    public FSMInterpreter(Interpreter interpreter, string fsmName, Scope scope, BaseDefinition definition)
    {
        currentScope = scope;
        _def = definition;
        this.fsmName = fsmName;
        this.interpreter = interpreter;
    }

    private FSMInterpreter Init(Interpreter interpreter, string fsmName, Scope scope, BaseDefinition definition)
    {
        currentScope = scope;
        this.fsmName = fsmName;
        _def = definition;
        this.interpreter = interpreter;
        return this;
    }

    private void Release()
    {
        currentScope = null;
        _def = null;
        curFSM = null;
        interpreter = null;
    }
    
    public WFSM BuildWFSM(WLangParser.FsmBuilderContext context)
    {
        var code = context.fileCode();
        if (code != null)
        {
            context.Accept(interpreter);
        }
        var fsm = interpreter.ObjectPool.GetWFSM();
        curFSM = fsm.FSM;
        var blocks = context.fsmBlock();
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].Accept(this);
        }
        return fsm;
    }

    public override Symbol VisitFSMState(WLangParser.FSMStateContext context)
    {
        for (int i = 0; i < context.ChildCount; i++)
        {
            var child = context.children[i];
            if (child is WLangParser.FsmContentContext)
            {
                child.Accept(this);
            }
        }

        return Symbol.NULL;
    }

    public override Symbol VisitFSMCondition(WLangParser.FSMConditionContext context)
    {
        for (int i = 0; i < context.ChildCount; i++)
        {
            context.children[i].Accept(this);
        }
        return Symbol.NULL;
    }

    public override Symbol VisitFsmCondition(WLangParser.FsmConditionContext context)
    {
        var from = GetStateDefine(context.t.f).Value;
        var to = GetStateDefine(context.t.t).Value;
        var sym = context.c.Accept(interpreter);
        if (sym.Type == TYPE_METHOD)
        {
            var m = _def.GetMethod(sym.Value);
            var inter = interpreter;
            if (from >= 0)
            {
                curFSM.AddTransition(from, to
                    , transition => { return m.Call(inter).IsTrue; });
            }
            else
            {
                curFSM.AddTransitionFromAny(to
                    , transition =>
                    {
                        return m.Call(inter).IsTrue;
                    }
                );
            }
        }
        return Symbol.NULL;
    }

    public override Symbol VisitFsmContent(WLangParser.FsmContentContext context)
    {
        if (context.Parent is WLangParser.FSMStateContext)
        {
            var callbacks = new Action<State<int, int>>[3];
            if (context.ChildCount > 2)
            {
                if (context.children[1].GetText() == ":")  // 有后序方法
                {
                    var param = context.children[2];
                    int cbIndex = 0;
                    for (int i = 0; i < param.ChildCount; i++)
                    {
                        var child = param.GetChild(i);
                        if (child is TerminalNodeImpl == false)
                        {
                            var sym = param.GetChild(i).Accept(interpreter);
                            if (sym.Type == TYPE_METHOD)
                            {
                                var m = _def.GetMethod(sym.Value);
                                var inter = interpreter;
                                callbacks[cbIndex] = _ => { m.Call(inter); };
                            }
                            else
                            {
                                callbacks[cbIndex] = null;
                            }

                            cbIndex++;
                        }
                    }

                    curFSM.AddState(GetStateDefine(context.i).Value
                        , new State<int, int>(callbacks[0], callbacks[1], callbacks[2]));
                }
                else
                {
                    var subFSM = WLangMgr.Inst.GenFSM(context.f.Text, interpreter);
                    WBTreeMgr.Inst.SetFsmTree(context.f.Text, fsmName);
                    curFSM.AddState(GetStateDefine(context.i).Value, subFSM.FSM);
                }
            }
            else
            {
                // 空state
                curFSM.AddState(GetStateDefine(context.i).Value
                    , new State<int, int>());
            }
        } 
        return Symbol.NULL;
    }

    private Symbol GetStateDefine(string text)
    {
        return currentScope.Resolve(ParseDefineString("SD_", text));
    }
    private Symbol GetStateDefine(IToken token)
    {
        return currentScope.Resolve(ParseDefineString("SD_", token.Text));
    }

    public override Symbol VisitFsmTrigger(WLangParser.FsmTriggerContext context)
    {
        var from = GetStateDefine(context.t.f).Value;
        var trigger = GetStateDefine(context.i).Value;
        var to = GetStateDefine(context.t.t).Value;
        var sym = Symbol.NULL;
        if (context.ChildCount >= 4)
            sym = context.children[3].Accept(interpreter);
        if (sym.IsNull)
        {
            curFSM.AddTriggerTransition(trigger, from, to);
            return Symbol.NULL;
        }
        else
        {
            var m = _def.GetMethod(sym.Value);
            var inter = interpreter;
            curFSM.AddTriggerTransition(trigger, from, to, transition =>
            {
                return m.Call(inter).IsTrue;
            });
        }
        
        return Symbol.NULL;
    }

    public override Symbol VisitFSMTriggerTime(WLangParser.FSMTriggerTimeContext context)
    {
        for (int i = 0; i < context.ChildCount; i++)
            context.children[i].Accept(this);
        return Symbol.NULL;
    }

    public override Symbol VisitFSMStart(WLangParser.FSMStartContext context)
    {
        curFSM.SetStartState(GetStateDefine(context.s).Value);
        return Symbol.NULL;
    }

    public override Symbol VisitFSMExit(WLangParser.FSMExitContext context)
    {
        curFSM.AddExitTransition(GetStateDefine(context.s).Value);
        curFSM.needsExitTime = true;
        return Symbol.NULL;
    }

    public override Symbol VisitFsmTriggerTime(WLangParser.FsmTriggerTimeContext context)
    {
        var from = GetStateDefine(context.t.f).Value;
        var to = GetStateDefine(context.t.t).Value;
        float time = context.n.Accept(interpreter).ToFloat(_def);
        var sym = Symbol.NULL;
        if (context.ChildCount >= 4)
            sym = context.children[3].Accept(interpreter);
        if(sym.IsNull)
            curFSM.AddTransition(new TransitionAfter<int>(from, to, time));
        else
        {
            var m = _def.GetMethod(sym.Value);
            var inter = interpreter;
            curFSM.AddTransition(new TransitionAfter<int>(from, to, time, after =>
            {
                return m.Call(inter).IsTrue;
            }));}
        return Symbol.NULL;
    }

    public override Symbol VisitFSMTrigger(WLangParser.FSMTriggerContext context)
    {
        for (int i = 0; i < context.ChildCount; i++)
            context.children[i].Accept(this);
        return Symbol.NULL;
    }
    
    public static string ParseDefineString(string title, string origin)
    {
        var buf = new StringBuilder();
        ParseDefineString(title, origin, ref buf);
        return buf.ToString();
    }
    public static void ParseDefineString(string title, string origin, ref StringBuilder buf)
    {
        var chars = origin.ToCharArray();
        buf.Append(title);
        buf.Append(char.ToUpper(chars[0]));
        for (int j = 1; j < chars.Length; j++)
        {
            char c = chars[j];
            if (char.IsUpper(c) || char.IsDigit(c))
            {
                buf.Append('_');
            }
            else if (char.IsLower(c))
            {
                c = char.ToUpper(c);
            }

            buf.Append(c);
        }
    }
}
