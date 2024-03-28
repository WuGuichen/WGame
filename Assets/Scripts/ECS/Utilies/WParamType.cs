public class WParamType
{
    public const int Int = 0;
    public const int Float = 1;
    public const int Vector2 = 1 << 1;
}

public class WParam
{
    public const ulong MASK_TYPE = 0xf000000000000000;
    public const ulong MASK_INT = 0x00000000ffffffff;

    public static int Get(ulong value)
    {
        var type = (int)(value & MASK_TYPE);
        if (type != WParamType.Int)
        {
            WLogger.Error("类型错误");
            return 0;
        }

        return (int)(value & MASK_INT);
    }

    public static void Set(ref ulong param, int value)
    {
        param = (MASK_INT & (ulong)value);
    }
}
