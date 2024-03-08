using System.Collections.Generic;
using Entitas;
using TWY.Physics;
using Unity.Mathematics;
using UnityEngine;

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
                var hitInfo = hitInfos[i];
                var tar = EntityUtils.GetGameEntity(hitInfo.EntityId);
                var model = entity.gameViewService.service.Model;
                var modelTarget = tar.gameViewService.service.Model;
                var dir = tar.position.value - entity.position.value;
                var sqrDist = dir.sqrMagnitude;
                var dist = math.sqrt(sqrDist);
                var normalDir = dir / dist;
                var angleToTarget = model.forward.GetAngle360(normalDir, model.up);
                var angleToEntity = modelTarget.forward.GetAngle360(-normalDir, modelTarget.up);
                DetectMgr.Inst.UpdateDistance(tar.instanceID.ID, entity.instanceID.ID, dist);
                DetectMgr.Inst.UpdateAngle(entity.instanceID.ID, hitInfo.EntityId, angleToTarget*Mathf.Rad2Deg);
                DetectMgr.Inst.UpdateAngle(tar.instanceID.ID, entity.instanceID.ID, angleToEntity*Mathf.Rad2Deg);
                tar.linkSensor.Sensor.detectorCharacterService.service.AddDetectTarget(new HitInfo(entity.instanceID.ID,
                    position, dist, sqrDist));
                entity.linkSensor.Sensor.detectorCharacterService.service.AddDetectTarget(new HitInfo(tar.instanceID.ID,
                    tar.position.value, dist, sqrDist));
            }
        }
        
        UnityEngine.Profiling.Profiler.EndSample();
    }
}
