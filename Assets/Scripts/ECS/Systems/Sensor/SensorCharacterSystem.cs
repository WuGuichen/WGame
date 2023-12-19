using System.Collections.Generic;
using Entitas;
using TWY.Physics;

public class SensorCharacterSystem : IExecuteSystem
{
    private readonly IGroup<SensorEntity> charSensorGroup;
    private List<HitInfo> hitInfos = new();
    public SensorCharacterSystem(Contexts contexts)
    {
        charSensorGroup = contexts.sensor.GetGroup(SensorMatcher.AllOf(SensorMatcher.SensorCharOpen));
    }
    public void Execute()
    {
        UnityEngine.Profiling.Profiler.BeginSample("SensorCharacter");
        foreach (var sensor in charSensorGroup)
        {
            if (sensor.hasSensorCharacterService)
            {
                sensor.sensorCharacterService.service.UpdateSensor();
            }

            var entity = sensor.linkCharacter.Character;
            // 允许目标发现自己
            var position = entity.gameViewService.service.Position;
            var sphere = new SphereF(position.ToFloat3(), 20f);
            hitInfos.Clear();
            if (entity.isCampWhite)
            {
                EntityUtils.BvhEnemy.TestHitSphereNonAlloc(sphere, ref hitInfos);
                for (int i = 0; i < hitInfos.Count; i++)
                {
                    var tar = EntityUtils.GetGameEntity(hitInfos[i].EntityId);
                    tar.linkSensor.Sensor.sensorCharacterService.service.AddDetectTarget(new HitInfo(entity.instanceID.ID, hitInfos[i].SqrDist, position));
                }
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }
}
