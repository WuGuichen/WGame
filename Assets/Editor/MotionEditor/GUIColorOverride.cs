using System;
using UnityEngine;

namespace Motion
{
    struct GUIColorOverride : IDisposable
    {
        private readonly Color _oldColor;

        public GUIColorOverride(Color newColor)
        {
            _oldColor = GUI.color;
            GUI.color = newColor;
        }

        public void Dispose()
        {
            GUI.color = _oldColor;
        }
    }
}
