using System;
using System.Collections.Generic;

namespace Motion
{
    class Control : IDisposable
    {
        private readonly List<Manipulator> _manipulators = new();

        public virtual bool HandleManipulatorsEvents(WindowState state)
        {
            var isHandled = false;

            foreach (var manipulator in _manipulators)
            {
                isHandled = manipulator.HandleEvent(state);
                if (isHandled)
                    break;
            }

            return isHandled;
        }
        
        public void AddManipulator(Manipulator m)
        {
            _manipulators.Add(m);
        }

        public virtual void Dispose()
        {
        }
    }
}