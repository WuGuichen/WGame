public class SensorType
{
    public const int RightWeapon = 0;
    public const int LeftWeapon = 1;
    public const int RightHand = 2;
    public const int LeftHand = 3;
    public const int RightFoot = 4;
    public const int LeftFoot = 5;

    public const int Count = 6;

    public static readonly string[] list = new string[Count]
    {
        "右手武器",
        "左手武器",
        "右拳",
        "左拳",
        "右腿",
        "左腿",
    };

    public static readonly int[] IDs = new int[Count]
    {
        RightWeapon,
        LeftWeapon,
        RightHand,
        LeftHand,
        RightFoot,
        LeftFoot,
    };
}
