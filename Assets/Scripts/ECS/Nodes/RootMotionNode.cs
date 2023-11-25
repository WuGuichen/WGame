namespace Motion
{
    [System.Serializable]
    public class RootMotionNode : EventNode
    {
        public float rootMotionRate;
        public float duration;

        public override float timeEnd => base.timeEnd + duration;

        public override string Name => "动画根运动节点";

        public RootMotionNode(float time) : base(time)
        {
            
        }
    }
}