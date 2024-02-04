using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Interfaces;

namespace WGame.GOAP
{
    public class BaseAgentBrain : WAgentBrainBase
    {
        public BaseAgentBrain(AgentBehaviour agent) : base(agent)
        {
        }

        public override AgentBehaviour Agent => agent;

        protected override void OnEnable()
        {
            agent.SetGoal<PatrolGoal>(false);
        }

        protected override void OnDisable()
        {
        }

        protected override void OnActionStop(IActionBase action)
        {
            WLogger.Print("Stop");
        }

        protected override void OnNoActionFound(IGoalBase goal)
        {
            agent.SetGoal<PatrolGoal>(false);
            WLogger.Print("NotFound");
        }

        protected override void OnGoalCompleted(IGoalBase goal)
        {
            agent.SetGoal<PatrolGoal>(false);
            WLogger.Print("Compeleted");
        }
    }
}