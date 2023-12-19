public interface ISensorService
{
    void UpdateDetectorDrawer();
    void UpdateSensorDrawer();
    void UpdateDetect(float deltaTime);
    void AddDetectTarget(HitInfo hitInfo);
    void UpdateSensor();
    void AddHatePoint(int id, float value);
    void SetHatePoint(int id, float value);
    void Dispose();
    GameEntity Entity { get; }
    SensorEntity Sensor { get; }
}
