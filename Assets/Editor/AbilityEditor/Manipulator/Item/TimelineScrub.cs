using UnityEditor;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal class TimelineScrub : WindowItemState
    {
        private readonly GUIContent headerContent = new GUIContent();
        private GUIStyle _style;
        private Tooltip _tooltip;
        private Rect boundingRect;
        
        public Color headColor { get; set; }
        public Color lineColor { get; set; }
        public bool drawLine { get; set; }
        public bool drawHead { get; set; }
        public bool canMoveHead { get; set; }
        public string tooltip { get; set; }
        public Vector2 boundOffset { get; set; }
        public float widgetHeight { get { return _style.fixedHeight; } }
        public float widgetWidth { get { return _style.fixedWidth; } }
        
        public Rect bounds
        {
            get
            {
                Rect r = boundingRect;
                r.y = Window.rectTimeRuler.yMax - widgetHeight;
                r.position += boundOffset;

                return r;
            }
        }

        public GUIStyle style => _style;
        public bool showTooltip { get; set; }
        public bool firstDrag { get; private set; }

        public void Init(GUIStyle style, System.Action<float> onDrag)
        {
            drawLine = true;
            drawHead = true;
            canMoveHead = false;
            tooltip = string.Empty;
            boundOffset = Vector2.zero;
            headColor = Window.Setting.colorWhite;
            lineColor = style.normal.textColor;
            _style = style;

            _tooltip = new Tooltip(Window.Setting.displayBackground, Window.Setting.tinyFont);

            var scrub = new TimelineScrubManipulator(
                evt =>
                {
                    firstDrag = true;
                    var rect = Window.rectTimeRuler;
                    rect.x -= widgetWidth * 0.5f;
                    return rect.Contains(evt.mousePosition) && bounds.Contains(evt.mousePosition);
                },
                evt =>
                {
                    firstDrag = false;
                    canMoveHead = true;

                    var t = Window.Pos2Time(evt.mousePosition.x);
                    var snapT = Window.SnapTime(t);
                    onDrag?.Invoke(snapT);
                },
                evt =>
                {
                    canMoveHead = false;
                    showTooltip = false;
                    firstDrag = false;

                    if (Window.EnableSnap)
                    {
                        var t = Window.Pos2Time(evt.mousePosition.x);
                        var snapT = Window.SnapTime2(t);
                        Window.CurrentTime = Mathf.Max(0f, snapT);
                    }
                }
            );
            AddManipulator(scrub);
        }
        // public TimelineScrub(GUIStyle style, System.Action<float> onDrag)
        // {
        //     Init(style, onDrag);
        // }
        
        public void Draw(float time)
        {
            float posX = Window.Time2Pos(time) + Window.rectTimeArea.x;
            boundingRect = new Rect(posX - widgetWidth * 0.5f, Window.rectBody.y, widgetWidth, widgetHeight);

            if (Event.current.type == EventType.Repaint)
            {
                if (boundingRect.xMax < Window.rectTimeRuler.xMin)
                    return;
                if (boundingRect.xMin > Window.rectTimeRuler.xMax)
                    return;
            }

            if (drawLine)
            {
                Rect lineRect = new Rect(posX - 0.5f, Window.rectClient.y, 1, Window.rectTimeArea.height);
                EditorGUI.DrawRect(lineRect, lineColor);
            }

            if (drawHead && Event.current.type == EventType.Repaint)
            {
                using (new GUIColorScope(headColor))
                {
                    style.Draw(boundingRect, headerContent, false, false, false, false);
                }

                if (canMoveHead)
                    EditorGUIUtility.AddCursorRect(bounds, MouseCursor.MoveArrow);
            }

            if (showTooltip)
            {
                _tooltip.text = Window.FormatTime(time);

                Vector2 position = bounds.position;
                position.y = Window.rectTimeRuler.y;
                position.y -= _tooltip.bounds.height;
                position.x -= Mathf.Abs(_tooltip.bounds.width - bounds.width) / 2.0f;

                Rect tooltipBounds = bounds;
                tooltipBounds.position = position;
                _tooltip.bounds = tooltipBounds;

                _tooltip.Draw();
            }

        }
    }
}