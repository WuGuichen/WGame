using System.Collections.Generic;

namespace WGame.Ability
{
    public interface BuffOwner
    {
        void AddBuff(List<string> buffList);
        void RemoveBuff(List<string> buffList);
        int GetAttrValue(int attrType, bool addBuff = false);
        void AddAttrValue(int attrType, float changeValue);
        int GetAttrValue(int characterID, int attrType);
        void PlayEffect(string effName, int partType = 0, float duration = 10f);
        int EntityID { get; }
    }
}