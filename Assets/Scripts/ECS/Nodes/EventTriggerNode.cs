using WGame.Trigger;

namespace Motion
{
    [System.Serializable]
    public class EventTriggerNode : TriggerNode
    {
        public int[] triggerTime;
        public int[] triggerType;
        public int[] triggerParam;
        public WtEventType eventType = new();
        public int duration;

        public override float timeEnd => duration*0.001f;

        public EventTriggerNode(float time) : base(time)
        {
            
        }

        public override string Name => "动画事件触发节点";
    }
}