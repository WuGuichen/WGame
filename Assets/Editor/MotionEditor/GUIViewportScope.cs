using System;
using UnityEngine;

namespace Motion
{
    struct GUIViewportScope : IDisposable
    {
        private bool _open;

        public GUIViewportScope(Rect position)
        {
            _open = false;
            if (Event.current.type == UnityEngine.EventType.Repaint || Event.current.type == UnityEngine.EventType.Layout)
            {
                GUI.BeginClip(position, -position.min, Vector2.zero, false);
                _open = true;
            }
        }

        public void Dispose()
        {
            if (_open)
            {
                GUI.EndClip();
                _open = false;
            }
        }
    }
}