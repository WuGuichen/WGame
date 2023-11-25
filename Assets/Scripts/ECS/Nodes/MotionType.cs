
public class MotionType
{
    public const int LocalMotion = 1;
    public const int Attack1 = 1 << 1;
    public const int Attack2 = 1 << 2;
    public const int Attack3 = 1 << 3;
    public const int Jump = 1 << 4;
    public const int Step = 1 << 5;
    public const int Defense = 1 << 6;
    public const int JumpAttack1 = 1 << 7;
    
    // 特殊动作
    public const int HitFwd = 10001;
    public const int HitBwd = 10002;

    public const int Count = 8;

    // 可以自然转换的部分
    public static readonly int[] EnumList = new int[Count]
    {
        LocalMotion, Attack1, Attack2, Attack3, Jump, Step, Defense, JumpAttack1
    };
    
    public static readonly string[] EnumName = new string[Count]
    {
        "LocalMotion", "Attack1", "Attack2", "Attack3", "Jump", "Step", "Defense", "JumpAttack1"
    };
}

public class OtherMotionType
{
    public const int HitFwd = 1;
}