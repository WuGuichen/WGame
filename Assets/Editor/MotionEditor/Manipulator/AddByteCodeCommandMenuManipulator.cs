using UnityEditor;
using UnityEngine;

namespace Motion
{
    class AddByteCodeCommandMenuManipulator : NewEventMenuManipulator
    {
        private ByteCodeCommandControl _control;

        public AddByteCodeCommandMenuManipulator(WindowState state, ByteCodeCommandControl control) : base(state)
        {
            _control = control;
        }
        
        private void OnClickAddCommandEvent(object userdata)
        {
            _control.AddCommand(float.Parse(userdata.ToString()));
        }

        void OnClickDeleteEvent()
        {
            state.RemoveNodeControl(_control);
        }

        void OnCopyControlEvent()
        {
            PrefabModel.CopyControl(_control.Node);
            Debug.Log("已复制");
        }

        protected override bool ContextClick(Event evt, WindowState state)
        {
            if (_control.rightSize.Contains(evt.mousePosition))
            {
                menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete"), false, OnClickDeleteEvent);
                menu.AddItem(new GUIContent("Add Command"), false, OnClickAddCommandEvent, state.GetSnappedTimeAtMousePosition(evt.mousePosition));
                menu.AddItem(new GUIContent("复制轨道"), false, OnCopyControlEvent);
                menu.ShowAsContext();
                return true;
            }

            return false;
        }
    }
}