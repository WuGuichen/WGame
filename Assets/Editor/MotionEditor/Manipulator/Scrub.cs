using System;
using UnityEngine;

namespace Motion
{
    class Scrub : Manipulator
    {
        protected readonly Func<Event, WindowState, bool> _onMouseDown;
        protected readonly Action<double> _onMouseDrag;
        protected readonly Action _onMouseUp;

        protected bool _isCaptured;

        public Scrub(Func<Event, WindowState, bool> onMouseDown, Action<double> onMouseDrag, Action onMouseUp)
        {
            _onMouseDown = onMouseDown;
            _onMouseDrag = onMouseDrag;
            _onMouseUp = onMouseUp;
        }

        protected override bool MouseDown(Event evt, WindowState state)
        {
            if (evt.button != 0)
                return false;
            if (!_onMouseDown(evt, state))
                return false;
            
            state.AddCaptured(this);
            _isCaptured = true;
            return true;
        }
        
        protected override bool MouseUp(Event evt, WindowState state)
        {
            if (!_isCaptured)
                return false;

            _isCaptured = false;
            state.RemoveCaptured(this);

            _onMouseUp();

            return true;
        }

        protected override bool MouseDrag(Event evt, WindowState state)
        {
            if (!_isCaptured)
                return false;

            _onMouseDrag(state.GetSnappedTimeAtMousePosition(evt.mousePosition));

            return true;
        }
    }
}