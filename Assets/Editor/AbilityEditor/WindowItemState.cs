using System;
using System.Collections.Generic;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal class WindowItemState : ScriptableObject
    {
        public AbilityEditWindow Window => AbilityEditWindow.Window;
        
        public virtual string GetItemType(){return String.Empty;}

        [SerializeField] private List<IManipulator> _captured = new();
        [SerializeField] protected Rect rect = Rect.zero;
        
        public Rect manipulatorRect
        {
            get { return rect; }
        }

        public void AddManipulator(IManipulator manipulator)
        {
            if (!_captured.Contains(manipulator))
            {
                _captured.Add(manipulator);
            }
        }

        public void RemoveManipulator(IManipulator manipulator)
        {
            _captured.Remove(manipulator);
        }
        
        public virtual void Draw()
        { }
        public virtual void DrawInspector()
        { }
        
        public bool HandleManipulatorsEvents(AbilityEditWindow window, Event evt)
        {
            var isHandled = false;
            using (var itr = _captured.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    isHandled = itr.Current.HandleEvent(evt, window);
                    if (isHandled)
                        break;
                }
            }

            return isHandled;
        }
    }
}