namespace WGame.Ability
{
    public enum EventDataType
    {
        [WLable("播放/动画", false)]
        PlayAnim,
        [WLable("播放/特效")]
        PlayEffect,
        [WLable("通知/发送", false)]
        NoticeMessage,
        [WLable("通知/接收")]
        AddMessageReceiver,
        DoAction,
        [WLable("锁定时间状态")]
        LockTick,
        [WLable("对锁定目标/保持距离")]
        FocusKeepDist,
        [WLable("对锁定目标/给目标施加力")]
        FocusDoForce,
        [WLable("对终结技目标/给目标造成伤害")]
        FinishTargetHit,
        [WLable("触发器/输入切换Motion")]
        TriggerInputToMotion,
        [WLable("触发器/状态切换Motion")]
        TriggerStateToMotion,
        [WLable("设置/角色参数")]
        SetOwnerProperty,
        [WLable("设置/角色属性")]
        SetOwnerAttr,
        [WLable("设置/状态")]
        SetState,
        [WLable("设置/时间区域")]
        SetTimeArea,
        [WLable("设置/移动参数")]
        SetMoveParam,
        [WLable("打断", false)]
        Interrupt,
        [WLable("移动/到目标点")]
        MoveToPoint,
    }
}
