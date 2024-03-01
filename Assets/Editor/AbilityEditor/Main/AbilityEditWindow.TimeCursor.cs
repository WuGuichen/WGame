using UnityEngine;

namespace WGame.Ability.Editor
{
    internal enum AreaType
    {
        Header,
        Lines
    }
    internal sealed partial class AbilityEditWindow
    {
        private TimelineScrub _timeCursor;

        private void DrawTimeCursor(AreaType areaType)
        {
            if (_timeCursor == null || _timeCursor.style != Setting.timeCursor)
            {
                _timeCursor = new TimelineScrub(Setting.timeCursor, OnDragTimelineHead);
            }

            var isHeaderMode = areaType == AreaType.Header;
            DrawTimeCursor(isHeaderMode, !isHeaderMode);
        }

        private void DrawTimeCursor(bool drawHead, bool drawline)
        {
            _timeCursor.HandleManipulatorsEvents(this, Event.current);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (rectTimeRuler.Contains(Event.current.mousePosition))
                {
                    _timeCursor.HandleManipulatorsEvents(this, Event.current);
                    CurrentTime = Mathf.Max(0f, SnapTime(Pos2Time(Event.current.mousePosition.x)));
                }
            }

            _timeCursor.drawHead = drawHead;
            _timeCursor.drawLine = drawline;
            _timeCursor.Draw(CurrentTime);
        }
        
        
        private void OnDragTimelineHead(float newTime)
        {
            var t = Mathf.Max(0f, newTime);
            var delta = t - CurrentTime;
            Preview(delta);
            CurrentTime = t;
            _timeCursor.showTooltip = true;
        }
    }
}