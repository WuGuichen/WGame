namespace WGame.Notice
{
    public interface IReciever
    {
        public int LeftNoticeTime { get; set; }
        /// <summary>
        /// 写死的对应消息类型(如果需要后续可以相应多种消息类型）
        /// </summary>
        public int MessageType { get; }
        public int Key { get; set; }

        /// <summary>
        /// 被移除
        /// </summary>
        public void OnRemoved(GameEntity entity){}
        /// <summary>
        /// 被添加
        /// </summary>
        public void OnAdded(GameEntity entity){}
        /// <summary>
        /// 被触发
        /// </summary>
        public void OnTrigger(GameEntity entity, IMessage message);
        public bool CheckCondition(IMessage message);

        public IReciever Build(int key, int times);
    }
}
