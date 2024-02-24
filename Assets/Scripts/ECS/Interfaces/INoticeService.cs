
using WGame.Notice;

public interface INoticeService
{
    public void Notice(IMessage message);
    public void AddReciever(int key);
    public void RemoveReciever(int key);
}
