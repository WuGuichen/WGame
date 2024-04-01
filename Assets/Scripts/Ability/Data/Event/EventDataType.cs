namespace WGame.Ability
{
    public enum EventDataType
    {
        [WLable("播放/动画", false)]
        PlayAnim,
        [WLable("播放/特效")]
        PlayEffect,
        NoticeMessage,
        DoAction,
        [WLable("锁定时间状态")]
        LockTick,
        [WLable("触发器/输入/切换Motion")]
        TriggerInputToMotion,
        [WLable("触发器/输入/切换Ability")]
        TriggerInputToAbility,
        [WLable("触发器/状态/切换Motion")]
        TriggerStateToMotion,
        [WLable("触发器/状态/切换Ability")]
        TriggerStateToAbility,
        [WLable("设置/状态")]
        SetState,
        [WLable("设置/移动参数")]
        SetMoveParam,
        [WLable("打断", false)]
        Interrupt,
        [WLable("移动/到目标点")]
        MoveToPoint,
    }
}
