namespace WGame.Ability
{
    public interface IEventData
    {
        EventDataType EventType { get; }
        void Enter(EventOwner owner);
        void Duration(EventOwner owner, float deltaTime, int duration, int totalTime);
        void Exit(EventOwner owner, bool isBreak);
        IEventData Clone();
    }
}