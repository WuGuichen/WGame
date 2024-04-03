using System.Collections.Generic;
using UnityEngine;
using WGame.Res;

namespace WGame.Ability
{
    public class BuffOwnerEntity : BuffOwner
    {
        private GameEntity _entity;
        private IAbility _abilityService;
        private int entityID;
        public void PlayEffect(string effName, int partType = 0, float duration = 10f)
        {
            var partTrans = EntityUtils.GetCharacterPart(_entity, partType);
            effName = WUtils.GetFileNameWithoutExtension(effName);
            EffectMgr.LoadEffect(effName, partTrans, partTrans.position, Quaternion.identity, duration);
        }

        public int EntityID => entityID;

        public BuffOwnerEntity(GameEntity entity, IAbility abilityService)
        {
            _entity = entity;
            _abilityService = abilityService;
            entityID = _entity.instanceID.ID;
        }
        public void AddBuff(List<string> buffList)
        {
            foreach (var buffName in buffList)
            {
                _abilityService.BuffManager.AddBuff(buffName);
            }
        }

        public void RemoveBuff(List<string> buffList)
        {
            foreach (var buffName in buffList)
            {
                _abilityService.BuffManager.DelBuff(buffName);
            }
        }

        public int GetAttrValue(int attrType, bool addBuff = false)
        {
            return _entity.attribute.value.Get(attrType, addBuff);
        }

        public void AddAttrValue(int attrType, float changeValue)
        {
            _entity.attribute.value.Add(_entity, attrType, (int)changeValue);
        }

        public int GetAttrValue(int characterID, int attrType)
        {
            var target = EntityUtils.GetGameEntity(characterID);
            return target.attribute.value.Get(attrType);
        }
    }
}