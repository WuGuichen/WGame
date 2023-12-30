using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct Symbol
{
    public string Text;
    public int Value;
    public int Type;

    public Symbol(int value, int type, string text)
    {
        Value = value;
        Type = type;
        Text = text;
    }

    public Symbol(int value, int type)
    {
        Value = value;
        Type = type;
        if (type == BaseDefinition.TYPE_BOOLEN)
        {
            Text = value > 0 ? "true" : "false";
        }
        else
        {
            Text = null;
        }
    }
    
    public Symbol(string text, int type)
    {
        Value = 0;
        Type = type;
        Text = text;
    }

    public Symbol(string text)
    {
        Value = 0;
        Type = BaseDefinition.TYPE_STRING;
        Text = text;
    }
    
    public Symbol(int value)
    {
        Value = value;
        Type = BaseDefinition.TYPE_INT;
        Text = null;
    }

    public override string ToString()
    {
        if (Text != null)
            return Text;
        return Value.ToString();
    }

    public string ToString(BaseDefinition def)
    {
        if (Type == BaseDefinition.TYPE_TABLE)
        {
            var list = def.GetTable(Value);
            StringBuilder buf = new StringBuilder();
            buf.Append('[');
            for (int i = 0; i < list.Count; i++)
            {
                buf.Append(list[i].ToString(def));
                buf.Append(",");
            }

            buf.Remove(buf.Length - 1, 1);
            buf.Append("]");
            return buf.ToString();
        }
        else
        {
            if (Text != null)
                return Text;
            return Value.ToString();
        }
    }

    public override int GetHashCode()
    {
        return (Value << 4) + Type;
    }

    public static Symbol ERROR = new Symbol(0, -1, "error");
    public static Symbol NULL = new Symbol(0, 0, "null");
    public static Symbol TRUE = new Symbol(1, BaseDefinition.TYPE_BOOLEN, "true");
    public static Symbol FALSE = new Symbol(0, BaseDefinition.TYPE_BOOLEN, "false");
    public static Symbol PASS = new Symbol(0, 0, "pass");

    public bool IsTrue => (Type != 0 && Value > 0);
    public bool IsFalse => (Type == 0 || Value <= 0);
    public bool IsNull => Type == 0;
    public bool IsNotNull => Type != 0;
    public Symbol Copy() => new Symbol(Value, Type, Text);
}

public static class SymbolExtention
{
    public static float ToFloat(this Symbol sym, in BaseDefinition def)
    {
        if(sym.Type == BaseDefinition.TYPE_FLOAT)
            return def.GetFloat(sym.Value);
        else if (sym.Type == BaseDefinition.TYPE_INT)
            return sym.Value;
        throw WLogger.ThrowError("类型转换错误");
    }
    
    public static List<Symbol> ToTable(this Symbol sym, in BaseDefinition def)
    {
        return def.GetTable(sym.Value);
    }
    public static Method ToMethod(this Symbol sym, in BaseDefinition def)
    {
        return def.GetMethod(sym.Value);
    }
    
    public static Vector3 ToVector3(this Symbol sym, in BaseDefinition def)
    {
        if (sym.Type != BaseDefinition.TYPE_TABLE)
        {
            throw WLogger.ThrowError("类型转换错误");
        }
        var table = def.GetTable(sym.Value);
        if(table.Count >= 3)
            return new Vector3(def.GetFloat(table[0].Value), def.GetFloat(table[1].Value), def.GetFloat(table[2].Value));
        throw WLogger.ThrowError("类型转换失败");
    }

    public static Quaternion ToQuaternion(this Symbol sym, BaseDefinition def)
    {
        if (sym.Type != BaseDefinition.TYPE_TABLE)
        {
            throw WLogger.ThrowError("类型转换错误");
        }
        var table = def.GetTable(sym.Value);
        if (table.Count < 3)
        {
            throw WLogger.ThrowError("类型转换失败, 参数数量不对");
        }

        if (table.Count == 3)
        {
            return Quaternion.Euler(def.GetFloat(table[0].Value), def.GetFloat(table[1].Value), def.GetFloat(table[2].Value));
        }
        else
        {
            return new Quaternion(def.GetFloat(table[0].Value), def.GetFloat(table[1].Value),
                def.GetFloat(table[2].Value), def.GetFloat(table[3].Value));
        }
    }
}
