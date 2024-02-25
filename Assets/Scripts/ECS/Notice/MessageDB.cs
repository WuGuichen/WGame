using WGame.Notice;

public class MessageDB
{
    public const int BeHittedID = 0;
    public const int CastSkillID = 1;
    
    public class Define
    {
        public struct BeHitted : IMessage
        {
            public int TypeId => BeHittedID;
        }
        
        public struct CastSkill : IMessage
        {
            public int TypeId => CastSkillID;
            public int SkillID { get; set; }
        }
    }
    
    public class Getter
    {
        public static Define.BeHitted GetBehitted()
            => new Define.BeHitted();

        public static Define.CastSkill GetCastSkill(int id)
            => new Define.CastSkill()
            {
                SkillID = id
            };
    }
}
