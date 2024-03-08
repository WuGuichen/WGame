using UnityEditor;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal class SpliteWindow : WindowItemState
    {
        private readonly float headerSplitterWidth = 6f;
        private readonly float headerSplitterWidthVisual = 2f;
        
        [SerializeReference] private System.Action<float> setter = null;
        [SerializeReference] private System.Func<float> getter = null;
        
        public Rect manipulatorRect
        {
            get { return rect; }
        }
        
        public void Init(System.Action<float> setter, System.Func<float> getter)
        {
            this.setter = setter;
            this.getter = getter;

            rect = new Rect(getter() - headerSplitterWidth * 0.5f,
                Window.toobarHeight,
                headerSplitterWidthVisual,
                Window.rectWindow.height);

            AddManipulator(new SpliteWindowManipulator(this));
        }
        
        public override void Draw()
        {
            if (getter != null)
            {
                rect.x = getter() - headerSplitterWidth * 0.5f;
                rect.height = Window.rectWindow.height;
                EditorGUI.DrawRect(rect, Window.Setting.colorTopOutline3);

                if (GUIUtility.hotControl == 0)
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeLeftRight);
            }
        }
        
        public void OnMouseDrag(Event evt)
        {
            setter?.Invoke(evt.mousePosition.x);
        }
    }
}