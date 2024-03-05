using System;
using System.Collections.Generic;
using UnityEditor;

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
                                case EditorDataType.AnimatorState:
                                {
                                    // PopupList(obj, pis[i].Name, val, UnitWrapper.Instance.StateNameList);
                                }
                                    break;
                                case EditorDataType.AnimatorParam:
                                {
                                    // PopupList(obj, pis[i].Name, val, UnitWrapper.Instance.ParameterList);
                                }
                                    break;
                                case EditorDataType.CustomProperty:
                                {
                                    // PopupList(obj, pis[i].Name, val, ActionEngine.PropertyName.CustomPropertyList());
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

        private void ListField(object obj, string propertyName, object val)
        {
        }

        private void GameObjectField(object obj, string propertyName, object val)
        {
            GameObject go = null;
        }

        private void GameObjectListField(object obj, string propertyName, object val)
        {
        }
    }
}