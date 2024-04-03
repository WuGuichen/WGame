using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class NBuffData : BuffData
    {
        [EditorData("属性类型", EditorDataType.AttributeTypeID)]
        public int AttrID { get; set; } = 99;
        [EditorData("属性加值", EditorDataType.Int)]
        public int AddValue { get; set; } = 0;
        [EditorData("属性乘值", EditorDataType.Int)]
        public int MulValue { get; set; } = 0;

        [EditorData("开始特效", EditorDataType.GameObject)]
        public string EffectStart { get; set; } = "/Effects/HCFX_Buff_Up.prefab";

        [EditorData("开始特效部位", EditorDataType.TypeID, 7)]
        public int EffStartPart { get; set; } = 0;
        [EditorData("持续特效", EditorDataType.GameObject)]
        public string EffectKeep { get; set; }
        [EditorData("持续特效部位", EditorDataType.TypeID, 7)]
        public int EffKeepPart { get; set; } = 0;

        public override void Deserialize(JsonData jd)
        {
            base.Deserialize(jd);

            var cfg = jd["NBuff"];
            AttrID = JsonHelper.ReadInt(cfg[0]);
            AddValue = JsonHelper.ReadInt(cfg[1]);
            MulValue = JsonHelper.ReadInt(cfg[2]);
            EffectStart = JsonHelper.ReadString(cfg[3]);
            EffStartPart = JsonHelper.ReadInt(cfg[4]);
            EffectKeep = JsonHelper.ReadString(cfg[5]);
            EffKeepPart = JsonHelper.ReadInt(cfg[6]);
        }

        public override JsonWriter Serialize(JsonWriter writer)
        {
            base.Serialize(writer);
            
            writer.WritePropertyName("NBuff");
            writer.WriteArrayStart();
            writer.Write(AttrID);
            writer.Write(AddValue);
            writer.Write(MulValue);
            writer.Write(EffectStart);
            writer.Write(EffStartPart);
            writer.Write(EffectKeep);
            writer.Write(EffKeepPart);
            writer.WriteArrayEnd();

            return writer;
        }
    }
}