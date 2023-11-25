// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Reflection;
// using UnityEditor;
// using UnityEngine;
//
// namespace Motion
// {
//     class ConditionTriggerControl : NodeControl<ConditionTriggerNode>
//     {
//         // 此处变动需要与代码中使用配合
//         private Dictionary<int, Type> conditionTypeMap = new Dictionary<int, Type>()
//         {
//             // {ConditionType.OnButtonDown, typeof(EventButtonType)},
//             // {ConditionType.OnButtonUp, typeof(EventButtonType)},
//         };
//
//         private FieldInfo[] _conditionTypeInfos;
//         private int[] conditionTypes;
//         private string[] conditionTypesName;
//         private bool[] conditionSelectList;
//         
//         private FieldInfo[] _conditionInfos;
//         private int[] conditions = Array.Empty<int>();
//         private string[] conditionNames;
//         
//         private FieldInfo[] _actionTypeInfos;
//         private int[] actionTypes;
//         private string[] actionTypesName;
//         
//         public ConditionTriggerControl(WindowState state) : base(state)
//         {
//             conditionStyle.fixedWidth = 100;
//         }
//
//         // private void InitActionTypeInfo()
//         // {
//         //     if (_actionTypeInfos == null)
//         //     {
//         //         Type t = typeof(ActionType);
//         //         _actionTypeInfos = t.GetFields();
//         //     }
//         //     else
//         //     {
//         //         return;
//         //     }
//         //
//         //     actionTypes = new int[_actionTypeInfos.Length];
//         //     actionTypesName = new string[_actionTypeInfos.Length];
//         //     for (int i = 0; i < _actionTypeInfos.Length; i++)
//         //     {
//         //         actionTypes[i] = (int)(_actionTypeInfos[i].GetRawConstantValue());
//         //         actionTypesName[i] = _actionTypeInfos[i].Name;
//         //     }
//         // }
//         private void InitConditionTypeInfo()
//         {
//             if (_conditionTypeInfos == null)
//             {
//                 // Type t = typeof(WTEventType);
//                 // _conditionTypeInfos = t.GetFields();
//             }
//             else
//             {
//                 return;
//             }
//
//             conditionTypes = new int[_conditionTypeInfos.Length];
//             conditionTypesName = new string[_conditionTypeInfos.Length];
//             for (int i = 0; i < _conditionTypeInfos.Length; i++)
//             {
//                 conditionTypes[i] = (int)(_conditionTypeInfos[i].GetRawConstantValue());
//                 conditionTypesName[i] = _conditionTypeInfos[i].Name;
//             }
//         }
//
//         private Color _color = new Color(0, 1f, 1f, 0.5f);
//         public override Color color => _color;
//
//         private int conditionTypeIndex = -1;
//         private int actionTypeIndex = -1;
//         // private bool showConditionList = false;
//         private Rect leftSizeExtension;
//         private GUIStyle conditionStyle = new GUIStyle();
//         private bool isPresistent = false;
//
//         // ReSharper disable Unity.PerformanceAnalysis
//         public override void OnLeftGUI()
//         {
//             leftSizeExtension = new Rect(leftSize.x, leftSize.y, leftSize.width,
//                 leftSize.height + conditions.Length * 0);
//
//             GUILayout.BeginArea(leftSizeExtension);
//             
//             EditorGUILayout.BeginHorizontal();
//             node.active = GUILayout.Toggle(node.active, "开启", GUILayout.MaxWidth(80));
//             Node.duration = EditorGUILayout.IntField("持续", Node.duration, GUILayout.MinWidth(130));
//             isPresistent = Node.duration == Int32.MaxValue;
//             isPresistent = GUILayout.Toggle(isPresistent, "永久检测");
//             Node.clearOnExit = GUILayout.Toggle(Node.clearOnExit, "动作退出时清除");
//             if (isPresistent)
//                 Node.duration = Int32.MaxValue;
//             if (Node.duration <= 0)
//                 Node.duration = 0;
//             EditorGUILayout.EndHorizontal();
//             // var newConditionTypeIndex = EditorGUILayout.Popup(conditionTypeIndex, conditionTypesName, GUILayout.Width(100));
//             // // EditorGUILayout.Toggle("",showConditionList, GUILayout.Width(30));
//             // var newActionTypeIndex = EditorGUILayout.Popup(actionTypeIndex, actionTypesName, GUILayout.Width(100));
//             // UpdateActionType(newActionTypeIndex);
//             // showConditionList = EditorGUILayout.Foldout(showConditionList, "选择condition");
//             // if (showConditionList && conditionTypeIndex >= 0)
//             // {
//             //     // EditorGUILayout.BeginVertical();
//             //     GUILayout.Space(30);
//             //     for (var i = 0; i < conditionNames.Length; i++)
//             //     {
//             //         conditionSelectList[i] = EditorGUILayout.Toggle(conditionNames[i], conditionSelectList[i], GUILayout.Width(40));
//             //     }
//             //     // EditorGUILayout.EndVertical();
//             // }
//             // UpdateConditionType(newConditionTypeIndex);
//
//
//             EditorGUILayout.BeginHorizontal();
//             if (GUILayout.Button(VisualizeHelper.GetTriggerNodeDescription(Node)))
//             {
//                 // MotionInspector.SetTarget(Node);
//                 GUIUtility.ExitGUI();
//             }
//             EditorGUILayout.EndHorizontal();
//
//             GUILayout.EndArea();
//         }
//
//         private void UpdateConditionType(int type)
//         {
//             if (type != conditionTypeIndex)
//             {
//                 conditionTypeIndex = type;
//                 Debug.Log(conditionTypesName[type] + conditionTypes[type]);
//
//                 _conditionInfos = conditionTypeMap[1 << type].GetFields();
//                 conditions = new int[_conditionInfos.Length];
//                 conditionNames = new string[_conditionInfos.Length];
//                 conditionSelectList = new bool[_conditionInfos.Length];
//                 for (var i = 0; i < _conditionInfos.Length; i++)
//                 {
//                     conditions[i] = (int)(_conditionInfos[i].GetRawConstantValue());
//                     conditionNames[i] = _conditionInfos[i].Name;
//                 }
//             }
//         }
//
//         private void UpdateActionType(int type)
//         {
//             if (type != actionTypeIndex)
//             {
//                 actionTypeIndex = type;
//                 Debug.Log(actionTypesName[type] + actionTypes[type]);
//             }
//         }
//
//         public override void OnRightGUI()
//         {
//             DrawTimeRect(Node.time, Node.duration*0.001f + Node.time, color);
//             var clipRect = new Rect(0.0f, 0.0f, state.window.RightContentHeaderSize.width + 2000f, state.window.position.height);
//             clipRect.xMin += state.window.RightContentHeaderSize.x;
//             using (new GUIViewportScope(clipRect))
//             {
//                 float drawTime = node.time + Node.duration * 0.001f;
//                 float drawWidth = 1f;
//                 EditorGUI.DrawRect(
//                     new Rect(state.TimeToPixel(drawTime) - drawWidth / 2, rightSize.y, drawWidth, rightSize.height),
//                     Color.cyan * 0.8f);
//                 var clipSize = TimeToClipSize(Node.time, Node.time + 1000);
//                 clipSize = new Rect(clipSize.x, clipSize.y + 15, clipSize.width, clipSize.height);
//                 GUILayout.BeginArea(clipSize);
//                 string actionStr = VisualizeHelper.GetTriggerNodeActionDescription(Node);
//                 GUILayout.Label(actionStr);
//                 GUILayout.EndArea();
//             }
//         }
//         
//     }
// }