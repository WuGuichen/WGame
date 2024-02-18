using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Interfaces;

namespace WGame.GOAP
{
    public class BaseAgentBrain : WAgentBrainBase
    {
        private GameEntity _entity;
        private HatePointInfo _info;
        public BaseAgentBrain(AgentBehaviour agent, GameEntity entity, IGoapSet goapSet, IAgentDistanceObserver distanceObserver) : base(agent)
        {
            _entity = entity;
            _info = entity.linkSensor.Sensor.detectorCharacterService.service.HatePointInfo;
            base.agent.Entity = entity;
            base.agent.GoapSet = goapSet;
            base.agent.DistanceObserver = distanceObserver;
        }

        public override AgentBehaviour Agent => agent;

        protected override void OnEnable()
        {
            agent.SetGoal<PatrolGoal>(false);
        }

        private bool IsHating => _info.MaxHateEntityRank >= HateRankType.Alert;

        public override void OnUpdate()
        {
            UpdateHateRank();
        }

        protected override void OnDisable()
        {
        }

        protected override void OnActionStop(IActionBase action)
        {
            // WLogger.Print("Stop");
            UpdateHateRank();
        }

        private void UpdateHateRank()
        {
            if (IsHating)
            {
                agent.SetGoal<HateGoal>(false);
            }
            else
            {
                agent.SetGoal<PatrolGoal>(false);
            }
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