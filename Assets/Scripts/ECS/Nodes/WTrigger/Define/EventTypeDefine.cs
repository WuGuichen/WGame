namespace WGame.Trigger
{
    public class InputSignalEvent
    {
        public const int WasPressed = 1;
        public const int WasReleased = 2;
        public const int IsPressing = 3;
        public const int IsHold = 4;
    }

    public class SensorEvent
    {
        public const int Enter = 1;
        public const int Contact = 2;
        public const int Exit = 3;
    }
}