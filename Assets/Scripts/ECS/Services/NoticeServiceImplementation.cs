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

    public void AddReciever(int key)
    {
        NoticeDB.Inst.AddReciever(_notice, key);
    }

    public void AddReciever(int key, float duration, bool replace = true)
    {
        NoticeDB.Inst.AddReciever(_notice, key);
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
