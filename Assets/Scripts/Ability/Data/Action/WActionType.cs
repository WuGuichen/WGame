namespace WGame.Ability
{
    public class WActionType
    {
        public const int SetUnbalance = 0;
        public const int MoveCamera = 1;
        public const int RotateCamera = 2;

        public static readonly string[] Names =
        {
            "设置失衡",
            "移动相机",
            "旋转相机",
        };
    }
}