using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;

public delegate void WLangFunc(List<Symbol> param, Interpreter interpreter);
public class Method : IType
{
    private static Stack<Method> pool = new Stack<Method>();
    
    public string Name { get; private set; }

    public static Method Get(Action<List<Symbol>, Interpreter> callback)
    {
        return Get("", callback);
    }
    public static Method Get(string name, Action<List<Symbol>, Interpreter> callback)
    {
        Method m = null;
        if (pool.Count > 0)
        {
            m = pool.Pop();
        }
        else
        {
            m = new Method();
        }

        m.SetFile(callback);
        m.Name = name;
        return m;
    }
    
    public static Method Get(string name, WLangParser.FileContext context, string[] param)
    {
        Method m = null;
        if (pool.Count > 0)
        {
            m = pool.Pop();
        }
        else
        {
            m = new Method();
        }

        m.Name = name;
        m.SetFile(context, param);
        return m;
    }

    public static void Push(Method m)
    {
        pool.Push(m);
    }
    
    protected WLangParser.FileContext file;

    private string[] parameters;

    public static Method Empty = new Method();

    private Action<List<Symbol>, Interpreter> callback;

    public void SetFile(Action<List<Symbol>, Interpreter> call)
    {
        callback = call;
    }
    
    public void SetFile(string code, string[] param)
    {
        var stream = new AntlrInputStream(code);
        var lexer = new WLangLexer(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new WLangParser(tokens);
        file = parser.file();
        parameters = param;
    }

    public void SetFile(WLangParser.FileContext fileContext, string[] param)
    {
        file = fileContext;
        parameters = param;
    }

    public Method()
    {
        callback = null;
        file = null;
    }

    private static List<Symbol> emptyParam = new List<Symbol>();
    public Symbol Call(Interpreter interpreter, string fileName)
    {
        return Call(emptyParam, interpreter, fileName);
    }
    public Symbol Call(List<Symbol> param, Interpreter interpreter, string fileName)
    {
        if (callback != null)
        {
            callback.Invoke(param, interpreter);
        }
        else
        {
            if (parameters != null)
            {
                int lenP = parameters.Length;
                while (param.Count < lenP)
                {
                    param.Add(Symbol.NULL);
                }

                for (int i = 0; i < lenP; i++)
                {
                    interpreter.Define(parameters[i], param[i]);
                }
            }

            return interpreter.ReVisitFile(file);
        }

        return interpreter.GetReturn();
    }

    public static Method PRINT = new Method()
    {
        Name = "PRINT",
        callback = ((list, interp) =>
        {
            StringBuilder buf = new StringBuilder();
            foreach (var sym in list)
            {
                buf.Append(sym.ToString(interp.Definition));
                buf.Append(',');
            }

            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            WLogger.WLangLog(buf.ToString().TrimEnd(','));
            interp.SetRetrun(Symbol.TRUE);
        })
    };
}
