using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using WGame.Trigger;

namespace Motion
{
    class EventTriggerControl : NodeControl<EventTriggerNode>
    {
        private GUIStyle tipStyle = new GUIStyle();
        private bool isLoaded = false;
        private bool editParam = false;
        // private bool onDrag = false;
        private List<TriggerItem> triggerTimeControls = new ();

        private static readonly string[] defaultTypes = new string[1] { "未定义" };
        private int indexMainType = 0;
        private string[] mainTypes = defaultTypes;
        private int[] mainValues;

        private int indexSubType = 0;
        private string[] subTypes = defaultTypes;
        private int[] subValues;
        private int subParam = 0;

        private int indexEventType;
        private string[] eventTypes =defaultTypes;
        private int[] eventValues;
        private int eventParam = 0;
        
        private int duration;
        private bool maxDuration = false;
        
        public EventTriggerControl(WindowState state) : base(state)
        {
            timeStartControl.drawHead = true;
            timeStartControl.drawLine = true;
            timeStartControl.alwaysShowTooltip = true;
            tipStyle.fontSize = 10;
            tipStyle.normal.textColor = Color.white;
        }
        
        private void OnDragTrigger(TriggerItem item, double time)
        {
            if(time > 0 && (state.maxTime > 0 && time < state.maxTime))
                item.triggerTime = (int)(time * 1000);
            else
            {
                if (time <= 0.0f)
                    item.triggerTime = 0;
                if (state.maxTime >0 && time >= state.maxTime)
                    item.triggerTime = (int)(state.maxTime * 1000);
            }

            // onDrag = true;
        }

        private void OnRightClickTrigger(TriggerItem item)
        {
            var menu = VisualizeHelper.ShowTriggerItemMenu(item, Save, OnDeleteTrigger);
            menu.ShowAsContext();
        }

        private void OnClickUp()
        {
            // onDrag = false;
            Save();
        }

        void OnDeleteTrigger(object item)
        {
            RemoveTrigger((TriggerItem)item);
        }
        
