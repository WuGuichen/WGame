using WGame.Notice;
using WGame.Runtime;

public class NoticeDB : Singleton<NoticeDB>
{
    public const int OnStepBeHit = 0;
    public const int OnUseAbility = 1;
    public const int OnDefenseBeHit = 2;

    public void RemoveReciever(NoticeCenter center, int key)
    {
        center.RemoveReciever(key);
    }

    public void AddReciever(NoticeCenter center, int key, int times = 9, bool replace = false)
    {
        if (_recievers.Length > key)
        {
            center.AddReciever(_recievers[key].Build(key, times), replace);
        }
        else
        {
            WLogger.Error("尝试添加未加入_recievers的接收者");
        }
    }
    private readonly IReciever[] _recievers = {
        new ReceiverBeHittedOnStep(),
        new ReceiverGenAbilityEntity(),
        new ReceiverBeHittedOnDefense()
    };

    public void InternalTriggerReciever(int key, GameEntity entity, IMessage message)
    {
        if (_recievers.Length > key)
        {
            var reciever = _recievers[key];
            if (reciever.MessageType == message.TypeId)
            {
                _recievers[key].OnTrigger(entity, message);
                return;
            }
            WLogger.Warning("消息不合法");
        }
        WLogger.Warning("没有接收者");
    }
}
