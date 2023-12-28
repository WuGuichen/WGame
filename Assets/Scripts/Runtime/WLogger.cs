using System;
using System.Data;
using System.Text;
using UnityEngine;

public class WLogger
{
    private static StringBuilder _buf = new StringBuilder();

    public static void AppendBuffer(string str)
    {
        _buf.Append(str);
    }

    public static void PrintBuffer()
    {
        WLogger.Print(_buf.ToString());
        WTerminal.Output(_buf.ToString());
    }
    
    public static void PrintErrorBuffer()
    {
        WLogger.Error(_buf.ToString());
        WTerminal.Output(_buf.ToString());
    }

    public static void ClearBuffer()
    {
        _buf.Clear();
    }
    
    public static void Info(object msg)
    {
#if UNITY_EDITOR
        Debug.Log(msg);
#endif
        WTerminal.Output(msg.ToString());
    }
    
    public static void Print(object msg)
    {
#if UNITY_EDITOR
        StringBuilder buf = new StringBuilder();
        buf.Append("<color=#00ff00>");
        buf.Append(msg.ToString());
        buf.Append("</color>");
        Debug.Log(buf.ToString());
#endif
        WTerminal.Output(msg.ToString());
    }
    
    public static void PrintList(int[] list)
    {
#if UNITY_EDITOR
        string res = "[";
        for (int i = 0; i < list.Length; i++)
        {
            res += (list[i] + ",");
        }

        res = res.Substring(0, res.Length - 1);
        res += "]";
        Print(res);
#endif
    }
    
    public static void Println(string msg = null)
    {
        #if UNITY_EDITOR
        if (msg != null)
            Debug.Log(msg+"\n");
        else
            Debug.Log("\n");
        #endif
    }
    
    public static void Warning(string msg)
    {
        Debug.LogWarning(msg);
    }
    public static void Error(string msg)
    {
        Debug.LogError(msg);
    }
    public static void Exception(System.Exception msg)
    {
        Debug.LogException(msg);
    }

    public static InvalidExpressionException ThrowError(string msg)
    {
        return new SyntaxErrorException(msg);
    }

    public static ArgumentException ThrowArgumentError(string msg)
    {
        return new ArgumentException(msg);
    }
}
