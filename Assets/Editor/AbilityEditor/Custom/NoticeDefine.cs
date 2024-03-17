namespace WGame.Ability.Editor.Custom
{
    public class NoticeDefine : StringToIDDefine
    {
        private readonly string[] stringArray = {
            "受击/闪避中",
            "生成/技能实体"
        };
        
        private readonly int[] idArray = {
            NoticeDB.OnStepBeHit,
            NoticeDB.OnUseAbility,
        };

        public string[] StringArray => stringArray;
        public int[] IDArray => idArray;
    }
}
