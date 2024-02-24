using WGame.Notice;

public class NoticeServiceImplementation : INoticeService
{
    private NoticeCenter _notice;

    public NoticeServiceImplementation()
    {
        _notice = new NoticeCenter();
    }

    public void Notice(IMessage message)
    {
        _notice.Notice(message);
    }

    public void AddReciever(int key)
    {
        NoticeDB.Inst.AddReciever(_notice, key);
    }

    public void RemoveReciever(int key)
    {
        NoticeDB.Inst.RemoveReciever(_notice, key);
    }
}
