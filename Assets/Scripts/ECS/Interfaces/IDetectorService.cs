public interface IDetectorService : IBaseService
{
    void UpdateSensorDrawer();
    void UpdateDetect(float deltaTime);
    void AddDetectTarget(HitInfo hitInfo);
    GameEntity Entity { get; }
    SensorEntity Sensor { get; }
    HatePointInfo HatePointInfo { get; }
    void ChangeHateInfoBuffer(int entityId, float value, int type);
    void SetHateInfoBuffer(int entityId, float value, int type);
}
