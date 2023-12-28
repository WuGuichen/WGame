public class PlayModeTimeServiceImplementation : ITimeService
{
    private float deltaTime;
    private float fixedDeltaTime;
    private float realTimeSinceStart;
    public float DeltaTime => deltaTime;
    public float FixedDeltaTime => fixedDeltaTime;
    public float RealTimeSinceStart => realTimeSinceStart;
    
    public void UpdateDeltaTime(float value) => deltaTime = value;

    public void UpdateFixedDeltaTime(float value) => fixedDeltaTime = value;
    public void UpdateRealTimeSinceStart(float value) => realTimeSinceStart = value;
}
