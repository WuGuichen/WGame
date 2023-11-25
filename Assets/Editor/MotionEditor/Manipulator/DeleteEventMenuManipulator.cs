using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motion
{
    class DeleteEventMenuManipulator : NewEventMenuManipulator
    {
        private NodeControl _control;
        public DeleteEventMenuManipulator(WindowState state, NodeControl control) : base(state)
        {
            _control = control;
        }

        void OnClickDeleteEvent()
        {
            state.RemoveNodeControl(_control);
        }

        protected override bool ContextClick(Event evt, WindowState state)
        {
            if (_control.rightSize.Contains(evt.mousePosition))
            {
                menu = state.GetMenu();
                menu.AddItem(new GUIContent("Delete"), false, OnClickDeleteEvent);
                menu.ShowAsContext();
                return true;
            }

            return false;
        }
    }
}