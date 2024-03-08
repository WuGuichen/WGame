using UnityEditor;
using UnityEngine;

namespace WGame.Ability.Editor
{
    public enum EventHandleType
    {
        Left = 0,
        Right,
    }
    
    internal class EventHandler : WindowItemState
    {
        [SerializeField] private ActorEvent _parent = null;
        
        public ActorEvent parent
        {
            get => _parent;
            set => _parent = value;
        }
        [SerializeField] private EventHandleType _handleType = EventHandleType.Left;
        public EventHandleType handleType
        {
            get => _handleType;
            set => _handleType = value;
        }
        [SerializeField] private Color _color;
        public Color color
        {
            get => _color;
            set => _color = value;
        }

        public float time => handleType == EventHandleType.Left ? parent.start : parent.end;
        
        public void Init(ActorEvent parent, EventHandleType handleType)
        {
            this.parent = parent;
            this.handleType = handleType;

            color = Window.Setting.colorEventHandle;
        }

        public void BuildRect()
        {
            var x = Window.Time2Pos(time) + Window.rectTimeArea.x;
            if (handleType == EventHandleType.Right)
            {
                if (parent.duration == 0)
                {
                    x += Mathf.Max(Window.FrameWidth - 5, 0);
                }
                else
                {
                    x -= 5;
                }
            }
            rect = new Rect(x, parent.manipulatorRect.y, 5, parent.manipulatorRect.height);
        }
        
        public override string GetItemType()
        {
            return this.GetType().ToString();
        }

        public override void Draw()
        {
            BuildRect();

            using (new GUIColorScope(color))
            {
                GUI.Box(rect, "", Window.Setting.customEventKey);
            }

            if (GUIUtility.hotControl == 0)
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeLeftRight);
        }
        
        public void Move(Event evt)
        {
            var width = Window.Time2Pos(Window.Length);
            var posX = Mathf.Clamp(evt.mousePosition.x, Window.rectTimeArea.x, Window.rectTimeArea.x + width);
            var t = Window.SnapTime(Window.Pos2Time(posX));

            switch (handleType)
            {
                case EventHandleType.Left:
                    {
                        var limit = parent.end - Window.SnapInterval / 2;
                        var move = t <= limit ? parent.start - t : 0;
                        parent.start -= move;
                        parent.duration += move;
                    }
                    break;
                case EventHandleType.Right:
                    {
                        var limit = parent.start + Window.SnapInterval / 2;
                        var move = t >= limit ? parent.end - t : 0;
                        parent.duration -= move;
                    }
                    break;
            }
        }
    }
}