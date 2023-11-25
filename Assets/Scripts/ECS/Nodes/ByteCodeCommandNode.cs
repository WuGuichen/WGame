namespace Motion
{
    [System.Serializable]
    public class ByteCodeCommandNode : TriggerNode
    {
        public int[] commandTime;
        public int[] commandType;
        public int[] commandParam;

        public ByteCodeCommandNode(float time) : base(time)
        {
            
        }

        public override string Name => "字节码命令节点";
    }
}