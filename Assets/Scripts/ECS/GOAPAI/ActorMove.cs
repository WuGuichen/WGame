using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Interfaces;
using UnityEngine;

namespace WGame.GOAP
{
    public class ActorMove : WAgentActorBase
    {
        private EntityTarget curTarget;
        private bool shouldMove = false;
        public ActorMove(AgentBehaviour agent) : base(agent)
        {
        }

        protected override void OnEnable()
        {
            
        }

        protected override void OnDisable()
        {
            
        }

        public override void Update()
        {
            if (!this.shouldMove)
                return;
            
            if (this.curTarget == null)
                return;
            
            curTarget.Move(Vector3.down);
        }

        protected override void OnTargetInRange(ITarget target)
        {
            shouldMove = false;
        }

        protected override void OnTargetChanged(ITarget target, bool inRange)
        {
            curTarget = target as EntityTarget;
            shouldMove = !inRange;
        }

        protected override void OnTargetOutOfRange(ITarget target)
        {
            shouldMove = true;
        }
    }
}