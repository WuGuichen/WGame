using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Classes.Builders;
using CrashKonijn.Goap.Configs.Interfaces;
using CrashKonijn.Goap.Enums;
using CrashKonijn.Goap.Resolver;

namespace WGame.GOAP
{
    public class GoapSetConfigFactory : GoapSetFactoryBase
    {
        public override IGoapSetConfig Create()
        {
            var builder = new GoapSetBuilder("GettingStartSet");

            builder.AddGoal<PatrolGoal>()
                .AddCondition<IsPatroling>(Comparison.GreaterThanOrEqual, 1);

            builder.AddAction<PatrolAction>()
                .SetTarget<PatrolTarget>()
                .AddEffect<IsPatroling>(EffectType.Increase)
                .SetBaseCost(1)
                .SetInRange(0.3f);
            return builder.Build();
        }
    }
}