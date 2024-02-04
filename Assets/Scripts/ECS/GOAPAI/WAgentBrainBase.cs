using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Interfaces;

namespace WGame.GOAP
{
    public abstract class WAgentBrainBase
    {
        protected AgentBehaviour agent;
        public abstract AgentBehaviour Agent { get; }

        protected bool isEnable;

        protected WAgentBrainBase(AgentBehaviour agent)
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
                agent.Events.OnActionStop += OnActionStop;
                agent.Events.OnNoActionFound += OnNoActionFound;
                agent.Events.OnGoalCompleted += OnGoalCompleted;
            }
            else
            {
                agent.Events.OnActionStop -= OnActionStop;
                agent.Events.OnNoActionFound -= OnNoActionFound;
                agent.Events.OnGoalCompleted -= OnGoalCompleted;
                OnDisable();
            }
        }

        protected abstract void OnEnable();
        protected abstract void OnDisable();

        protected abstract void OnActionStop(IActionBase action);
        protected abstract void OnNoActionFound(IGoalBase goal);
        protected abstract void OnGoalCompleted(IGoalBase goal);
        
        public virtual void Disable()
        {
            SetEnable(false);
        }
    }
}