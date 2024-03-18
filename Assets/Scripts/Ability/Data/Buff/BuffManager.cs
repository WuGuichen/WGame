using System.Collections.Generic;

namespace WGame.Ability
{
    public class BuffManager
    {
        private LinkedList<BuffStatus> _buffList = new();
        public BuffOwner Owner { get; }

        public BuffManager(BuffOwner owner)
        {
            Owner = owner;
        }

        public void AddBuff(int id)
        {
            if (!DataMgr.Inst.TryGetBuffData(id, out var buffFactoryData))
            {
                return;
            }

            var buffData = buffFactoryData.Buff;
            using (var itr = _buffList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    if (id == itr.Current.ID)
                    {
                    }
                }
            }
        }
    }
}