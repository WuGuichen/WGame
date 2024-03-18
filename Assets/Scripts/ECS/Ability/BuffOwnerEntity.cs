using System.Collections.Generic;

namespace WGame.Ability
{
    public class BuffOwnerEntity : BuffOwner
    {
        private GameEntity _entity;
        public BuffOwnerEntity(GameEntity entity)
        {
            _entity = entity;
        }
        public void AddBuff(List<string> buffList)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveBuff(List<string> buffList)
        {
            throw new System.NotImplementedException();
        }
    }
}