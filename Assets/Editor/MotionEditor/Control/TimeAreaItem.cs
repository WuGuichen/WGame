using System;
using System.Collections;
using System.Collections.Generic;
using NiceIO;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

namespace Motion
{
    class TimeAreaItem : Control
    {
        protected WindowState state;
        public Color headColor { get; set; }
        public Color lineColor { get; set; }
        public bool drawLine { get; set; }
        public bool drawHead { get; set; }
        public bool canMoveHead { get; set; }
        public string tooltip { get; set; }
        public Vector2 boundOffset { get; set; }
        
        readonly GUIContent _headerContent = new GUIContent();
        protected GUIStyle _style;
        protected Tooltip _tooltip;

        protected Rect _boundingRect;

        protected float widgetHeight => _style.fixedHeight;

        protected float widgetWidth => _style.fixedWidth;
        protected virtual string ToolTip => string.Format("{0:F2}", _time);
        protected double _time;

        public Rect Bounds
        {
            get => _boundingRect;
        }

        public GUIStyle Style
        {
            get => _style;
        }
        
        public bool showTooltip { get; set; }
        
        // is this the first frame the drag callback is being invoked
        public bool firstDrag { get; protected set; }

        public bool alwaysShowTooltip = false;

        public TimeAreaItem(GUIStyle style, WindowState state, Action<double> onDrag, bool alwaysShowTooltip = false)
        {
            this.state = state;
            _style = style;
            headColor = Color.white;
            this.alwaysShowTooltip = alwaysShowTooltip;
            
            InitItem(onDrag);
            
            lineColor = _style.normal.textColor;
            drawLine = true;
            drawHead = true;
            canMoveHead = false;
            tooltip = string.Empty;
            boundOffset = Vector2.zero;
            _tooltip = new Tooltip(TimelineFuncHelper.displayBackground, TimelineFuncHelper.tinyFont);
        }

        protected virtual void InitItem(Action<double> onDrag)
        {
            var scrub = new Scrub(
                (evt, st) =>
                {
                    firstDrag = true;
                    return st.window.TimeContentWithOffset.Contains(evt.mousePosition) &&
                           Bounds.Contains(evt.mousePosition);
                },
                (d) =>
                {
                    if (onDrag != null)
                        onDrag(d);
                    firstDrag = false;
                },
                () =>
                {
                    showTooltip = false;
                    firstDrag = false;
                });
            AddManipulator(scrub);
        }

        protected Rect contentSize;

        public void Draw(Rect rect, WindowState state, double time)
        {
            _time = time;
            contentSize = rect;
            var clipRect = new Rect(0.0f, 0.0f, state.window.position.width, state.window.position.height);
            clipRect.xMin += state.window.LeftContentHeaderSize.x;

            using (new GUIViewportScope(clipRect))
            {
                Vector2 windowCoordinate = rect.min;
                windowCoordinate.x = state.TimeToPixel(time);
                
                _boundingRect = new Rect(windowCoordinate.x - widgetWidth / 2.0f, windowCoordinate.y, widgetWidth, widgetHeight);
                // Do not paint if the time cursor goes outside the timeline bounds...
                if (Event.current.type == UnityEngine.EventType.Repaint)
                {
                    if (_boundingRect.xMax < state.window.TimeContent.xMin)
                        return;
                    if (_boundingRect.xMin > state.window.TimeContent.xMax)
                        return;
                }

                var top = new Vector3(windowCoordinate.x, rect.y);
                var bottom = new Vector3(windowCoordinate.x, rect.yMax);

                if (drawLine)
                {
                    Rect lineRect = Rect.MinMaxRect(top.x - 0.5f, top.y, bottom.x + 0.5f, bottom.y);
                    EditorGUI.DrawRect(lineRect, lineColor);
                }
                
                if (drawHead)
                {
                    Color c = GUI.color;
                    GUI.color = headColor;
                    GUI.Box(Bounds, _headerContent, Style);
                    GUI.color = c;

                    if (canMoveHead)
                        EditorGUIUtility.AddCursorRect(Bounds, MouseCursor.MoveArrow);
                }

                if (alwaysShowTooltip || showTooltip)
                {
                    _tooltip.text = ToolTip;

                    Vector2 position = Bounds.position;
                    //position.y = state.window.TimeContent.y;
                    position.y -= _tooltip.bounds.height;
                    position.x -= Mathf.Abs(_tooltip.bounds.width - Bounds.width) / 2.0f;

                    Rect tooltipBounds = Bounds;
                    tooltipBounds.position = position;
                    _tooltip.bounds = tooltipBounds;

                    _tooltip.Draw();
                }
            }
        }
    }
}