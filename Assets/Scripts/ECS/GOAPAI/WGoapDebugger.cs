using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Interfaces;
using UnityEngine;

namespace WGame.GOAP
{
    public class WGoapDebugger : IAgentDebugger
    {
        public string GetInfo(IMonoAgent agent, IComponentReference references)
        {
            return "Test";
        }
    }
}