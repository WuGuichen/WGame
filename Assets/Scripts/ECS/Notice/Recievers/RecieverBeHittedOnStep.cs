using UnityTimer;

namespace WGame.Notice
{
    public struct RecieverBeHittedOnStep : IReciever
    {
        public void OnAdded(GameEntity entity)
        {
            entity.linkAbility.Ability.abilityEvade.service.Enable();
            WLogger.Print("Added");
        }

        public int LeftNoticeTime { get; set; }
        public int MessageType => MessageDB.BeHittedID;
        public int Key { get; set; }

        public void OnRemoved(GameEntity entity)
        {
            entity.linkAbility.Ability.abilityEvade.service.Disable();
            WLogger.Print("Remove");
        }

        public void OnTrigger(GameEntity entity, IMessage message)
        {
            UnityEngine.Time.timeScale = 0.2f;
            WLogger.Print("极限");
            Timer.Register(0.1f, () => { UnityEngine.Time.timeScale = 1f; });
        }

        public bool CheckCondition(IMessage message)
        {
            var msg = (MessageDB.Define.BeHitted)message;
            if (msg.hitInfo.part == EntityPartType.Evasion)
            {
                return true;
            }

            return false;
        }

        public IReciever Build(int key, int times)
        {
            return new RecieverBeHittedOnStep(){Key = key, LeftNoticeTime = times};
        }
    }
}