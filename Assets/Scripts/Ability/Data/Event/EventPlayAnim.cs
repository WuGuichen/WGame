using WGame.Utils;

namespace WGame.Ability
{
    using LitJson;
    
    public sealed class EventPlayAnim : IData, IEventData
    {
        public string DebugName => AnimName;
        
        [EditorData("动画名", EditorDataType.AnimationClip)]
        public string AnimName { get; set; } = "Longs_Jump_Platformer_Land";
        [EditorData("开始时间偏移", EditorDataType.Int)]
        public int PlayOffsetStart { get; set; }
        [EditorData("结束时间偏移", EditorDataType.Int)]
        public int PlayOffsetEnd { get; set; }

        [EditorData("过渡时间", EditorDataType.Int)]
        public int TransDuration { get; set; } = 100;
        [EditorData("部位", EditorDataType.TypeID, 0)]
        public int LayerType { get; set; }
        [EditorData("部位重置", EditorDataType.Bool)]
        public bool ResetLayer { get; set; }

        [EditorData("播放速率(%)", EditorDataType.Int)]
        public int SpeedRate { get; set; } = 100;

        private float _speedRate;
        
        public EventDataType EventType => EventDataType.PlayAnim;
        public void Enter(EventOwner owner)
        {
            owner.SetAnimSpeed(_speedRate);
            owner.PlayAnim(AnimName, PlayOffsetStart, PlayOffsetEnd, TransDuration, LayerType, ResetLayer);
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
        }

        public void Exit(EventOwner owner, bool isBreak)
        {
        }

        public void Deserialize(JsonData jd)
        {
            var cfg = jd["AnimCfg"];
            AnimName = JsonHelper.ReadString(cfg[0]);
            PlayOffsetStart = JsonHelper.ReadInt(cfg[1]);
            PlayOffsetEnd = JsonHelper.ReadInt(cfg[2]);
            TransDuration = JsonHelper.ReadInt(cfg[3]);
            LayerType = JsonHelper.ReadInt(cfg[4]);
            ResetLayer = JsonHelper.ReadBool(cfg[5]);
            SpeedRate = JsonHelper.ReadInt(cfg[6]);
            _speedRate = SpeedRate * 0.01f;
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            writer.WritePropertyName("AnimCfg");
            writer.WriteArrayStart();
            writer.Write(AnimName);
            writer.Write(PlayOffsetStart);
            writer.Write(PlayOffsetEnd);
            writer.Write(TransDuration);
            writer.Write(LayerType);
            writer.Write(ResetLayer);
            writer.Write(SpeedRate);
            writer.WriteArrayEnd();
            return writer;
        }

        public IEventData Clone()
        {
            var evt = new EventPlayAnim
            {
                AnimName = AnimName,
                PlayOffsetStart = PlayOffsetStart,
                TransDuration = TransDuration,
                LayerType = LayerType,
                ResetLayer = ResetLayer
            };

            return evt;
        }
    }
}