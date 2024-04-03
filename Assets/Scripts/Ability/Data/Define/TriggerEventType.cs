namespace WGame.Ability
{
    public class TriggerEventType
    {
        public const int HitTarget = 1;
        public const int BeHit = 1 << 1;
        public const int JumpLand = 1 << 2;
        public const int JumpLand_Heavy = 1 <<3;
        public const int ChangeAttr = 1 << 4;

        public const int Count = 5;

        public static readonly string[] Names = new string[Count]
        {
            "命中目标", "被命中", "落地", "重重落地", "属性改变"
        };
    }
}