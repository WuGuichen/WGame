
public interface ITimeService
{
    float DeltaTime { get; }
    float FixedDeltaTime { get; }
    float RealTimeSinceStart { get; }
    void UpdateDeltaTime(float value);
    void UpdateFixedDeltaTime(float value);
    void UpdateRealTimeSinceStart(float value);
}
