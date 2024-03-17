namespace WGame.Notice
{
    public interface IAction
    {
        void Do(GameEntity entity, IMessage message);
    }
}