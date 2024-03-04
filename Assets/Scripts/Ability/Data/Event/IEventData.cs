namespace WGame.Ability
{
    public interface IEventData
    {
        EventDataType EventType { get; }
        IEventData Clone();
    }
}