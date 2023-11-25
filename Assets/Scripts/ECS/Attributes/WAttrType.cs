namespace WGame.Attribute
{
    public class WAttrType
    {
        public const int MaxHP = 0;
        public const int CurHP = 1;
        public const int MaxMP = 2;
        public const int CurMP = 3;
        public const int ATK = 4;
        public const int DEF = 5;
        public const int AtkForceFwd = 6;
        public const int AtkForceRight = 7;
        public const int AtkForceUp = 8;

        public const int Count = 9;

        public static readonly string[] Names = new string[Count]
        {
            "血量", 
            "当前血量",
            "魔力",
            "当前魔力",
            "攻击",
            "防御",
            "冲击力_前",
            "冲击力_上",
            "冲击力_右",
        };
    }
}  