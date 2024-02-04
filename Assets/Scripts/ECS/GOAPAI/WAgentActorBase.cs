using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Interfaces;

namespace WGame.GOAP
{
    public abstract class WAgentActorBase
    {
        protected AgentBehaviour agent;

        private bool isEnable;

        public WAgentActorBase(AgentBehaviour agent)
        {
            this.agent = agent;
            isEnable = false;
        }
        
        public void SetEnable(bool value)
        {
            if (value == isEnable)
                return;
            isEnable = value;
            if (value)
            {
                OnEnable();
                agent.Events.OnTargetInRange += OnTargetInRange;
                agent.Events.OnTargetChanged += OnTargetChanged;
                agent.Events.OnTargetOutOfRange += OnTargetOutOfRange;
            }
            else
            {
                agent.Events.OnTargetInRange -= OnTargetInRange;
                agent.Events.OnTargetChanged -= OnTargetChanged;
                agent.Events.OnTargetOutOfRange -= OnTargetOutOfRange;
                OnDisable();
            }
        }

        protected abstract void OnEnable();
        protected abstract void OnDisable();

        public abstract void Update();
        
        protected abstract void OnTargetInRange(ITarget target);
        protected abstract void OnTargetChanged(ITarget target, bool inRange);
        protected abstract void OnTargetOutOfRange(ITarget target);

        public virtual void Disable()
        {
            SetEnable(false);
        }
    }
}