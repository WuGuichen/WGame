using UnityEditor;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal abstract class IManipulator
    {
        private int controlId = 0;

        protected virtual bool MouseDown(Event evt, AbilityEditWindow window) { return false; }
        protected virtual bool MouseDrag(Event evt, AbilityEditWindow window) { return false; }
        protected virtual bool MouseUp(Event evt, AbilityEditWindow window) { return false; }
        protected virtual bool MouseWheel(Event evt, AbilityEditWindow window) { return false; }
        protected virtual bool DoubleClick(Event evt, AbilityEditWindow window) { return false; }
        protected virtual bool KeyDown(Event evt, AbilityEditWindow window) { return false; }
        protected virtual bool KeyUp(Event evt, AbilityEditWindow window) { return false; }
        protected virtual bool ContextClick(Event evt, AbilityEditWindow window) { return false; }
        protected virtual bool ValidateCommand(Event evt, AbilityEditWindow window) { return false; }
        protected virtual bool ExecuteCommand(Event evt, AbilityEditWindow window) { return false; }
        public virtual void Overlay(Event evt, AbilityEditWindow window) { }
        public bool HandleEvent(Event evt, AbilityEditWindow window)
        {
            controlId = EditorGUIUtility.GetControlID(UnityEngine.FocusType.Passive);

            bool isHandled = false;

            switch (evt.type)
            {
                case EventType.ScrollWheel:
                    isHandled = MouseWheel(evt, window);
                    break;

                case EventType.MouseUp:
                    {
                        if (EditorGUIUtility.hotControl == controlId)
                        {
                            isHandled = MouseUp(evt, window);

                            GUIUtility.hotControl = 0;
                            evt.Use();
                        }
                    }
                    break;

                case EventType.MouseDown:
                    {
                        isHandled = evt.clickCount < 2 ? MouseDown(evt, window) : DoubleClick(evt, window);

                        if (isHandled)
                            GUIUtility.hotControl = controlId;
                    }
                    break;

                case EventType.MouseDrag:
                    {
                        if (GUIUtility.hotControl == controlId)
                            isHandled = MouseDrag(evt, window);
                    }
                    break;

                case EventType.KeyDown:
                    isHandled = KeyDown(evt, window);
                    break;

                case EventType.KeyUp:
                    isHandled = KeyUp(evt, window);
                    break;

                case EventType.ContextClick:
                    isHandled = ContextClick(evt, window);
                    break;

                case EventType.ValidateCommand:
                    isHandled = ValidateCommand(evt, window);
                    break;

                case EventType.ExecuteCommand:
                    isHandled = ExecuteCommand(evt, window);
                    break;
            }

            if (isHandled)
            {
                evt.Use();
            }

            return isHandled;
        }
        
    }
}