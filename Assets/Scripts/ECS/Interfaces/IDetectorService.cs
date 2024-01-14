public interface IDetectorService
{
    void UpdateSensorDrawer();
    void UpdateDetect(float deltaTime);
    void AddDetectTarget(HitInfo hitInfo);
    void Dispose();
    GameEntity Entity { get; }
    SensorEntity Sensor { get; }
    HatePointInfo HatePointInfo { get; }
}
