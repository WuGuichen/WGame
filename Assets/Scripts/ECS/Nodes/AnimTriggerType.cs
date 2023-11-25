namespace Motion
{
    public class AnimTriggerType
    {
        public const int RootMotion = 1;
        public const int Move = 1 << 1;
        public const int Rotate = 1 << 2;
        public const int OpenSensor = 1 << 3;
        public const int AnimSpeed = 1 << 4;
        public const int ThrustUp = 1 << 5;
        public const int Loop = 1 << 6;
        public const int TransMotionType = 1 << 7;
        public const int SwitchMotion = 1 << 8;
        public const int KeepDistFromTarget = 1 << 9;
        public const int CloseSensor = 1 << 10;
        public const int DamageRate = 1 << 11;

        public const int Count = 12;

        public static readonly int[] Triggers = new int[Count]
        {
            RootMotion,
            Move,
            Rotate,
            OpenSensor,
            AnimSpeed,
            ThrustUp,
            Loop,
            TransMotionType,
            SwitchMotion,
            KeepDistFromTarget,
            CloseSensor,
            DamageRate,
        };

        public static readonly string[] TriggerNames = new string[Count]
        {
            "动画根运动",
            "移动",
            "转向",
            "开启武器",
            "动画倍速",
            "向上运动",
            "循环",
            "切换动作类型",
            "切换动作",
            "保持目标距离",
            "关闭武器",
            "受伤害率"
        };
    }
}
