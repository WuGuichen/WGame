using System.Collections;
using System.Collections.Generic;
using Motion;
using UnityEngine;

namespace Motion
{
    class RootMotionControl : NodeControl<RootMotionNode>
    {
        protected TimeAreaItem endTime;

        public RootMotionControl(WindowState state) : base(state)
        {
        }
    }
}
