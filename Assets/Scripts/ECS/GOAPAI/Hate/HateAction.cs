using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Classes;
using CrashKonijn.Goap.Enums;
using CrashKonijn.Goap.Interfaces;

namespace WGame.GOAP
{
    public class HateAction : ActionBase<HateAction.Data>
    {
        public class Data : IActionData
        {
            public HatePointInfo Info { get; set; }
            public ITarget Target { get; set; }
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
                data.Info = entity.linkSensor.Sensor.detectorCharacterService.service.HatePointInfo;
            }
            else
            {
                data.Info = null;
            }
        }

        public override ActionRunState Perform(IMonoAgent agent, Data data, ActionContext context)
        {
            if (data.Info != null)
            {
                if (data.Info.MaxHateEntityRank < HateRankType.Alert)
                {
                    // 仇恨等级过低
                    return ActionRunState.Stop;
                }
                else
                {
                    // 依然仇恨
                    return ActionRunState.Continue;
                }
            }
            
            return ActionRunState.Stop;
        }

        public override void End(IMonoAgent agent, Data data)
        {
            WLogger.Print("End");
        }

        public override float GetCost(IMonoAgent agent, IComponentReference references)
        {
            return this.Config.BaseCost;
        }

        public override bool IsInRange(IMonoAgent agent, float distance, IActionData data, IComponentReference references)
        {
            return true;
        }

        public override float GetInRange(IMonoAgent agent, IActionData data)
        {
            return this.Config.InRange;
        }
    }
}