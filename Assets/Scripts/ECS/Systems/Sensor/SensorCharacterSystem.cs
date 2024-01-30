using System.Collections.Generic;
using Entitas;
using TWY.Physics;
using Unity.Mathematics;

public class SensorCharacterSystem : IExecuteSystem
{
    private readonly IGroup<SensorEntity> charSensorGroup;
    private List<HitInfo> hitInfos = new();
    private readonly IGroup<GameEntity> _whiteGroup;
    private readonly IGroup<GameEntity> _redGroup;
    public SensorCharacterSystem(Contexts contexts)
    {
        charSensorGroup = contexts.sensor.GetGroup(SensorMatcher.AllOf(SensorMatcher.SensorCharOpen));
        _whiteGroup = contexts.game.GetGroup(GameMatcher.CampWhite);
        _redGroup = contexts.game.GetGroup(GameMatcher.CampRed);
    }
    public void Execute()
    {
        UnityEngine.Profiling.Profiler.BeginSample("SensorCharacter");
        
        foreach (var entity in _whiteGroup)
        {
            var position = entity.position.value;
            var sphere = new SphereF(position.ToFloat3(), 50f);
            hitInfos.Clear();
            EntityUtils.BvhRed.TestHitSphereNonAlloc(sphere, ref hitInfos);
            for (int i = 0; i < hitInfos.Count; i++)
            {
                var tar = EntityUtils.GetGameEntity(hitInfos[i].EntityId);
                var dist = math.sqrt(hitInfos[i].SqrDist);
                DetectMgr.Inst.UpdateDistance(tar.instanceID.ID, entity.instanceID.ID, dist);
                tar.linkSensor.Sensor.detectorCharacterService.service.AddDetectTarget(new HitInfo(entity.instanceID.ID,
                    position, dist, hitInfos[i].SqrDist));
                entity.linkSensor.Sensor.detectorCharacterService.service.AddDetectTarget(new HitInfo(tar.instanceID.ID,
                    tar.position.value, dist, hitInfos[i].SqrDist));
            }
        }
        
        UnityEngine.Profiling.Profiler.EndSample();
    }
}
