namespace WGame.Ability
{
    public class MessageHitTarget : INoticeMessage
    {
        [EditorData("伤害来源", EditorDataType.Enum)]
        public BuffTargetType TargetType { get; set; }
    }
}