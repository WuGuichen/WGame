using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motion
{
    abstract class Manipulator
    {
        int m_Id;

        protected virtual bool MouseDown(Event evt, WindowState state) { return false; }
        protected virtual bool MouseDrag(Event evt, WindowState state) { return false; }
        protected virtual bool MouseWheel(Event evt, WindowState state) { return false; }
        protected virtual bool MouseUp(Event evt, WindowState state) { return false; }
        protected virtual bool DoubleClick(Event evt, WindowState state) { return false; }
        protected virtual bool KeyDown(Event evt, WindowState state) { return false; }
        protected virtual bool KeyUp(Event evt, WindowState state) { return false; }
        protected virtual bool ContextClick(Event evt, WindowState state) { return false; }
        protected virtual bool ValidateCommand(Event evt, WindowState state) { return false; }
        protected virtual bool ExecuteCommand(Event evt, WindowState state) { return false; }

        public virtual void Overlay(Event evt, WindowState state) { }



        public bool HandleEvent(WindowState state)
        {
            if (m_Id == 0)
            {    
                m_Id = TimelineFuncHelper.GetPermanentControlID();
            }
            bool isHandled = false;
            var evt = Event.current;

            switch (evt.GetTypeForControl(m_Id))
            {
                case UnityEngine.EventType.ScrollWheel:
                    isHandled = MouseWheel(evt, state);
                    break;

                case UnityEngine.EventType.MouseUp:
                    {
                        if (GUIUtility.hotControl == m_Id)
                        {
                            isHandled = MouseUp(evt, state);

                            GUIUtility.hotControl = 0;
                            evt.Use();
                        }
                    }
                    break;

                case UnityEngine.EventType.MouseDown:
                    {
                        isHandled = evt.clickCount < 2 ? MouseDown(evt, state) : DoubleClick(evt, state);

                        if (isHandled)
                            GUIUtility.hotControl = m_Id;
                    }
                    break;

                case UnityEngine.EventType.MouseDrag:
                    {
                        if (GUIUtility.hotControl == m_Id)
                            isHandled = MouseDrag(evt, state);
                    }
                    break;

                case UnityEngine.EventType.KeyDown:
                    isHandled = KeyDown(evt, state);
                    break;

                case UnityEngine.EventType.KeyUp:
                    isHandled = KeyUp(evt, state);
                    break;

                case UnityEngine.EventType.ContextClick:
                    isHandled = ContextClick(evt, state);
                    break;

                case UnityEngine.EventType.ValidateCommand:
                    isHandled = ValidateCommand(evt, state);
                    break;

                case UnityEngine.EventType.ExecuteCommand:
                    isHandled = ExecuteCommand(evt, state);
                    break;
            }

            if (isHandled)
                evt.Use();

            return isHandled;
        }
    }


}
