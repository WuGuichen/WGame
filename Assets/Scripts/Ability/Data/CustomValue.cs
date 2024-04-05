using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public enum CustomValueType
    {
        Normal,
        AttrValueAdd,
        AttrValueMult,
    }
    public class CustomValue : IData
    {
        public string DebugName => "数值";

        [EditorData("", EditorDataType.Enum, 0f)]
        public CustomValueType ValueType { get; set; } = CustomValueType.Normal;
        [EditorData("值(千分比)", EditorDataType.Int, 35f)]
        public int ModValue { get; set; }
        
        [EditorData("属性", EditorDataType.AttributeTypeID, 30f)]
        public int AttrID { get; set; }

        public float GetValue(BuffOwner owner)
        {
            float res = 0f;
            var modVal = ModValue * 0.001f;
            switch (ValueType)
            {
                case CustomValueType.Normal:
                    res = modVal;
                    break;
                case CustomValueType.AttrValueAdd:
                    res = owner.GetAttrValue(AttrID) + modVal;
                    break;
                case CustomValueType.AttrValueMult:
                    res = owner.GetAttrValue(AttrID) * modVal;
                    break;
            }
            return res;
        }
        
        public void Deserialize(JsonData jd)
        {
            ValueType = JsonHelper.ReadEnum<CustomValueType>(jd["Type"]);
            AttrID = JsonHelper.ReadInt(jd["AttrID"]);
            ModValue = JsonHelper.ReadInt(jd["ModValue"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "Type", ValueType.ToString());
            JsonHelper.WriteProperty(ref writer, "AttrID", AttrID);
            JsonHelper.WriteProperty(ref writer, "ModValue", ModValue);
            return writer;
        }
    }
}