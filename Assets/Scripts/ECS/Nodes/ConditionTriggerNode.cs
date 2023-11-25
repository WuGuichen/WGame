using WGame.Trigger;

namespace Motion
{
    [System.Serializable]
    public class ConditionTriggerNode : TriggerNode
    {
        public WtEventType wtEventData;
        public int conditionUID;
        public int triggerTimes;
        public bool clearOnExit;
        public int duration;
        
        public override string Name => "触发器节点";

        public ConditionTriggerNode(float time) : base(time)
        {
            
        }
    }
}