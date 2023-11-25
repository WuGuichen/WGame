using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Motion
{
    class TriggerAnimationControl : NodeControl<TriggerAnimationNode>
    {
        protected List<TriggerItem> triggerTimeControls = new ();

        private string[] triggerTypes;
        private int triggerIndex;
        private float triggerParam;
        private bool isLoaded = false;
        private bool editParam = false;
        private bool showParam = true;
        private bool showTip = false;
        private string tipStr = "";
        private GUIStyle tipStyle = new GUIStyle();
        // private bool onDrag = false;
        private float dragTime = 0f;
        // private int nodeIndex = 0;
        
        public TriggerAnimationControl(WindowState state) : base(state)
        {
            timeStartControl.drawHead = false;
            timeStartControl.drawLine = false;
            timeStartControl.alwaysShowTooltip = false;
            tipStyle.fontSize = 10;
            tipStyle.normal.textColor = Color.white;
        }

        private void OnDragTrigger(TriggerItem item, double time)
        {
            if(time > 0 && time < state.maxTime)
                item.triggerTime = (int)(time * 1000);
            else
            {
                if (time <= 0.0f)
                    item.triggerTime = 0;
                if (time >= state.maxTime)
                    item.triggerTime = (int)(state.maxTime * 1000);
            }

            // onDrag = true;
            // dragTime = item.triggerTime * 0.001f;
            // timeStartControl.Draw(rightSize, state, item.triggerTime);
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
            AddManipulator(new AddAnimTimeTriggerMenuManipulator(state, this));
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

        public override Color color => Color.magenta * 0.5f;

        public override void OnLeftGUI()
        {
            if(!isLoaded)
                Load();
            GUILayout.BeginArea(leftSize);
            EditorGUILayout.BeginHorizontal();
            node.active = GUILayout.Toggle(node.active, "开启");
            editParam = GUILayout.Toggle(editParam, "编辑参数");
            showParam = GUILayout.Toggle(showParam, "显示参数");
            showTip = GUILayout.Toggle(showTip, "参数详情");
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(Node.Name);
            GUILayout.Label(tipStr, tipStyle);
            GUILayout.Label(dragTime.ToString(), GUILayout.Width(40));
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        public override void OnRightGUI()
        {
            DrawTimeRect(0,state.maxTime, color);
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
                else if (showTip)
                {
                    var width = 10f;
                    var rect = new Rect(item.Bounds.x + width, item.Bounds.y + width * 2, width * 6, width * 2);
                    EditorGUI.LabelField(rect, VisualizeHelper.GetTriggerNodeParamTip(item.triggerType, item.triggerParam));
                }
                else if (showParam)
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
                    tipStr = VisualizeHelper.GetTriggerNodeParamTip(item.triggerType, item.triggerParam);
                    dragTime = item.triggerTime * 0.001f;
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
    }
}