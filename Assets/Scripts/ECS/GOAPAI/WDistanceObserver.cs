using CrashKonijn.Goap.Interfaces;

namespace WGame.GOAP
{
    public class WDistanceObserver : IAgentDistanceObserver
    {
        public float GetDistance(IMonoAgent agent, ITarget target, IComponentReference reference)
        {
            if (target == null)
            {
                return 0f;
            }

            var entity = target as EntityTarget;
            if (entity == null)
            {
                return 0f;
            }

            var targetEntity = EntityUtils.GetGameEntity(agent.EntityID);
            return DetectMgr.Inst.GetDistance(targetEntity, entity.Entity);
        }
    }
}