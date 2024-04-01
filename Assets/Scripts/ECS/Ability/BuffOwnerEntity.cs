using System.Collections.Generic;
using UnityEngine;
using WGame.Res;

namespace WGame.Ability
{
    public class BuffOwnerEntity : BuffOwner
    {
        private GameEntity _entity;
        private int entityID;
        public void PlayEffect(string effName, int partType = 0, float duration = 10f)
        {
            var partTrans = EntityUtils.GetCharacterPart(_entity, partType);
            effName = WUtils.GetFileNameWithoutExtension(effName);
            EffectMgr.LoadEffect(effName, partTrans, partTrans.position, Quaternion.identity, duration);
        }

        public int EntityID => entityID;

        public BuffOwnerEntity(GameEntity entity)
        {
            _entity = entity;
            entityID = _entity.instanceID.ID;
        }
        public void AddBuff(List<string> buffList)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveBuff(List<string> buffList)
        {
            throw new System.NotImplementedException();
        }

        public int GetAttrValue(int attrType, bool addBuff = false)
        {
            return _entity.attribute.value.Get(attrType, addBuff);
        }

        public int GetAttrValue(int characterID, int attrType)
        {
            var target = EntityUtils.GetGameEntity(characterID);
            return target.attribute.value.Get(attrType);
        }
    }
}