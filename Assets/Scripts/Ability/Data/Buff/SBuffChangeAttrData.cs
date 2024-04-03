using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class SBuffChangeAttrData : BuffData
    {
        [EditorData("属性类型", EditorDataType.AttributeTypeID)]
        public int AttrID { get; set; }
        [EditorData("增减值", EditorDataType.Int)]
        public int AddValue { get; set; }
        [EditorData("增减比例", EditorDataType.Int)]
        public int MulValue { get; set; }
        [EditorData("特效", EditorDataType.GameObject)]
        public string EffStart { get; set; } = "/Effects/HCFX_Buff_Up.prefab";
        [EditorData("持续部位", EditorDataType.TypeID, 7)]
        public int EffStartPart { get; set; } = 0;
        public override void Deserialize(JsonData jd)
        {
            base.Deserialize(jd);
            var cfg = jd["SBuff"];
            AttrID = JsonHelper.ReadInt(cfg[0]);
            AddValue = JsonHelper.ReadInt(cfg[1]);
            MulValue = JsonHelper.ReadInt(cfg[2]);
            EffStart = JsonHelper.ReadString(cfg[3]);
            EffStartPart = JsonHelper.ReadInt(cfg[4]);
        }

        public override JsonWriter Serialize(JsonWriter writer)
        {
            base.Serialize(writer);
            
            writer.WritePropertyName("SBuff");
            writer.WriteArrayStart();
            writer.Write(AttrID);
            writer.Write(AddValue);
            writer.Write(MulValue);
            writer.Write(EffStart);
            writer.Write(EffStartPart);
            writer.WriteArrayEnd();
            return writer;
        }
    }
}