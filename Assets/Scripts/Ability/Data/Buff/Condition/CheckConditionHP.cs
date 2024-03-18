using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public sealed class CheckConditionHP : ICondition, IData
    {
        [EditorData("比较类型", EditorDataType.Enum)]
        public CompareType CmpType { get; set; } = CompareType.Equal;

        [EditorData("比较值", EditorDataType.Object, 50)]
        public CustomValue CmpValue { get; set; } = new CustomValue();

        private CBuffStatus _owner;

        public ConditionType CondType => ConditionType.CheckHP;
        public ICondition Clone()
        {
            return new CheckConditionHP()
            {
                CmpType = CmpType,
                CmpValue = CmpValue
            };
        }

        public void Init(CBuffStatus owner)
        {
            throw new System.NotImplementedException();
        }

        public void Destroy()
        {
            throw new System.NotImplementedException();
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public bool CheckBuff(BuffOwner buffOwner)
        {
            throw new System.NotImplementedException();
        }

        public string DebugName => "比较HP";
        public void Deserialize(JsonData jd)
        {
            CmpType = JsonHelper.ReadEnum<CompareType>(jd["CmpType"]);
            CmpValue.Deserialize(jd["CmpValue"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "CmpType", CmpType.ToString());
            writer.WritePropertyName("CmpValue");
            writer.WriteObjectStart();
            writer = CmpValue.Serialize(writer);
            writer.WriteObjectEnd();

            return writer;
        }
    }
}