using CrashKonijn.Goap.Interfaces;

namespace WGame.GOAP
{
    public class WDistanceObserver
    {
        public static EntityDistanceObserver entity = new EntityDistanceObserver();
    }
    public class EntityDistanceObserver : IAgentDistanceObserver
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
            var motionService = reference.GetCachedComponent<MotionServiceImplementation>();
            return DetectMgr.Inst.GetDistance(motionService.LinkEntity, entity.Entity);
        }
    }
}