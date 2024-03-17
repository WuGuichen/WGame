namespace WGame.Ability.Editor.Custom
{
    public interface StringToIDDefine
    {
        public abstract string[] StringArray { get; }
        public abstract int[] IDArray { get; }

        public static NoticeDefine Notice = new NoticeDefine();
        public static CustomDefine Attribute = new CustomDefine(typeof(WGame.Attribute.WAttrType));
        
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
}