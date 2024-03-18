using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using WGame.Ability.Editor.Custom;
using WGame.Editor;

namespace WGame.Ability.Editor
{
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    internal sealed partial class AbilityEditWindow
    {
        internal void DrawData(object obj)
        {
            PropertyInfo[] pis = obj.GetType().GetProperties().OrderBy(p => p.MetadataToken).ToArray();
            for (int i = 0; i < pis.Length; ++i)
            {
                object[] attrs = pis[i].GetCustomAttributes(typeof(EditorDataAttribute), false);
                if (attrs.Length == 1)
                {
                    EditorDataAttribute epa = (EditorDataAttribute)attrs[0];
                    if (!string.IsNullOrEmpty(epa.Deprecated)) continue;

                    GUILayout.BeginHorizontal();
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
                                case EditorDataType.Enum:
                                    Helper.SetProperty(obj, pis[i].Name, EditorGUILayout.EnumPopup((Enum)val));
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
                                case EditorDataType.Action:
                                {
                                    // PopupList(obj, pis[i].Name, val, ActionList());
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
                    GUILayout.EndHorizontal();
                }
            }
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

        private void ListField(object obj, string propertyName, object val)
        {
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