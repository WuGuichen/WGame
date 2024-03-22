using WGame.Runtime;

namespace WGame.UI
{
    public class NetPlayerInfoModel : Singleton<NetPlayerInfoModel>
    {
        private int _curSelectCharacterIndex = 0;

        public int CurSelectCharacterIndex => _curSelectCharacterIndex;

        public int CurCharacterID
        {
            get
            {
                if (TryGetCharacterID(_curSelectCharacterIndex, out var charId))
                {
                    return charId;
                }

                return -1;
            }
        }

        public bool TryGetCharacterData(int charId, out BaseData.CharacterData charData)
        {
            charData = GameData.Tables.TbCharacter.Get(charId);
            return charData != null;
        }

        public bool TryGetCharacterID(int idx, out int charId)
        {
            var ids = MainDefine.Inst.selectablePlayerIDs;
            if (idx < 0 || idx >= ids.Length)
            {
                WLogger.Error("当前没有选择的角色");
                charId = -1;
                return false;
            }

            charId = ids[idx];
            return true;
        }

        public void SelectPlayer(int idx)
        {
            _curSelectCharacterIndex = idx;
        }
    }
}