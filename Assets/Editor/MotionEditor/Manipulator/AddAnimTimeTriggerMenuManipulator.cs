using UnityEditor;
using UnityEngine;

namespace Motion
{
    class AddAnimTimeTriggerMenuManipulator : NewEventMenuManipulator
    {
        private TriggerAnimationControl _control;

        public AddAnimTimeTriggerMenuManipulator(WindowState state, TriggerAnimationControl control) : base(state)
        {
            _control = control;
        }
        
        private void OnClickAddTriggerEvent(object userdata)
        {
            _control.AddTrigger(float.Parse(userdata.ToString()));
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
                menu.AddItem(new GUIContent("Add Trigger"), false, OnClickAddTriggerEvent, state.GetSnappedTimeAtMousePosition(evt.mousePosition));
                menu.AddItem(new GUIContent("复制轨道"), false, OnCopyControlEvent);
                menu.ShowAsContext();
                return true;
            }

            return false;
        }
    }
}