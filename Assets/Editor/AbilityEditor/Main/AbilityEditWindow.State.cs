using System.Collections.Generic;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal sealed partial class AbilityEditWindow
    {
        [SerializeReference] private List<IManipulator> captureList = new();
        
        public void AddCaptured(IManipulator manipulator)
        {
            if (!captureList.Contains(manipulator))
            {
                captureList.Add(manipulator);
            }
        }

        public void RemoveCaptured(IManipulator manipulator)
        {
            captureList.Remove(manipulator);
        }
    }
}