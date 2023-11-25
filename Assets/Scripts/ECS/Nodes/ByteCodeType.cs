namespace Motion
{
    public class ByteCodeType
    {
        public const int Literal_INT = 1 << 0;
        public const int Add = 1 << 1;
        public const int Subtract = 1 << 2;
        public const int Multiply = 1 << 3;
        public const int Divide = 1 << 4;
        public const int GetAttribute = 1 << 5;
        public const int GetMax = 1 << 6;
        public const int GetMin = 1 << 7;
        public const int SetAttribute = 1 << 8;
        public const int End = 1 << 9;
        public const int Literal_FLOAT2 = 1 << 10;
        public const int GetTarget = 1 << 11;
        public const int GetMySelf = 1 << 12;

        public const int Count = 13;

        public static readonly int[] Triggers = new int[Count]
        {
            Literal_INT,
            Add,
            Subtract,
            Multiply,
            Divide,
            GetAttribute,
            GetMax,
            GetMin,
            SetAttribute,
            End,
            Literal_FLOAT2,
            GetTarget,
            GetMySelf,
        };

        public static readonly string[] TriggerNames = new string[Count]
        {
            "读取整数",
            "相加",
            "相减",
            "相乘",
            "相除",
            "读取属性",
            "求最大值",
            "求最小值",
            "设置属性",
            "提前结束",
            "读取小数",
            "读取锁定目标",
            "读取自身",
        };
    }
}