        protected override void AddControlManipulator()
        {
            AddManipulator(new AddEventTriggerMenuManipulator(state, this));
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

        public override Color color => Color.cyan * 0.5f;

        public override void OnLeftGUI()
        {
            if(!isLoaded)
                Load();
            GUILayout.BeginArea(leftSize);
            EditorGUILayout.BeginHorizontal();
            node.active = GUILayout.Toggle(node.active, "开启");
            GUILayout.Label(Node.Name);
            editParam = GUILayout.Toggle(editParam, "编辑参数");
            int maxTime = (int)((state.maxTime - Node.time) * 1000);
            if (maxTime < 0)
                maxTime = 0;
            var tmpFloat = EditorGUILayout.IntField("持续时间",duration);
            var tmpBool = GUILayout.Toggle(maxDuration, "到结束");
            if (tmpBool && maxDuration != tmpBool)
            {
                maxDuration = tmpBool;
                tmpFloat = maxTime;
            }
            if (tmpFloat != duration || tmpFloat > maxTime)
            {
                if (tmpFloat >= maxTime)
                {
                    tmpFloat = maxTime;
                }
                
                maxDuration = tmpFloat == maxTime;
                duration = tmpFloat;
                Node.duration = duration;
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            var tmp = EditorGUILayout.Popup(indexMainType, mainTypes);
            if (tmp != indexMainType)
            {
                indexMainType = tmp;
                OnMainTypeChanged();
                SaveEvent();
            }
            tmp = EditorGUILayout.Popup(indexSubType, subTypes);
            if (tmp != indexSubType)
            {
                indexSubType = tmp;
                SaveEvent();
            }
            tmp = EditorGUILayout.IntField(subParam, GUILayout.Width(50));
            if (tmp != subParam)
            {
                subParam = tmp;
                SaveEvent();
            }
            tmp = EditorGUILayout.Popup(indexEventType, eventTypes);
            if (tmp != indexEventType)
            {
                indexEventType = tmp;
                SaveEvent();
            }
            tmp = EditorGUILayout.IntField(eventParam, GUILayout.Width(50));
            if (tmp != eventParam)
            {
                eventParam = tmp;
                SaveEvent();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        public override void OnRightGUI()
        {
            // DrawTimeRect(0,state.maxTime, color);
            DrawTimeRect(node.time, node.time + Node.duration*0.001f, color);
            triggerTimeControls.ForEach(item =>
            {
                item.Draw(rightSize, state, item.triggerTime * 0.001f);
                if (editParam)
                {
                    var rect = new Rect(item.Bounds.x + 10, item.Bounds.y + 20, 100, 20);
                    var param = EditorGUI.IntField(rect, item.triggerParam);
                    if (param != item.triggerParam)
                    {
                        item.triggerParam = param;
                        Save();
                    }
                }
                else
                {
                    VisualizeHelper.ShowTriggerItemTip(item);
                }
            });
        }

        private void OnMouseOverlay()
        {
            foreach (var item in triggerTimeControls)
            {
                if (item.isShowTip)
                {
                    return;
                }
            }
        }

        public void AddTrigger(float time, int type = 0, int param = 0)
        {
            var item = new TriggerItem(TimelineFuncHelper.timeCursor, state, null, OnMouseOverlay, OnClickUp, OnDragTrigger, true,
                OnRightClickTrigger, time);
            item.triggerType = type;
            item.triggerParam = param;
            triggerTimeControls.Add(item);
            if(isLoaded)
                Save();
        }

        public void RemoveTrigger(TriggerItem item)
        {
            triggerTimeControls.Remove(item);
            if(isLoaded)
                Save();
        }

        public void Load()
        {
            triggerTimeControls = new List<TriggerItem>();
            if (Node != null && Node.triggerTime is { Length: > 0 })
            {
                for (var i = 0; i < Node.triggerTime.Length; i++)
                {
                    AddTrigger(Node.triggerTime[i]*0.001f, Node.triggerType[i], Node.triggerParam[i]);
                }
            }
            
            GetPopData(typeof(MainTypeDefine), ref mainValues, ref mainTypes);
            indexMainType = Node.eventType.mainType;
            OnMainTypeChanged();
            for (int i = 0; i < subValues.Length; i++)
            {
                if (subValues[i] == Node.eventType.subType)
                {
                    indexSubType = i;
                    break;
                }
            }

            subParam = Node.eventType.subTypeParam;

            for (int i = 0; i < eventValues.Length; i++)
            {
                if (eventValues[i] == Node.eventType.eventType)
                {
                    indexEventType = i;
                    break;
                }
            }

            eventParam = Node.eventType.eventParam;
            duration = Node.duration;
            int maxTime = (int)((state.maxTime - Node.time) * 1000);
            maxDuration = duration == maxTime;

            isLoaded = true;
        }

        public void Save()
        {
            var count = triggerTimeControls.Count;
            triggerTimeControls = triggerTimeControls.OrderBy(x => x.triggerTime).ToList();
            Node.triggerTime = new int[count];
            Node.triggerType = new int[count];
            Node.triggerParam = new int[count];
            for (int i = 0; i < count; i++)
            {
                var item = triggerTimeControls[i];
                Node.triggerTime[i] = item.triggerTime;
                Node.triggerType[i] = item.triggerType;
                Node.triggerParam[i] = item.triggerParam;
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

        private void OnMainTypeChanged()
        {
            var assembly = typeof(MainTypeDefine).Assembly;
            Type type = assembly.GetType("WGame.Trigger."+mainTypes[indexMainType] + "SubType");
            if (type == null)
                return;
            GetPopData(type, ref subValues, ref subTypes);
            if (indexSubType >= subValues.Length)
                indexSubType = subValues.Length - 1;
            type = assembly.GetType("WGame.Trigger." + mainTypes[indexMainType] + "Event");
            if (type == null)
                return;
            GetPopData(type, ref eventValues, ref eventTypes);
            if (indexEventType >= eventTypes.Length)
                indexEventType = eventTypes.Length - 1;
        }

        private void SaveEvent()
        {
            Node.eventType = new WtEventType()
            {
                mainType = mainValues[indexMainType],
                subType = subValues[indexSubType],
                subTypeParam = subParam,
                eventType = eventValues[indexEventType],
                eventParam = eventParam
            };
        }
    }
}