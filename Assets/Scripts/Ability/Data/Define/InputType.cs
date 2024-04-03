namespace WGame.Ability
{
    public class InputType
    {
        public const int Attack = 1;
        public const int Defense = 1 << 1;
        public const int HoldAttack = 1 << 2;
        public const int JumpAttack = 1 << 3;
        public const int Jump = 1 << 4;
        public const int LocalMotion = 1 << 5;
        public const int Step = 1 << 6;
        public const int Special = 1 << 7;

        public const int Count = 8;
    }
}