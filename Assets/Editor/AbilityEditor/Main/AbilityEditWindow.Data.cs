using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using WGame.Ability.Editor.Custom;
using WGame.Editor;
using WGame.Utils;

namespace WGame.Ability.Editor
{
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    internal sealed partial class AbilityEditWindow
    {
        internal void DrawData(object obj, bool hasGroup = true)
        {
            if (obj == null)
            {
                WLogger.Error("空对象");
                return;
            }
            PropertyInfo[] pis = obj.GetType().GetProperties().OrderBy(p => p.MetadataToken).ToArray();
            for (int i = 0; i < pis.Length; ++i)
            {
                object[] attrs = pis[i].GetCustomAttributes(typeof(EditorDataAttribute), false);
                if (attrs.Length == 1)
                {
                    EditorDataAttribute epa = (EditorDataAttribute)attrs[0];
                    if (!string.IsNullOrEmpty(epa.Deprecated)) continue;

                    if (hasGroup)
                    {
                        GUILayout.BeginHorizontal();
                    }
                    {
                        GUILayout.Label(epa.PropertyName, GUILayout.Width(epa.LabelWidth));

                        object val = Helper.GetProperty(obj, pis[i].Name);
                        if (epa.Edit)
                        {
                            switch (epa.DataType)
                            {
                                case EditorDataType.Bool:
                                    Helper.SetProperty(obj, pis[i].Name, GUILayout.Toggle((bool)val, ""));
                                    break;
                                case EditorDataType.Int:
                                    Helper.SetProperty(obj, pis[i].Name, EditorGUILayout.IntField((int)val));
                                    break;
                                case EditorDataType.Float:
                                    Helper.SetProperty(obj, pis[i].Name, EditorGUILayout.FloatField((float)val));
                                    break;
                                case EditorDataType.String:
                                    Helper.SetProperty(obj, pis[i].Name, EditorGUILayout.TextField((string)val));
                                    break;
                                case EditorDataType.Vector2:
                                    Helper.SetProperty(obj, pis[i].Name,
                                        EditorGUILayout.Vector2Field("", (Vector2)val));
                                    break;
                                case EditorDataType.Vector3:
                                    Helper.SetProperty(obj, pis[i].Name,
                                        EditorGUILayout.Vector3Field("", (Vector3)val));
                                    break;
                                case EditorDataType.Vector4:
                                    Helper.SetProperty(obj, pis[i].Name,
                                        EditorGUILayout.Vector4Field("", (Vector4)val));
                                    break;
                                case EditorDataType.Color:
                                    Helper.SetProperty(obj, pis[i].Name, EditorGUILayout.ColorField((Color)val));
                                    break;
                                case EditorDataType.Quaternion:
                                {
                                    Quaternion q = (Quaternion)val;
                                    q.eulerAngles = EditorGUILayout.Vector3Field("", q.eulerAngles);
                                    Helper.SetProperty(obj, pis[i].Name, q);
                                }
                                    break;
                                case EditorDataType.GameObject:
                                {
                                    GameObjectField(obj, pis[i].Name, val);
                                }
                                    break;
                                case EditorDataType.Lable:
                                    GUILayout.Label(val.ToString());
                                    break;
                                case EditorDataType.TypeID:
                                    PopupList(obj, pis[i].Name, val, StringToIDDefine.DefineDict[epa.Param]);
                                    break;
                                case EditorDataType.MaskTypeID:
                                    PopupMaskList(obj, pis[i].Name, val, StringToIDDefine.DefineDict[epa.Param]);
                                    break;
                                case EditorDataType.Enum:
                                    Helper.SetProperty(obj, pis[i].Name, EditorGUILayout.EnumPopup((Enum)val));
                                    break;
                                case EditorDataType.EnumNamed:
                                    PopupEnum(obj, pis[i].Name, val);
                                    break;
                                case EditorDataType.AnimationClip:
                                {
                                    AnimationClipField(obj, pis[i].Name, val);
                                }
                                    break;
                                case EditorDataType.BUffName:
                                {
                                }
                                    break;
                                case EditorDataType.NoticeReceiver:
                                {
                                    PopupList(obj, pis[i].Name, val, StringToIDDefine.Notice);
                                }
                                    break;
                                case EditorDataType.AttributeTypeID:
                                {
                                    PopupList(obj, pis[i].Name, val, StringToIDDefine.Attribute);
                                }
                                    break;
                                case EditorDataType.BuffDataTypeID:
                                {
                                    PopupList(obj, pis[i].Name, val, StringToIDDefine.BuffData);
                                }
                                    break;
                                case EditorDataType.ActionID:
                                {
                                    PopupList(obj, pis[i].Name, val, StringToIDDefine.Action);
                                } 
                                    break;
                                case EditorDataType.Param:
                                {
                                    SetParam(obj, pis[i].Name, val);
                                }
                                    break;
                                case EditorDataType.Object:
                                {
                                    DrawData(val);
                                }
                                    break;
                                case EditorDataType.List:
                                {
                                    ListField(obj, pis[i].Name, val);
                                }
                                    break;
                                case EditorDataType.GameObjectList:
                                {
                                    GameObjectListField(obj, pis[i].Name, val);
                                }
                                    break;

                            }
                        }
                        else
                        {
                            string sz = (val == null ? string.Empty : val.ToString());
                            GUILayout.Label(sz, GUILayout.Width(epa.LabelWidth));
                        }
                    }
                    if (hasGroup)
                    {
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }

        private void PopupList(object obj, string propertyName, object val, Enum list)
        {
        }
        
        private void PopupList(object obj, string propertyName, object val, List<string> list)
        {
            int idx = Helper.GetStringIndex(list, (string)val);
            idx = EditorGUILayout.Popup(idx, list.ToArray());
            if (idx >= 0 && idx < list.Count)
            {
                Helper.SetProperty(obj, propertyName, list[idx]);
            }
        }

        private void SetParam(object obj, string propertyName, object val)
        {
            if (obj is CustomParam param)
            {
                switch (param.ParamType)
                {
                    case DataType.Bool:
                        var res = EditorGUILayout.Toggle(param.Value.AsBool());
                        TAny.Set(param.Value, res);
                        break;
                    case DataType.Int:
                        TAny.Set(param.Value, EditorGUILayout.IntField(param.Value.AsInt()));
                        break;
                    case DataType.Long:
                        TAny.Set(param.Value, EditorGUILayout.LongField(param.Value.AsLong()));
                        break;
                    case DataType.ULong:
                        TAny.Set(param.Value, EditorGUILayout.LongField((long)param.Value.AsULong()));
                        break;
                    case DataType.Float:
                        TAny.Set(param.Value, EditorGUILayout.FloatField(param.Value.AsFloat()));
                        break;
                    case DataType.String:
                        TAny.Set(param.Value, EditorGUILayout.TextField(param.Value.AsString()));
                        break;
                    case DataType.Vector2:
                        TAny.Set(param.Value, EditorGUILayout.Vector2Field("",param.Value.AsVector2()));
                        break;
                    case DataType.Vector2Int:
                        TAny.Set(param.Value, EditorGUILayout.Vector2IntField("",param.Value.AsVector2Int()));
                        break;
                    case DataType.Vector3:
                        TAny.Set(param.Value, EditorGUILayout.Vector3Field("",param.Value.AsVector3()));
                        break;
                    case DataType.Vector3Int:
                        TAny.Set(param.Value, EditorGUILayout.Vector3IntField("",param.Value.AsVector3Int()));
                        break;
                    case DataType.Vector4:
                        TAny.Set(param.Value, EditorGUILayout.Vector4Field("",param.Value.AsVector4()));
                        break;
                    case DataType.Quaternion:
                        TAny.Set(param.Value, EditorGUILayout.Vector4Field("",param.Value.AsVector4()));
                        break;
                    case DataType.Color:
                        TAny.Set(param.Value, EditorGUILayout.ColorField("",param.Value.AsColor()));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Helper.SetProperty(obj, propertyName, EditorGUILayout.EnumPopup((Enum)val, GUILayout.Width(80)));
            }

            // Helper.SetProperty(obj, propertyName, newValue);
        }

        private void PopupMaskList(object obj, string propertyName, object val, StringToIDDefine define)
        {
            // var intValue = EditorGUI.MaskField(EditorGUILayout.GetControlRect() ,);
            var intValue = EditorGUILayout.MaskField("", (int)val, define.StringArray);
            Helper.SetProperty(obj, propertyName, intValue);
        }
        
        private void PopupList(object obj, string propertyName, object val, StringToIDDefine define)
        {
            int idx = define.GetIndex((int)val);
            var list = define.StringArray;
            idx = EditorGUILayout.Popup(idx, list);
            if (idx >= 0 && idx < list.Length)
            {
                Helper.SetProperty(obj, propertyName, define.IDArray[idx]);
            }
        }

        private void PopupEnum(object obj, string propertyName, object val)
        {
            var type = val.GetType();
            StringToIDDefine define = StringToIDDefine.DefineTypeDict[type];
            int idx = define.GetIndex((int)val);
            var list = define.StringArray;
            idx = EditorGUILayout.Popup(idx, list);
            if (idx >= 0 && idx < list.Length)
            {
                Helper.SetProperty(obj, propertyName, Enum.ToObject(type, define.IDArray[idx]));
            }
        }

        private void ListField(object obj, string propertyName, object val)
        {
            var itemType = typeof(bool);
            var list = val as System.Collections.IList;
            foreach (var interfaceType in val.GetType().GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    itemType = val.GetType().GetGenericArguments()[0];
                    break;
                }
            }

            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    using (new GUIColorScope(Setting.colorInspectorLabel))
                    {
                        GUILayout.Label("Size");
                    }
                    int count = list.Count;
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("总数:"+count);
                            if (GUILayout.Button("+"))
                            {
                                count++;
                            }
                            if (GUILayout.Button("-"))
                            {
                                count--;
                            }

                            if (GUILayout.Button("Clear"))
                            {
                                count = 0;
                            }
                        }
                        GUILayout.EndHorizontal();
                    if (count < list.Count)
                    {
                        int idx = list.Count;
                        while (idx > count)
                        {
                            list.RemoveAt(idx - 1);
                            idx--;
                        }
                    }
                    else if (count > list.Count)
                    {
                        int num = count - list.Count;
                        for (int j = 0; j < num; ++j)
                        {
                            TypeCode typeCode = Type.GetTypeCode(itemType);
                            if (typeCode == TypeCode.String)
                            {
                                list.Add(string.Empty);
                            }
                            else
                            {
                                list.Add(System.Activator.CreateInstance(itemType));
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();

                for (int j = 0; j < list.Count; ++j)
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal(GUILayout.MaxWidth(200f));
                        GUILayout.Label(j.ToString(), GUILayout.Width(10));
                        if (itemType.IsEnum)
                        {
                            list[j] = EditorGUILayout.EnumPopup((Enum)list[j]);
                        }
                        else
                        {
                            var typeCode = Type.GetTypeCode(itemType);
                            switch (typeCode)
                            {
                                case TypeCode.Int32:
                                    list[j] = EditorGUILayout.IntField((int)list[j]);
                                    break;
                                case TypeCode.Single:
                                    list[j] = EditorGUILayout.FloatField((float)list[j]);
                                    break;
                                case TypeCode.String:
                                    list[j] = EditorGUILayout.TextField((string)list[j]);
                                    break;
                                case TypeCode.Object:
                                    DrawData(list[j]);
                                    break;
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();

            Helper.SetProperty(obj, propertyName, list);
        }

        private void AnimationClipField(object obj, string propertyName, object val)
        {
            AnimationClip clip = null;
            var szPath = (string)val;

            if (!string.IsNullOrEmpty(szPath))
            {
                if (animationClip2stringDic.ContainsKey(szPath))
                {
                    clip = animationClip2stringDic[szPath];
                }
                else
                {
                    clip = GameAssetsMgr.Inst.LoadAnimClip(szPath);
                    if (clip != null && !animationClip2stringDic.ContainsKey(szPath))
                    {
                        animationClip2stringDic.Add(szPath, clip);
                    }
                }
            }
            
            clip = EditorGUILayout.ObjectField(clip, typeof(AnimationClip), true) as AnimationClip;
            var name = AssetDatabase.GetAssetPath(clip);

            Helper.SetProperty(obj, propertyName, (clip != null ? Path.GetFileNameWithoutExtension(name) : string.Empty));
        }
        private void GameObjectField(object obj, string propertyName, object val)
        {
            GameObject go = null;
            var szPath = (string)val;

            if (!string.IsNullOrEmpty(szPath))
            {
                if (gameobject2stringDic.ContainsKey(szPath))
                {
                    go = gameobject2stringDic[szPath];
                }
                else
                {
                    go = GameAssetsMgr.Inst.LoadObject<GameObject>(szPath);
                    if (go != null && !gameobject2stringDic.ContainsKey(szPath))
                    {
                        gameobject2stringDic.Add(szPath, go);
                    }
                }
            }
            
            go = EditorGUILayout.ObjectField(go, typeof(GameObject), true) as GameObject;
            var name = GameAssetsMgr.Inst.FormatResourceName(AssetDatabase.GetAssetPath(go));

            Helper.SetProperty(obj, propertyName, (go != null ? name : string.Empty));
        }

        private void GameObjectListField(object obj, string propertyName, object val)
        {
        }
    }
}