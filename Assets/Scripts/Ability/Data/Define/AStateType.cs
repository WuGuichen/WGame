namespace WGame.Ability
{
    public class AStateType
    {
        public const int EnableWeapon = 1;
        public const int Unbalance = 1 << 1;
        public const int LocalMotion = 1 << 2;
        public const int IsOnGround = 1 << 3;
        public const int UnHittable = 1 << 4;

        public const int Count = 5;

        public static readonly string[] Names = new string[Count]
        {
            "开启武器", "失去平衡", "待机", "在地面", "不可命中"
        };
    }
}