using UnityEngine;

public interface ICameraService
{
    Transform Root { get; }
    Transform Pivot { get; }
    Transform Camera { get; }
    bool IsAutoControl { get; }
    Vector3 CachedFwd { get; }
    void Shake(float time, float intensity = 1f, WEaseType easeType = WEaseType.Linear);
    void Move(Vector3 deltaPos, WEaseType easeType = WEaseType.Linear, float duration = 0.0f, float holdTime = 0.2f);
    void StopMove();
    void StopRotate();
    void Rotate(Vector3 deltaRot, WEaseType easeType = WEaseType.Linear, float speed = 1f, float holdTime = 2f);
    void Process(float deltaTime);
}
