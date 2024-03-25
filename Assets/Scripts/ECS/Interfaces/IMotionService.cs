public interface IMotionService
{
    void StartMotion(int motionID);
    void UpdateMotion();
    void SwitchMotion(int UID, bool isNet=true);
    void SetLocalMotion(int animGroup);
    void ResetMotion();
    GameEntity LinkEntity { get; }
    MotionAnimationProcessor AnimProcessor { get; }
    MotionAnimTriggerProcessor AnimTriggerProcessor { get; }

    void TransMotionByMotionType(int type);

    IMotionService OnInit(MotionEntity entity);
    void OnMotionExit();

    void Destroy();
}
