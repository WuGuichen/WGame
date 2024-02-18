using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Classes;
using CrashKonijn.Goap.Enums;
using CrashKonijn.Goap.Interfaces;

namespace WGame.GOAP
{
    public class PatrolAction : ActionBase<PatrolAction.Data>
    {
        public class Data : IActionData
        {
            public ITarget Target { get; set; }
            public MoveAgent Agent { get; set; }
            public float Timer { get; set; }
        }

        public override void Created()
        {
        }

        public override void Start(IMonoAgent agent, Data data)
        {
            // data.Timer = Random.Range(0.3f, 1f);
            if (agent.Entity is GameEntity entity)
            {
                // 设置目标
                data.Agent = entity.aiAgent.service.MoveAgent;
                data.Agent.SetMoveSpeedRate(2f);
            }
            else
            {
                data.Agent = null;
            }
        }

        public override ActionRunState Perform(IMonoAgent agent, Data data, ActionContext context)
        {
            if (data.Agent != null)
            {
                var reached = data.Agent.MoveToPatrolPoint(data.Agent.CurPatrolIndex);
                if (reached)
                {
                    WLogger.Print("Stop");
                    return ActionRunState.Stop;
                }
                else
                {
                    return ActionRunState.Continue;
                }
            }
            
            return ActionRunState.Stop;
        }

        public override void End(IMonoAgent agent, Data data)
        {
            data.Agent.SetNewPatrolPointIndex();
            WLogger.Print("End");
        }

        public override float GetCost(IMonoAgent agent, IComponentReference references)
        {
            return this.Config.BaseCost;
        }

        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return distance <= GetInRange(agent, data);
        }

        public override float GetInRange(IMonoAgent agent, IActionData data)
        {
            return this.Config.InRange;
        }
    }
}