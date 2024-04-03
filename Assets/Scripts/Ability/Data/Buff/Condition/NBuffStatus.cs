namespace WGame.Ability
{
    public class NBuffStatus : BuffStatus
    {
        private NBuffData _data;
        public override bool Initialize(BuffManager buffManager, BuffData buff)
        {
            base.Initialize(buffManager, buff);
            _data = _buff as NBuffData;
            
            // 特效相关
            if (!string.IsNullOrEmpty(_data.EffectStart))
            {
                _mgr.Owner.PlayEffect(_data.EffectStart);
            }

            return true;
        }

        public override void Apply(int attrType, ref int addVal, ref int mulVal)
        {
            if (_data.AttrID == attrType)
            {
                addVal += _data.AddValue;
                mulVal += _data.MulValue;
            }
        }

        public override int ChangeAttrType()
        {
            return _data.AttrID;
        }

        protected override void Reset()
        {
            base.Reset();
        }
    }
}