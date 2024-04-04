public class MotionType
{
    public const int LocalMotion = 1;
    public const int Attack1 = 1 << 1;
    public const int Attack2 = 1 << 2;
    public const int Attack3 = 1 << 3;
    public const int Jump = 1 << 4;
    public const int Step = 1 << 5;
    public const int Defense = 1 << 6;
    
    [WLable("跳跃攻击")]
    public const int JumpAttack = 1 << 7;
    [WLable("受击")]
    public const int Hit = 1 << 8;
    [WLable("落地")]
    public const int JumpLand = 1 << 9;
    [WLable("终结技")]
    public const int FinishAttack = 1 << 10;
    [WLable("被终结")]
    public const int VictimFinishAttack = 1 << 11;
    [WLable("下落")]
    public const int Falling = 1 << 12;
    [WLable("特殊技")]
    public const int SpecialSkill = 1 << 13;
    [WLable("空闲动作")]
    public const int Spare = 1 << 14;
    [WLable("死亡")]
    public const int Death = 1 << 15;
    [WLable("起身")]
    public const int GetUp = 1 << 16;
    [WLable("紧急闪避")]
    public const int StepEmergency = 1 << 17;

    // 特殊动作不用专门写id

    public const int Count = 7;

    // 可以自然转换的部分
    public static readonly int[] EnumList = new int[Count]
    {
        LocalMotion, Attack1, Attack2, Attack3, Jump, Step, Defense
    };
    
    public static readonly string[] Names = new string[Count]
    {
        "待机", "一段普攻", "二段普攻", "三段普攻", "跳跃", "闪避", "防御"
    };
}