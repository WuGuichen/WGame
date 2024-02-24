namespace WGame.Notice
{
    public abstract class IReciever
    {
        public int LeftNoticeTime { get; internal set; }
        public int MessageType => _messageType;
        private int _messageType;
        public int Key { get; }

        public IReciever(int key, int messageType, int times = 1)
        {
            Key = key;
            _messageType = messageType;
            LeftNoticeTime = times;
        }
        
        /// <summary>
        /// 被移除
        /// </summary>
        public virtual void OnRemoved(){}
        /// <summary>
        /// 被添加
        /// </summary>
        public virtual void OnAdded(){}
        /// <summary>
        /// 被触发
        /// </summary>
        public abstract void OnTrigger();
        public abstract bool CheckCondition(IMessage message);
    }
}
