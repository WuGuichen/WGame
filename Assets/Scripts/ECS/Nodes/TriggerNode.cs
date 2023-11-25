namespace Motion
{
    public class TriggerNode : EventNode
    {
        public override string Name => "普通触发节点";
        
        public TriggerNode(){}

        public TriggerNode(float time)
        {
            this.time = time;
        }
    }
}