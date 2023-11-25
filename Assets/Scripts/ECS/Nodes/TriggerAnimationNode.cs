namespace Motion
{
    [System.Serializable]
    public class TriggerAnimationNode : TriggerNode
    {
        public int[] triggerTime;
        public int[] triggerType;

        public int[] triggerParam;

        public TriggerAnimationNode(float time) : base(time)
        {
            
        }

        public override string Name => "动画触发节点";
    }
}