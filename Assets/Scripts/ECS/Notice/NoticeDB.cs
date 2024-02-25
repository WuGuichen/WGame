using WGame.Notice;
using WGame.Runtime;

public class NoticeDB : Singleton<NoticeDB>
{
    public const int OnDefenseBeHit = 0;
    
    public void RemoveReciever( NoticeCenter center, int key)
    {
        center.RemoveReciever(key);
    }
    public void AddReciever(NoticeCenter center, int key)
    {
        if (_recievers.Length > key)
        {
            center.AddReciever(_recievers[key]);
        }
    }
    private IReciever[] _recievers = new IReciever[]
    {
        // 0
        new RecieverBeHittedOnDefense(OnDefenseBeHit)
    };
}

    public class RecieverBeHittedOnDefense : IReciever
    {
        public RecieverBeHittedOnDefense(int key, int times = 1) 
            : base(key, MessageDB.BeHittedID, times)
        {
            // 只可以在特定类型的message下触发
        }

        public override void OnAdded()
        {
            WLogger.Print("OnAdded");
        }

        public override void OnRemoved()
        {
            WLogger.Print("OnRemoved");
        }

        public override void OnTrigger()
        {
            WLogger.Print("OnTrigger");
        }

        public override bool CheckCondition(IMessage message)
        {
            if (message.TypeId != MessageDB.BeHittedID)
            {
                return false;
            }
            var msg = (MessageDB.Define.BeHitted)message;
            return true;
        }
    }
