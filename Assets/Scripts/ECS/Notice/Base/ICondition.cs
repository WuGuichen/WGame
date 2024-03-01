namespace WGame.Notice
{
    public interface ICondition
    {
        bool Check(GameEntity entity, IMessage message);
    }
}