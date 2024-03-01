using System.Collections.Generic;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal class WindowItemState
    {
        public AbilityEditWindow Window { get; }


        public WindowItemState(AbilityEditWindow window)
        {
            Window = window;
        }

        [SerializeField] private List<IManipulator> _captured = new();
        [SerializeField] protected Rect rect = Rect.zero;

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