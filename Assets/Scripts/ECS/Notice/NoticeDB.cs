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
        RecieverBeHittedOnDefense.Create(OnDefenseBeHit, MessageBehitted.UID)
    };
}

public class MessageBehitted : IMessage
    {
        public const int UID = 0;
        public int TypeId => UID;
    }

    public class RecieverBeHittedOnDefense : IReciever
    {
        public static RecieverBeHittedOnDefense Create( int key, int messageType)
        {
            return new RecieverBeHittedOnDefense(key,messageType);
        }

        private RecieverBeHittedOnDefense(int key, int messageType, int times = 1) : base(key,messageType, times)
        {
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
            var msg = message as MessageBehitted;
            return true;
        }
    }
