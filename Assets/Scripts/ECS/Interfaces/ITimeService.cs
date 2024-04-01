
public interface ITimeService
{
    float DeltaTime(float scale);
    float DivFixedDeltaTime { get; }
    float FixedDeltaTime { get; }
    float RealTimeSinceStart { get; }
    void UpdateDeltaTime(float value);
    void UpdateFixedDeltaTime(float value);
    void UpdateRealTimeSinceStart(float value);
}
