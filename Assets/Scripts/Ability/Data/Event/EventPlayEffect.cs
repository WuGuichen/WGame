namespace WGame.Ability
{
    using LitJson;
    using UnityEngine;
    using System.Collections.Generic;
    using WGame.Utils;
    
    public enum EffectDummyType
    {
        DummyFollow = 0, // move with dummy
        DummyPosition,   // only use dummy position and don't move with dummy 
        UnitPosition,
        Custom,
        InputByUser,     // user set 'PropertyName.sInputSkillPosition'
    }
    internal class EventPlayEffect : IData, IEventData
    {
        public string DebugName => GetType().ToString();
        [EditorData("特效名称", EditorDataType.GameObject)]
        public string EffectName { get; set; }
        [EditorData("是否循环", EditorDataType.Bool)]
        public bool IsLoop { get; set; }
        public string LoopName { get; set; }
        [EditorData("特效类型", EditorDataType.Enum)]
        public EffectDummyType DummyType { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 EulerAngle { get; set; }
        public bool UseRandom { get; set; }
        public List<string> RandomEffectList { get; set; } = new();
        public EventDataType EventType => EventDataType.PlayEffect;

        public void Deserialize(JsonData jd)
        {
            EffectName = JsonHelper.ReadString(jd["EffectName"]);
            IsLoop = JsonHelper.ReadBool(jd["IsLoop"]);
            LoopName = JsonHelper.ReadString(jd["LoopName"]);
            DummyType = JsonHelper.ReadEnum<EffectDummyType>(jd["DummyType"]);
            // DummyRoot = JsonHelper.ReadString(jd["DummyRoot"]);
            // DummyAttach = JsonHelper.ReadString(jd["DummyAttach"]);
            Position = JsonHelper.ReadVector3(jd["Position"]);
            EulerAngle = JsonHelper.ReadVector3(jd["Euler"]);
            UseRandom = JsonHelper.ReadBool(jd["UseRandom"]);
            RandomEffectList = JsonHelper.ReadListString(jd["RandomEffectList"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "EffectName", EffectName);
            JsonHelper.WriteProperty(ref writer, "IsLoop", IsLoop);
            JsonHelper.WriteProperty(ref writer, "LoopName", LoopName);
            JsonHelper.WriteProperty(ref writer, "DummyType", DummyType.ToString());
            // JsonHelper.WriteProperty(ref writer, "DummyRoot", DummyRoot);
            // JsonHelper.WriteProperty(ref writer, "DummyAttach", DummyAttach);
            JsonHelper.WriteProperty(ref writer, "Position", Position);
            JsonHelper.WriteProperty(ref writer, "Euler", EulerAngle);
            JsonHelper.WriteProperty(ref writer, "UseRandom", UseRandom);
            JsonHelper.WriteProperty(ref writer, "RandomEffectList", RandomEffectList);

            return writer;
        }

        public IEventData Clone()
        {
            EventPlayEffect evt = new EventPlayEffect();
            evt.EffectName = EffectName;
            evt.IsLoop = IsLoop;
            evt.LoopName = LoopName;
            evt.DummyType = DummyType;
            // evt.mDummyRoot = this.mDummyRoot;
            // evt.mDummyAttach = this.mDummyAttach;
            evt.Position = Position;
            evt.EulerAngle = EulerAngle;
            evt.UseRandom = UseRandom;
            evt.RandomEffectList.AddRange(this.RandomEffectList);

            return evt;
        }
    }
}