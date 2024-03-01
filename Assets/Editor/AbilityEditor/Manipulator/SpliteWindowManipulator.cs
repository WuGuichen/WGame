using UnityEditor;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal class SpliteWindowManipulator : IManipulator
    {
        private SpliteWindow owner;
        private bool captured = false;

        public SpliteWindowManipulator(SpliteWindow owner)
        {
            this.owner = owner;
        }
        
        protected override bool MouseDown(Event evt, AbilityEditWindow window)
        {
            if (owner.manipulatorRect.Contains(evt.mousePosition))
            {
                captured = true;
                window.AddCaptured(this);
                return true;
            }

            return false;
        }

        protected override bool MouseDrag(Event evt, AbilityEditWindow window)
        {
            if (!captured)
                return false;

            //window.headerWidth = evt.mousePosition.x;
            owner.OnMouseDrag(evt);

            return true;
        }

        protected override bool MouseUp(Event evt, AbilityEditWindow window)
        {
            if (!captured)
                return false;

            window.RemoveCaptured(this);
            captured = false;

            return true;
        }

        public override void Overlay(Event evt, AbilityEditWindow window)
        {
            Rect rect = window.rectWindow;
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeLeftRight);
        }
    }
}