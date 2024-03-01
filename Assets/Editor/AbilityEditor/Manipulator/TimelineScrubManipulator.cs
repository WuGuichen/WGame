using UnityEngine;

namespace WGame.Ability.Editor
{
    internal class TimelineScrubManipulator : IManipulator
    {
        private bool _captured;
        private readonly System.Func<Event, bool> onMouseDown;
        private readonly System.Action<Event> onMouseDrag;
        private readonly System.Action<Event> onMouseUp;

        public TimelineScrubManipulator(System.Func<Event, bool> onMouseDown
            , System.Action<Event> onMouseDrag, System.Action<Event> onMouseUp)
        {
            this.onMouseDown = onMouseDown;
            this.onMouseDrag = onMouseDrag;
            this.onMouseUp = onMouseUp;
        }
        
        protected override bool MouseDown(Event evt, AbilityEditWindow window)
        {
            if (evt.button != 0)
                return false;

            if (!onMouseDown(evt))
                return false;

            window.AddCaptured(this);
            _captured = true;

            return true;
        }

        protected override bool MouseUp(Event evt, AbilityEditWindow window)
        {
            if (!_captured)
                return false;

            _captured = false;
            window.RemoveCaptured(this);

            onMouseUp(evt);

            return true;
        }

        protected override bool MouseDrag(Event evt, AbilityEditWindow window)
        {
            if (!_captured)
                return false;

            onMouseDrag(evt);

            return true;
        }
    }
}