using CrashKonijn.Goap.Classes.Builders;
using CrashKonijn.Goap.Configs;
using CrashKonijn.Goap.Enums;
using CrashKonijn.Goap.Resolver;
using WGame.GOAP;
using WGame.Runtime;

public class WGOAPMgr : Singleton<WGOAPMgr>
{
    public GoapSetConfig CreateBaseGoapConfig()
    {
        var builder = new GoapSetBuilder("Base");

        // Debugger
        builder.SetAgentDebugger<WGoapDebugger>();

        // Goals
        // 进行巡逻
        builder.AddGoal<PatrolGoal>()
            .AddCondition<IsPatroling>(Comparison.GreaterThanOrEqual, 1);

        // 使仇恨等级小于警戒
        builder.AddGoal<HateGoal>()
            .AddCondition<IsHateRank>(Comparison.SmallerThan, HateRankType.Alert);

        // Actions
        // 增加巡逻
        builder.AddAction<PatrolAction>()
            .SetTarget<PatrolTarget>()
            .AddEffect<IsPatroling>(EffectType.Increase);

        // 减少仇恨值
        builder.AddAction<HateAction>()
            .SetTarget<HateTarget>()
            .AddEffect<IsHateRank>(EffectType.Decrease);
        
        // TargetSensors
        builder.AddTargetSensor<PatrolTargetSensor>()
            .SetTarget<PatrolTarget>();

        builder.AddTargetSensor<HateTargetSensor>()
            .SetTarget<HateTarget>();

        return builder.Build();
    }
    
}
