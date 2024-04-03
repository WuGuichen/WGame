using Entitas;

public class DetectCharacterSystem : IExecuteSystem
{
    private readonly IGroup<SensorEntity> charDetectorGroup;
    private readonly ITimeService _time;
    public DetectCharacterSystem(Contexts contexts)
    {
        charDetectorGroup = contexts.sensor.GetGroup(SensorMatcher.AllOf(SensorMatcher.DetectCharOpen));
        _time = contexts.meta.timeService.instance;
    }
    public void Execute()
    {
        UnityEngine.Profiling.Profiler.BeginSample("DetectSensorCharacter");
        foreach (var sensor in charDetectorGroup)
        {
            if(EntityUtils.IsNetCamera(sensor.linkCharacter.Character))
                continue;
            if (sensor.hasDetectorCharacterService)
            {
                sensor.detectorCharacterService.service.UpdateDetect(_time.TimeDeltaTime);
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }
}
