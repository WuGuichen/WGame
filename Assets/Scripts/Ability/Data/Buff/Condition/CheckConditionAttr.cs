using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public sealed class CheckConditionAttr : ICondition, IData
    {
        [EditorData("比较类型", EditorDataType.Enum)]
        public CompareType CmpType { get; set; } = CompareType.Equal;
        [EditorData("属性类型", EditorDataType.AttributeTypeID)]
        public int AttType { get; set; } = 1;

        [EditorData("比较值", EditorDataType.Object, 50)]
        public CustomValue CmpValue { get; set; } = new CustomValue();

        private CBuffStatus _owner;

        public ConditionType CondType => ConditionType.CheckHP;
        public ICondition Clone()
        {
            return new CheckConditionAttr()
            {
                CmpType = CmpType,
                AttType = AttType,
                CmpValue = CmpValue
            };
        }
        
        public void Init(CBuffStatus owner)
        {
            _owner = owner;
            TriggerMgr.Inst.Register(TriggerEventType.ChangeAttr, OnEvent);
        }

        private void OnEvent(TriggerContext context)
        {
            var entityId = context.GetProperty("target").AsInt();
            if (entityId == _owner.BuffMgr.Owner.EntityID)
            {
                _owner.OnEvent();
            }
        }

        public void Destroy()
        {
            TriggerMgr.Inst.Unregister(TriggerEventType.BeHit, OnEvent);
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public bool CheckBuff(BuffOwner buffOwner)
        {
            var curValue = buffOwner.GetAttrValue(AttType);
            float cmpValue = CmpValue.GetValue(buffOwner);
            return CustomCompare<float>.Compare(CmpType, curValue, cmpValue);
        }

        public string DebugName => "比较HP";
        public void Deserialize(JsonData jd)
        {
            CmpType = JsonHelper.ReadEnum<CompareType>(jd["CmpType"]);
            AttType = JsonHelper.ReadInt(jd["Attr"]);
            CmpValue.Deserialize(jd["CmpValue"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "CmpType", CmpType.ToString());
            JsonHelper.WriteProperty(ref writer, "Attr", CmpType.ToString());
            writer.WritePropertyName("CmpValue");
            writer.WriteObjectStart();
            writer = CmpValue.Serialize(writer);
            writer.WriteObjectEnd();

            return writer;
        }
    }
}