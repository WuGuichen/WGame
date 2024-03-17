using System.Collections.Generic;

namespace WGame.Notice
{
    public class NoticeCenter
    {
        private class RecieverReference
        {
            public static Stack<RecieverReference> _pool = new();

            public static RecieverReference Get(IReciever reciever)
            {
                if (_pool.Count > 0)
                {
                    return _pool.Pop().Set(reciever);
                }

                return new RecieverReference(reciever);
            }

            public static void Push(RecieverReference reference)
            {
                reference.Dispose();
                _pool.Push(reference);
            }

            public RecieverReference(IReciever reciever)
            {
                Reciever = reciever;
            }

            public void Dispose()
            {
                Reciever = null;
            }

            public RecieverReference Set(IReciever reciever)
            {
                Reciever = reciever;
                return this;
            }
            
            public IReciever Reciever { get; private set; }
        }
        
        // 用数组，避免rehash
        // 消息对应的接收器列表
        private SparseSet<LinkedList<RecieverReference>> _messageTypeList = new();
        // 唯一id注册接收器数组字典
        private SparseSet<RecieverReference> _recieverTypeList = new();

        // 绑定GameEntity
        private GameEntity _entity;
        
        public NoticeCenter(GameEntity entity)
        {
            _entity = entity;
        }

        /// <summary>
        /// 添加接收器
        /// </summary>
        /// <param name="key">唯一ID</param>
        /// <param name="reciever">接收器</param>
        /// <param name="replace">当已有唯一ID时替换成新的</param>
        public void AddReciever(IReciever reciever, bool replace = false)
        {
            if (_recieverTypeList.TryGet(reciever.Key, out var refReciever))
            {
                if (replace)
                {
                    // 替换成新的接收器
                    // 触发移除效果
                    refReciever.Reciever.OnRemoved(_entity);
                    // 触发添加效果
                    reciever.OnAdded(_entity);

                    // 替换接收器
                    refReciever.Set(reciever);
                }
            }
            else
            { 
                // 添加到引用
                refReciever = RecieverReference.Get(reciever);
                _recieverTypeList.Add(reciever.Key, refReciever);
                // 添加到接收器列表
                if (!_messageTypeList.TryGet(reciever.MessageType, out var linkedList))
                {
                    linkedList = new LinkedList<RecieverReference>();
                    _messageTypeList.Add(reciever.MessageType, linkedList);
                }
                linkedList.AddLast(refReciever);
                // 触发添加效果
                reciever.OnAdded(_entity);
            }
        }

        public void RemoveReciever(int key)
        {
            if (_recieverTypeList.TryGet(key, out var refReciever))
            {
                // 确实注册了接收器
                // 触发移除效果
                refReciever.Reciever.OnRemoved(_entity);
                
                // 接收器列表中移除
                _messageTypeList[refReciever.Reciever.MessageType].Remove(refReciever);
                
                // 引用回收
                RecieverReference.Push(refReciever);
                _recieverTypeList.Remove(key);
            }
        }

        /// <summary>
        /// 发出消息通知
        /// </summary>
        /// <param name="message"></param>
        public void Notice(IMessage message)
        {
            if (_messageTypeList.TryGet(message.TypeId, out var recievers))
            {
				var currentNode = recievers.First;
                while (currentNode != null)
                {
                    var reciever = currentNode.Value.Reciever;
                    if (reciever.CheckCondition(message))
                    {
                        reciever.OnTrigger(_entity, message);
                        if (reciever.LeftNoticeTime < 1000)
                        {
                            reciever.LeftNoticeTime--;
                            if (reciever.LeftNoticeTime <= 0)
                            {
                                RemoveReciever(reciever.Key);
                            }
                        }
                    }
                    currentNode = currentNode.Next;
                }
            }
        }

        /// <summary>
        /// 直接触发效果
        /// </summary>
        /// <param name="recieverKey"></param>
        /// <param name="message"></param>
        public void TriggerReciever(int recieverKey, IMessage message)
        {
            NoticeDB.Inst.InternalTriggerReciever(recieverKey, _entity, message);
        }
    }
}