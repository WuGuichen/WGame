using UnityEditor;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal static class TimeIndicator
    {
        private static readonly Tooltip tooltip = new Tooltip(AbilityEditWindow.Window.Setting.displayBackground,
            AbilityEditWindow.Window.Setting.tinyFont);
        private static float width = 11f;

        public static void Draw(AbilityEditWindow window, float time)
        {
            tooltip.text = window.FormatTime(time);

            float posX = window.Time2Pos(time);
            var bounds = new Rect(posX, 0, width, width);

            var tooltipBounds = tooltip.bounds;
            tooltipBounds.xMin = bounds.xMin - (tooltipBounds.width / 2.0f);
            tooltipBounds.y = bounds.y;
            tooltip.bounds = tooltipBounds;

            if (time >= 0)
            {
                tooltip.Draw();
                DrawLineAtTime(window, time, Color.black, true);
            }
        }

        public static void Draw(AbilityEditWindow window, float start, float end)
        {
            Draw(window, start);
            Draw(window, end);
        }

        public static void DrawLineAtTime(AbilityEditWindow window, float time, Color color, bool dotted = false)
        {
            var posX = window.Time2Pos(time);

            var p0 = new Vector3(posX, 0);
            var p1 = new Vector3(posX, window.rectTimeArea.height);

            if (dotted)
                DrawDottedLine(p0, p1, color);
            else
                DrawLine(p0, p1, color);
        }

        public static void DrawLine(Vector3 p1, Vector3 p2, Color color)
        {
            var c = Handles.color;
            Handles.color = color;
            Handles.DrawLine(p1, p2);
            Handles.color = c;
        }

        public static void DrawDottedLine(Vector3 p1, Vector3 p2, Color color)
        {
            var c = Handles.color;
            Handles.color = color;
            Handles.DrawDottedLine(p1, p2, 2);
            Handles.color = c;
        }

    }
}