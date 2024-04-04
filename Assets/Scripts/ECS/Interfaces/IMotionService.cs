public interface IMotionService
{
    void Initialize();
    void StartMotion(int motionID);
    void UpdateMotion();
    int CurrentMotionType { get; }
    bool TryGetCurAbilityProperty(string name, out TAny value);
    void SetLocalMotion(int animGroup);
    void SetMotionID(int motionType ,string name);
    GameEntity LinkEntity { get; }
    MotionAnimationProcessor AnimProcessor { get; }
    // MotionAnimTriggerProcessor AnimTriggerProcessor { get; }

    void TransMotionByMotionType(int type, int id = -1, bool isOverride = false);

    IMotionService OnInit(MotionEntity entity);
    void OnMotionExit();
    bool CheckMotionType(int motionType);

    void Destroy();
}
