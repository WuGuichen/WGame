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
        [EditorData("开始特效", EditorDataType.GameObject)]
        public string EffectKeep { get; set; }

        public override void Deserialize(JsonData jd)
        {
            base.Deserialize(jd);

            AttrID = JsonHelper.ReadInt(jd["Attr"]);
            AddValue = JsonHelper.ReadInt(jd["AddVal"]);
            MulValue = JsonHelper.ReadInt(jd["MulVal"]);
            EffectStart = JsonHelper.ReadString(jd["EffStart"]);
            EffectKeep = JsonHelper.ReadString(jd["EffKeep"]);
        }

        public override JsonWriter Serialize(JsonWriter writer)
        {
            base.Serialize(writer);
            
            JsonHelper.WriteProperty(ref writer, "Attr", AttrID);
            JsonHelper.WriteProperty(ref writer, "AddVal", AddValue);
            JsonHelper.WriteProperty(ref writer, "MulVal", MulValue);
            JsonHelper.WriteProperty(ref writer, "EffStart", EffectStart);
            JsonHelper.WriteProperty(ref writer, "EffKeep", EffectKeep);

            return writer;
        }
    }
}