namespace WGame.Ability.Editor.Custom
{
    public abstract class StringToIDDefine
    {
        public abstract string[] StringArray { get; }
        public abstract int[] IDArray { get; }

        public int GetIndex(int id)
        {
            for (var i = 0; i < IDArray.Length; i++)
            {
                if (IDArray[i] == id)
                {
                    return i;
                }
            }

            return -1;
        }
    }
    public class NoticeDefine : StringToIDDefine
    {
        private static readonly string[] stringArray = {
            "受击/闪避中",
            "受击/防御中"
        };
        
        private static readonly int[] idArray = {
            NoticeDB.OnStepBeHit,
            2,
        };

        public override string[] StringArray => stringArray;
        public override int[] IDArray => idArray;
    }
}
