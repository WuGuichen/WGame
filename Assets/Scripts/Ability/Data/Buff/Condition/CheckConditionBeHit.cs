using LitJson;

namespace WGame.Ability
{
    public class CheckConditionBeHit : ICondition, IData
    {
        public ConditionType CondType => ConditionType.OnBeHit;
        
        private CBuffStatus _owner;
        
        public ICondition Clone()
        {
            return new CheckConditionBeHit();
        }

        public void Init(CBuffStatus owner)
        {
            _owner = owner;
            TriggerMgr.Inst.Register(TriggerEventType.BeHit, OnEvent);
        }
        
        private void OnEvent(TriggerContext context)
        {
            var victim = context.GetProperty("victim").AsInt();
            if (victim == _owner.EntityID)
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
        }

        public bool CheckBuff(BuffOwner buffOwner)
        {
            return true;
        }

        public string DebugName { get; }
        public void Deserialize(JsonData jd)
        {
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            return writer;
        }
    }
}