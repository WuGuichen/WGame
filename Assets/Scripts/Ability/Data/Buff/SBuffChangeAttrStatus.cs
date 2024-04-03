namespace WGame.Ability
{
    public class SBuffChangeAttrStatus : BuffStatus
    {
        private int changeAttr;

        public override bool Initialize(BuffManager buffManager, BuffData buff)
        {
            base.Initialize(buffManager, buff);
            var data = buff as SBuffChangeAttrData;
            changeAttr = data.AttrID;

            if (!string.IsNullOrEmpty(data.EffStart))
            {
                buffManager.Owner.PlayEffect(data.EffStart, data.EffStartPart);
            }

            float value = buffManager.Owner.GetAttrValue(data.AttrID, true);
            var changeVal = value * (data.MulValue * mPrecision) + (data.AddValue * mPrecision);
            WLogger.Print(changeVal);
            buffManager.Owner.AddAttrValue(data.AttrID, changeVal);
            return false;
        }

        public override int ChangeAttrType()
        {
            return changeAttr;
        }
    }
}