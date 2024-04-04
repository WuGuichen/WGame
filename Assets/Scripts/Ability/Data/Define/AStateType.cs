namespace WGame.Ability
{
    public class AStateType
    {
        public const int EnableWeapon = 1;
        public const int Unbalance = 1 << 1;
        public const int LocalMotion = 1 << 2;
        public const int IsOnGround = 1 << 3;
        public const int UnHittable = 1 << 4;
        public const int Invincible = 1 << 5;
        public const int RotateToFocus = 1 << 6;
        public const int KeepDistance = 1 << 7;
        public const int IsRunning = 1 << 8;
        public const int MoveToFocus = 1 << 9;

        public const int Count = 10;

        public static readonly string[] Names = new string[Count]
        {
            "开启武器", "失去平衡", "待机", "在地面", "不可命中"
            , "无敌", "转向到目标", "保持距离", "跑步中", "移动向目标",
        };
    }
}