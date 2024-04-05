public interface IMotionService
{
    void Initialize();
    void StartMotion(int motionID);
    void UpdateMotion();
    bool TryGetCurAbilityProperty(string name, out TAny value);
    void SetLocalMotion(int animGroup);
    void SetMotionID(int motionType ,string name);
    MotionAnimationProcessor AnimProcessor { get; }
    void TransMotionByMotionType(int type, int id = -1, bool isOverride = false);
    IMotionService OnInit(MotionEntity entity);
    void OnMotionExit();
    void SetAnimSpeed(float value);
    bool CheckMotionType(int motionType);
    void Destroy();
}
