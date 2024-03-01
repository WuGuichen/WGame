using System;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal struct GUIStyleScope : IDisposable
    {
        readonly GUIStyle style;
        readonly Color oldColor;

        public GUIStyleScope(GUIStyle style, Color newColor)
        {
            this.style = style;
            oldColor = style.normal.textColor;
            style.normal.textColor = newColor;
        }

        public void Dispose()
        {
            style.normal.textColor = oldColor;
        }
    }
}