
using WGame.Notice;

public interface INoticeService
{
    public void Notice(IMessage message);
    public void AddReciever(int key, int times = 1, bool replace = true);
    public void AddReciever(int key, float duration, int times = 1, bool replace = true);
    public void RemoveReciever(int key);
}
