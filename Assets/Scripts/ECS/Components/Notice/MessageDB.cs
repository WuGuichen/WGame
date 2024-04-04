using Weapon;
using WGame.Ability;

namespace WGame.Notice
{
    public class MessageDB
    {
        public const int BeHittedID = 0;
        public const int CastSkillID = 1;

        public class Define
        {
            public class BeHitted : IMessage
            {
                public ContactInfo hitInfo;
                public int TypeId => BeHittedID;
            }

            public class CastSkill : IMessage
            {
                public int TypeId => CastSkillID;
                public EntityMoveInfo Info { get; set; }
            }
        }

        public class Getter
        {
            public static Define.BeHitted GetBehitted(ContactInfo info)
                => new Define.BeHitted() { hitInfo = info };

            public static Define.CastSkill GetCastSkill(EntityMoveInfo info)
                => new Define.CastSkill()
                {
                    Info = info
                };
        }
    }
}