using LitJson;

namespace WGame.Ability
{
    public sealed class ConditionAlways : ICondition, IData
    {
        public ConditionType CondType { get; }
        public ICondition Clone()
        {
            return this;
        }

        public void Init(CBuffStatus owner)
        {
        }

        public void Destroy()
        {
        }

        public void Reset()
        {
        }

        public bool CheckBuff(BuffOwner buffOwner)
        {
            return true;
        }

        public static ConditionAlways Instance = new ConditionAlways();
        public string DebugName { get; }
        public void Deserialize(JsonData jd)
        {
            
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            return writer;
        }
    }
    public interface ICondition
    {
        ConditionType CondType { get; }
        ICondition Clone();

        void Init(CBuffStatus owner);
        void Destroy();
        void Reset();
        bool CheckBuff(BuffOwner buffOwner);
    }
}