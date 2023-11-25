using System;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Motion
{
    class NodeControl : Control
    {
        public EventNode node;

        public NodeControl(WindowState state)
        {
            this.state = state;
            
            timeStartControl = new TimeAreaItem(TimelineFuncHelper.timeCursor, state, OnDragHead, true);
            timeStartControl.drawHead = true;
            timeStartControl.drawLine = true;
            timeStartControl.lineColor = color;
            timeStartControl.headColor = color;
            AddControlManipulator();
        }
        
        public T GetNode<T>() where T : EventNode
        {
            return node as T;
        }

        protected virtual void OnDragHead(double newTime)
        {
            node.time = (float)Math.Max(0.0, newTime);
            timeStartControl.showTooltip = true;
        }

        public virtual float Height
        {
            get => 40;
        }

        public virtual float Width
        {
            get => 480;
        }
        public virtual Color color => Color.blue * 0.5f;
        
        public Rect size =>
            new Rect(start.x - TimelineFuncHelper.kDurationGuiThickness,
                    start.y - TimelineFuncHelper.kDurationGuiThickness,
                    state.window.position.width,
                    Height + 2 * TimelineFuncHelper.kDurationGuiThickness);
        public Vector2 start;
        public Rect leftSize => new Rect(start.x, start.y, Width, Height);
        public Rect rightSize => new Rect(start.x + Width, start.y, state.window.position.width - Width, Height);

        protected WindowState state;
        public TimeAreaItem timeStartControl;

        public void Draw(Vector2 startPosition)
        {
            start = startPosition;
            EditorGUI.DrawRect(size, color * 0.2f);
            
            OnLeftGUI();
            
            timeStartControl?.Draw(rightSize, state, node.time);
            
            OnRightGUI();
        }

        public virtual void OnLeftGUI()
        {
            GUILayout.BeginArea(leftSize);
            node.active = EditorGUILayout.Toggle("开启", node.active);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(node.Name);
            // node.luaConfig = GUILayout.TextField(node.luaConfig);
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        
        public virtual void OnRightGUI()
        { 
            
        }
        
        public virtual void OnEventTimeChange(double time)
        { 
            
        }

        protected Rect TimeToClipSize(float start, float end)
        {
            var duration = end - start;
            var x = state.TimeToPixel(start);
            var y = rightSize.y;
            var width = state.TimeToPixel(start + duration) - x;
            var height = rightSize.height;
            return new Rect(x, y, width, height);
        }

        protected void DrawTimeRect(float start, float end, Color color)
        {
            using (new GUIViewportScope(rightSize))
            {
                float duration = end - start;
                if (!Mathf.Approximately(duration, 0))
                {
                    var clipSize = TimeToClipSize(start, end);
                    EditorGUI.DrawRect(clipSize, color * 0.3f);
                    GUILayout.BeginArea(clipSize);
                    GUILayout.Label(string.Format("{0:F2}", duration));
                    GUILayout.EndArea();
                }
            }
        }

        public override bool HandleManipulatorsEvents(WindowState state)
        {
            var ret = false;
            if (timeStartControl == null)
                ret = false;
            else
            {
                ret = timeStartControl.HandleManipulatorsEvents(state);
            }
            if(!ret)
                return base.HandleManipulatorsEvents(state);
            return false;
        }
        
        protected virtual void AddControlManipulator()
        {
            
            AddManipulator(new DeleteEventMenuManipulator(state, this));
        }
        
        // public virtual void OnTrigger(HitInfo info)
        // { 
        //     
        // }
        //
        // public virtual void OnTriggerEnd()
        // {
        //
        // }
        //
        // internal class HitInfo
        // {
        //     public GameObject gameObject;
        //     public Transform transform;
        //     public Vector3 point;
        //     public float time;
        // }
    }

    class NodeControl<T> : NodeControl where T : EventNode
    {
        public T Node
        {
            get => node as T;
        }

        public NodeControl(WindowState state) : base(state)
        {
            
        }

    }
}