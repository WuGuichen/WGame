
using WGame.Notice;

public interface INoticeService
{
    public void Notice(IMessage message);
    public void AddReciever(int key);
    public void AddReciever(int key, float duration, bool replace = true);
    public void RemoveReciever(int key);
}
