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
}
