using System;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal struct GUIColorScope : IDisposable
    {
        private readonly Color color;
        public GUIColorScope(Color color)
        {
            this.color = GUI.color;
            GUI.color = color;
        }

        public void Dispose()
        {
            GUI.color = this.color;
        }
    }
}