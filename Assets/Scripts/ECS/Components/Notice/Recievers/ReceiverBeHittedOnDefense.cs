using WGame.Res;

namespace WGame.Notice
{
    public class ReceiverBeHittedOnDefense : IReciever
    {
        public void OnAdded(GameEntity entity)
        {
        }

        public int LeftNoticeTime { get; set; }
        public int MessageType => MessageDB.BeHittedID;
        public int Key { get; set; }

        public void OnRemoved(GameEntity entity)
        {
        }

        public void OnTrigger(GameEntity entity, IMessage message)
        {
            var msg = message as MessageDB.Define.BeHitted;
            var info = msg.hitInfo;
            EffectMgr.LoadEffect("HCFX_Hit_10", info.pos, info.rot);
            var fwd = -info.dir;
            fwd.y = 0f;
            entity.gameViewService.service.Model.parent.forward = fwd;
        }

        public bool CheckCondition(IMessage message)
        {
            return true;
        }

        public IReciever Build(int key, int times)
        {
            return new ReceiverBeHittedOnDefense(){Key = key, LeftNoticeTime = times};
        }
    }
}