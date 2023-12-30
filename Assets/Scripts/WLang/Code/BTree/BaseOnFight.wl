## BaseOnFight

SELECTOR{
    DO{
        isReach = waitTarget and MoveToEntity(waitTarget) or false
        if(MoveToEntity(waitTarget, 30))
        {
            // 到达攻击范围内
            print("Attack")
            @E_SELF:Signal(SIG_ATTACK, 0.2)
            return FAIL
        }
    }
    // 到达巡逻点后停止一段时间
    WAIT_TIME 2
}