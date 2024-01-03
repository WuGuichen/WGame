## BaseOnFight

SELECTOR{
    DO{
        isReach = waitTarget and DoMoveToEntity(waitTarget, 100) or false
        if(isReach)
        {
            -- 到达攻击范围内
            print("Attack")
            Signal(E_SELF, SIG_ATTACK, 0.2)
            return FAIL
        }
        return SUCCESS
    }
    -- 到达巡逻点后停止一段时间
    WAIT_TIME 2
}