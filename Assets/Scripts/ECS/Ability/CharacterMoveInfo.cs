using UnityEngine;

public class CharacterMoveInfo
{
    public Vector3 DeltaPos { get; set; }
    public float Duration { get; set; }
    public WEaseType EaseType { get; set; }
    public bool IgnoreCharTimeScale { get; set; } = false;

    private Vector3 _lastDeltaPos = Vector3.zero;

    private float _curTime =0f;

    public bool OnUpdate(MotionAnimationProcessor owner, float deltaTime, float unscaledDeltaTime)
    {
        if (_curTime > Duration)
        {
            return true;
        }
        _curTime += IgnoreCharTimeScale ? unscaledDeltaTime : deltaTime;
        var rate = WEaseManager.Evaluate(EaseType, _curTime, Duration);
        var curPos = DeltaPos * rate;
        owner.DeltaMovePos += (curPos - _lastDeltaPos);
        _lastDeltaPos = curPos;
        return false;
    }

    public void RequestEnd()
    {
        _curTime = Duration + 1f;
    }
}
