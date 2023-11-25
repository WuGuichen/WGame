
namespace Motion
{
    [System.Serializable]
    public class PlayAnimationNode : EventNode
    {
        public int AnimClipID;

        public int playLayer = AnimLayerType.Base;
        public float duration;
        public float playTime;
        public float playTransTime = 0.1f;
        public bool isResetBaseLayer = false;
        public override float timeEnd => base.timeEnd + duration;

        public override string Name => "播放动画节点";

        public PlayAnimationNode(float time) : base(time)
        {
            
        }

        // public override void ToLua(StreamWriter stream)
        // {
        //     // luaConfig = string.Format("{{ Event = 'PlayAnimation',Name = '{0}' }}", triggerName);
        //     base.ToLua(stream);
        // }
    }
}
