## BaseOnFight

SELECTOR{
    DO{
        if waitTarget {
            distMin = 200
            isReach = S_DoMoveToEntity(waitTarget, distMin)
            if(isReach)
            {
                -- 到达攻击范围内
                print("Attack")
                @E_SELF:Signal(SIG_DEFENSE, 100)
                return FAIL
            }
        }
        else{
            @E_SELF:TriggerFSM(fsmName, SD_LOSE_TARGET)
            print("no WaitTarget")
        }
        return SUCCESS
    }
    -- 到达巡逻点后停止一段时间
    WAIT_TIME 2
}