using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using WGame.Util;

public class ReflectionHelper
{
    // public static Dictionary<string, string> GetFieldValues(object obj)
    // {
    //     return obj.GetType()
    //         .GetFields(BindingFlags.Public | BindingFlags.Static)
    //         .Where(f => f.FieldType == typeof(Action<GameEntity, int>))
    //         .ToDictionary(f => f.Name+f.Attributes,
    //             f => (string)f.GetValue(null));
    // }
    public static Dictionary<string, MethodInfo> GetFieldValues(Type obj)
    {
        var list = obj
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.Name != "DoAction")
            .ToList();
        var dict = obj
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.Name != "DoAction")
            .ToDictionary(f => f.Name,
                f => f);
        var infos = dict["DoGotHit"].GetParameters();
        return dict;
    }

    public static List<MethodInfo> GetStaticFunctionList(Type t, List<string> ignoreList = null)
    {
        if (ignoreList == null)
        {
            return t
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .ToList();
        }
        var list = t
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(f => !ignoreList.Contains(f.Name))
            .ToList();
        return list;
    }

    public static int GetConstIntValueAndName(Type type, ref string[] names, ref int[] values)
    {
        var list = type
            .GetFields(BindingFlags.Public | BindingFlags.Static);
        int count = 0;
        var len = list.Length;
        for (var i = 0; i < len; i++)
        {
            var info = list[i];
            if (info.Attributes.HasFlag(FieldAttributes.Literal | FieldAttributes.HasDefault)
                && info.Name != "Count"
                && info.FieldType == typeof(int))
            {
                list[count] = info;
                count++;
            }
        }

        var fieldNames = type.GetField("Names");
        bool hasNames = false;
        if (fieldNames != null)
        {
            names = (string[])fieldNames.GetValue(null);
            hasNames = true;
        }
        else
        {
            names = new string[count];
        }
        values = new int[count];
        for (var i = 0; i < count; i++)
        {
            var info = list[i];

            if (!hasNames)
            {
                var attrs = info.GetCustomAttributes(typeof(WLableAttribute), false);
                if (attrs.Length == 1)
                {
                    names[i] = ((WLableAttribute)attrs[0]).LabelText;
                }
                else
                {
                    names[i] = list[i].Name;
                }
            }

            values[i] = (int)list[i].GetRawConstantValue();
        }

        return count;
    }
}
