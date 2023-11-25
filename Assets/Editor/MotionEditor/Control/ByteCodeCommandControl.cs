using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using WGame.Trigger;

namespace Motion
{
    class ByteCodeCommandControl : NodeControl<ByteCodeCommandNode>
    {
        private GUIStyle tipStyle = new GUIStyle();
        private bool isLoaded = false;
        private bool editParam = false;
        // private bool onDrag = false;
        private List<CodeCommandItem> triggerTimeControls = new ();
        private string codeStr = "";
        private int _codeNum;
        private bool isError;

        private int CodeNum
        {
            get => _codeNum;
            set
            {
                _codeNum = value;
                if (_codeNum < 0)
                {
                    codeStr = "栈溢出";
                    isError = true;
                }
            }
        }

        public ByteCodeCommandControl(WindowState state) : base(state)
        {
            timeStartControl.drawHead = true;
            timeStartControl.drawLine = true;
            timeStartControl.alwaysShowTooltip = true;
            tipStyle.fontSize = 10;
            tipStyle.normal.textColor = Color.white;
        }
        
        private void OnDragTrigger(CodeCommandItem item, double time)
        {
            if(time > 0 && time < state.maxTime)
                item.commandTime = (int)(time * 1000);
            else
            {
                if (time <= 0.0f)
                    item.commandTime = 0;
                if (time >= state.maxTime)
                    item.commandTime = (int)(state.maxTime * 1000);
            }

            // onDrag = true;
        }

        private void OnRightClickTrigger(CodeCommandItem item)
        {
            var menu = VisualizeHelper.ShowByteCodeCommandItemMenu(item, Save, OnDeleteTrigger);
            menu.ShowAsContext();
        }

        private void OnClickUp()
        {
            // onDrag = false;
            codeStr = null;
            Save();
        }

        void OnDeleteTrigger(object item)
        {
            RemoveTrigger((CodeCommandItem)item);
        }
        
        protected override void AddControlManipulator()
        {
            AddManipulator(new AddByteCodeCommandMenuManipulator(state, this));
        }

        public override bool HandleManipulatorsEvents(WindowState state)
        {
            // onDrag = false;
            var ret = false;
            triggerTimeControls.ForEach(trigger =>
            {
                if (!ret)
                    ret = trigger.HandleManipulatorsEvents(state);
            });
            if(!ret)
                return base.HandleManipulatorsEvents(state);
            return false;
        }

        public override Color color => Color.red * 0.5f;

        public override void OnLeftGUI()
        {
            if(!isLoaded)
                Load();
            GUILayout.BeginArea(leftSize);
            EditorGUILayout.BeginHorizontal();
            node.active = GUILayout.Toggle(node.active, "开启");
            GUILayout.Label(Node.Name);
            editParam = GUILayout.Toggle(editParam, "编辑参数");
            
            EditorGUILayout.EndHorizontal();
            if (codeStr != null)
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = isError ? Color.red : Color.white;
                style.fontSize = 12;
                GUILayout.Label(codeStr, style);
            }
            GUILayout.EndArea();
        }

        public override void OnRightGUI()
        {
            // DrawTimeRect(0,state.maxTime, color);
            DrawTimeRect(node.time, state.maxTime, color);
            triggerTimeControls.ForEach(item =>
            {
                item.Draw(rightSize, state, item.commandTime * 0.001f);
                if (editParam)
                {
                    var rect = new Rect(item.Bounds.x + 10, item.Bounds.y + 20, 50, 20);
                    var param = EditorGUI.IntField(rect, item.commandParam);
                    if (param != item.commandParam)
                    {
                        item.commandParam = param;
                        Save();
                    }
                }
                else
                {
                    VisualizeHelper.ShowByteCodeCommandItemTip(item);
                }
            });
        }

        private void OnMouseOverlay()
        {
            if (codeStr == null)
            {
                int i = 0;
                CodeNum = 0;
                isError = false;
                foreach (var item in triggerTimeControls)
                {
                    i++;
                    if (item.commandType == 0)
                        continue;
                    if (item.commandType is ByteCodeType.Literal_INT or ByteCodeType.Literal_FLOAT2 or ByteCodeType.GetMySelf or ByteCodeType.GetTarget)
                    {
                        CodeNum++;
                    }
                    else if (item.commandType == ByteCodeType.GetAttribute)
                    {
                        CodeNum--;
                        CodeNum++;
                    }
                    else if (item.commandType == ByteCodeType.Add
                             || item.commandType == ByteCodeType.Subtract
                             || item.commandType == ByteCodeType.Multiply
                             || item.commandType == ByteCodeType.Divide
                             || item.commandType == ByteCodeType.GetMax
                             || item.commandType == ByteCodeType.GetMin)
                    {
                        CodeNum--;
                        CodeNum--;
                        CodeNum++;
                    }
                    else if (item.commandType == ByteCodeType.End)
                    {
                        codeStr = "检错通过,剩余数据：" + CodeNum;
                        break;
                    }
                    else if (item.commandType == ByteCodeType.SetAttribute)
                    {
                        CodeNum--;
                        CodeNum--;
                    }
                    else
                    {
                        Debug.LogError("没有定义检错");
                    }

                    if (isError)
                    {
                        codeStr += (">> 第" + i + "个: ");
                        for (int ii = 0; ii < ByteCodeType.TriggerNames.Length; ii++)
                        {
                            if (ByteCodeType.Triggers[ii] == item.commandType)
                            {
                                codeStr += ByteCodeType.TriggerNames[ii];
                                break;
                            }
                        }
                        break;
                    }
                    if (item.isShowTip)
                    {
                        codeStr = "检错通过,剩余数据：" + CodeNum;
                        return;
                    }
                }
            }
        }

        public void AddCommand(float time, int type = 0, int param = 0)
        {
            var item = new CodeCommandItem(TimelineFuncHelper.timeCursor, state, null, OnMouseOverlay, OnClickUp, OnDragTrigger, true,
                OnRightClickTrigger, time);
            item.commandType = type;
            item.commandParam = param;
            triggerTimeControls.Add(item);
            if(isLoaded)
                Save();
        }

        public void RemoveTrigger(CodeCommandItem item)
        {
            triggerTimeControls.Remove(item);
            if(isLoaded)
                Save();
        }

        public void Load()
        {
            triggerTimeControls = new List<CodeCommandItem>();
            if (Node != null && Node.commandTime is { Length: > 0 })
            {
                for (var i = 0; i < Node.commandTime.Length; i++)
                {
                    AddCommand(Node.commandTime[i]*0.001f, Node.commandType[i], Node.commandParam[i]);
                }
            }
            
            // GetPopData(typeof(MainTypeDefine), ref mainValues, ref mainTypes);
            isLoaded = true;
        }

        public void Save()
        {
            var count = triggerTimeControls.Count;
            triggerTimeControls = triggerTimeControls.OrderBy(x => x.commandTime).ToList();
            Node.commandTime = new int[count];
            Node.commandType = new int[count];
            Node.commandParam = new int[count];
            for (int i = 0; i < count; i++)
            {
                var item = triggerTimeControls[i];
                Node.commandTime[i] = item.commandTime;
                Node.commandType[i] = item.commandType;
                Node.commandParam[i] = item.commandParam;
            }
        }

        private void GetPopData(Type type, ref int[] values, ref string[] names)
        {
            var infos = type.GetFields();
            var len = infos.Length;
            values = new int[len];
            names = new string[len];
            for (int i = 0; i < infos.Length; i++)
            {
                var v = (int)(infos[i].GetRawConstantValue());
                values[i] = v;
                names[i] = infos[i].Name;
            }
        }
    }
}