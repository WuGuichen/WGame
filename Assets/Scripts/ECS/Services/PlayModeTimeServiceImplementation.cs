using UnityEngine;

public class PlayModeTimeServiceImplementation : ITimeService
{
    public PlayModeTimeServiceImplementation()
    {
        
    }

    public float deltaTime => Time.deltaTime;
    public float fixedDeltaTime => Time.fixedDeltaTime;
}
