using System;
using System.Collections.Generic;
using System.Reflection;
using Motion;
using UnityEditor;
using UnityEngine;
using WGame.Attribute;

class VisualizeHelper
{
    private static Dictionary<ConditionTriggerNode, string> triggerDescCache = new Dictionary<ConditionTriggerNode, string>();
    private static Dictionary<ConditionTriggerNode, string> triggerActionDescCache = new Dictionary<ConditionTriggerNode, string>();
    private static Dictionary<Type, FieldInfo[]> infoDict = new();

    public static void ClearTriggerDescCache()
    {
        triggerDescCache.Clear();
    }

    public static string GetTriggerNodeParamTip(int paramType, int param)
    {
        string res = "";
        switch (paramType)
        {
            case AnimTriggerType.AnimSpeed:
                res += "动画播放的倍率，不支持负数";
                break;
            case AnimTriggerType.Rotate:
                res += "旋转倍率，不支持负数";
                break;
            case AnimTriggerType.RootMotion:
                res += "大于0时控制动画根运动影响比例";
                break;
            case AnimTriggerType.Loop:
                res += "动画循环, 成对使用，第一个参数无意义，\n第二个参数大于0可以重置范围内trigger，大于100可以重置动画";
                break;
            case AnimTriggerType.ThrustUp:
                res += "添加一个向上运动的力";
                break;
            case AnimTriggerType.OpenSensor:
                res += "武器探测，大于0开启，否则关闭";
                break;
            case AnimTriggerType.Move:
                res += "移动倍率，当小于0的时候锁定平面速率";
                break;
            case AnimTriggerType.TransMotionType:
                res += "切换动作到动作类型，类型："+GetValueName(typeof(MotionType) ,param, MotionType.Count);
                break;
            default:
                res += "未定义";
                break;
        }
        return res;
    }

    public static string GetValueName(Type type, int value, int len = 0)
    {
        if (!infoDict.TryGetValue(type, out var infos))
        {
            infos = type.GetFields();
        }
        if (len == 0)
            len = infos.Length;
        if (len >= infos.Length)
            len = infos.Length;
        for (int i = 0; i < len; i++)
        {
            var v = (int)(infos[i].GetRawConstantValue());
            if (v == value)
                return infos[i].Name;
        }

        return "未定义";
    }

    public static void Reset()
    {
        infoDict.Clear();
        triggerDescCache.Clear();
        triggerActionDescCache.Clear();
    }

    public static void ShowByteCodeCommandItemTip(CodeCommandItem item)
    {
        var helper = MotionHelper.Inst;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        int param = item.commandParam;
        var lable = new GUIContent(helper.ParseCommandParam(item.commandType, param));
        // var lable = new GUIContent(param.ToString());
        style.fontSize = 10;
        style.fixedWidth = 50;
        style.wordWrap = true;
        style.alignment = TextAnchor.MiddleLeft;
        var height = style.CalcHeight(lable, style.fixedWidth);
        while (height > 32)
        {
            style.fontSize--;
            height = style.CalcHeight(lable, style.fixedWidth);
            if(style.fontSize <= 1)
                break;
        }
        var width = 14f;
        var rect = new Rect(item.Bounds.x + width, item.Bounds.y + 26, 2, 10);
        EditorGUI.LabelField(rect, lable, style);
    }

    public static GenericMenu ShowByteCodeCommandItemMenu(CodeCommandItem item, Action OnSelected,
        GenericMenu.MenuFunction2 OnDelect)
    {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("删除命令"),false, OnDelect, item);
        var itemNames = ByteCodeType.TriggerNames;
        for (int i = 0; i < itemNames.Length; i++)
        {
            menu.AddItem(new GUIContent("选择命令/" + itemNames[i]), false, (index) =>
            {
                item.commandType = ByteCodeType.Triggers[(int)index];
                OnSelected();
            }, i);
        }

        if (item.commandType == ByteCodeType.GetAttribute || item.commandType == ByteCodeType.SetAttribute)
        {
            itemNames = WAttrType.Names;
            for (int i = 0; i < itemNames.Length; i++)
            {
                menu.AddItem(new GUIContent("选择参数/" + itemNames[i]), false, (index) =>
                {
                    item.commandParam = (int)index;
                    OnSelected?.Invoke();
                }, i);
            }
        }

        return menu;
    }

    public static void ShowTriggerItemTip(TriggerItem item)
    {
        var helper = MotionHelper.Inst;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        int param = item.triggerParam;
        var lable = new GUIContent(helper.ParseDesc(item.triggerType, param));
        style.fontSize = 10;
        style.fixedWidth = 50;
        style.wordWrap = true;
        style.alignment = TextAnchor.MiddleLeft;
        var height = style.CalcHeight(lable, style.fixedWidth);
        while (height > 32)
        {
            style.fontSize--;
            height = style.CalcHeight(lable, style.fixedWidth);
            if(style.fontSize <= 1)
                break;
        }
        var width = 14f;
        var rect = new Rect(item.Bounds.x + width, item.Bounds.y + 26, 2, 10);
        EditorGUI.LabelField(rect, lable, style);
    }

    public static GenericMenu ShowTriggerItemMenu(TriggerItem item, Action OnSelected, GenericMenu.MenuFunction2 OnDelect)
    {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("删除事件"),false, OnDelect, item);
        var triggerNames = AnimTriggerType.TriggerNames;
        for (int i = 0; i < triggerNames.Length; i++)
        {
            menu.AddItem(new GUIContent("选择事件/" + triggerNames[i]), false, (index) =>
            {
                item.triggerType = AnimTriggerType.Triggers[(int)index];
                OnSelected();
            }, i);
        }

        if (item.triggerType == AnimTriggerType.TransMotionType)
        {
            var infos = typeof(MotionType).GetFields();
            for (int i = 0; i < MotionType.Count; i++)
            {
                var name = infos[i].Name;
                menu.AddItem(new GUIContent("选择参数/" + name), false, (index) =>
                {
                    item.triggerParam = (int)(MotionType.EnumList[(int)index]);
                    OnSelected();
                }, i);
            }
        }
        else if (item.triggerType == AnimTriggerType.SwitchMotion)
        {
            for (int i = 0; i < MotionIDs.IDList.Length; i++)
            {
                var name = MotionIDs.NameList[i];
                menu.AddItem(new GUIContent("选择参数/" + name), false, (index) =>
                {
                    item.triggerParam = MotionIDs.IDList[(int)index];
                    OnSelected();
                }, i);
            }
        }
        else if (item.triggerType == AnimTriggerType.OpenSensor || item.triggerType == AnimTriggerType.CloseSensor)
        {
            for (int i = 0; i < SensorType.list.Length; i++)
            {
                var name = SensorType.list[i];
                menu.AddItem(new GUIContent("选择参数/" + name), false, (index) =>
                {
                    item.triggerParam = SensorType.IDs[(int)index];
                    OnSelected();
                }, i);
            }
        }

        return menu;
    }
}
