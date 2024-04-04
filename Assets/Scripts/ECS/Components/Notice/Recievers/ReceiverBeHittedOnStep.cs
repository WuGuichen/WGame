using UnityTimer;

namespace WGame.Notice
{
    public class ReceiverBeHittedOnStep : IReciever
    {
        public void OnAdded(GameEntity entity)
        {
            entity.linkAbility.Ability.abilityEvade.service.Enable();
        }

        public int LeftNoticeTime { get; set; }
        public int MessageType => MessageDB.BeHittedID;
        public int Key { get; set; }

        public void OnRemoved(GameEntity entity)
        {
            entity.linkAbility.Ability.abilityEvade.service.Disable();
        }

        public void OnTrigger(GameEntity entity, IMessage message)
        {
            // UnityEngine.Time.timeScale = 0.2f;
            var msg = message as MessageDB.Define.BeHitted;
            entity.ReplaceCharacterTimeScale(0.2f);
            msg.hitInfo.entity.ReplaceCharacterTimeScale(0.2f);
            Timer.Register(0.3f, () =>
            {
                entity.ReplaceCharacterTimeScale(1f);
                msg.hitInfo.entity.ReplaceCharacterTimeScale(1f);
            });
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
            return new ReceiverBeHittedOnStep(){Key = key, LeftNoticeTime = times};
        }
    }
}