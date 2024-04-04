namespace WGame.Notice
{
    public class ReceiverGenAbilityEntity : IReciever
    {
        public int LeftNoticeTime { get; set; }
        public int MessageType => MessageDB.CastSkillID;
        public int Key { get; set; }
        
        // 技能前摇完成的释放瞬间
        public void OnTrigger(GameEntity entity, IMessage message)
        {
            var msg = (MessageDB.Define.CastSkill)message;
            entity.linkAbility.Ability.abilityService.service.GenEntity(msg.Info);
        }

        public bool CheckCondition(IMessage message)
        {
            return true;
        }

        public IReciever Build(int key, int times)
        {
            return new ReceiverGenAbilityEntity(){Key = key, LeftNoticeTime = times};
        }
    }
}