using UnityTimer;
using WGame.Notice;

public class NoticeServiceImplementation : INoticeService
{
    private NoticeCenter _notice;

    public NoticeServiceImplementation(GameEntity entity)
    {
        _notice = new NoticeCenter(entity);
    }

    public void Notice(IMessage message)
    {
        _notice.Notice(message);
    }

    public void AddReciever(int key, int times = 1, bool replace = true)
    {
        NoticeDB.Inst.AddReciever(_notice, key, times);
    }

    public void AddReciever(int key, float duration, int times = 1, bool replace = true)
    {
        NoticeDB.Inst.AddReciever(_notice, key, times);
        Timer.Register(duration, () =>
        {
            RemoveReciever(key);
        });
    }

    public void RemoveReciever(int key)
    {
        NoticeDB.Inst.RemoveReciever(_notice, key);
    }
}
