using System.Collections.Generic;

namespace WGame.Ability
{
    public interface BuffOwner
    {
        void AddBuff(List<string> buffList);
        void RemoveBuff(List<string> buffList);
    }
}