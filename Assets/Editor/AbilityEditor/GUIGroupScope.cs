using System;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal struct GUIGroupScope : IDisposable
    {
        public GUIGroupScope(Rect rect)
        {
            GUI.BeginGroup(rect);
        }

        public void Dispose()
        {
            GUI.EndGroup();
        }
    }
}