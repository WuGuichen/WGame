## BaseOnFight

SELECTOR{
    DO{
        if E_MAX_HATE_ENTITY >= 0 {
            distMin = 100
            isReach = S_DoMoveToEntity(E_MAX_HATE_ENTITY, distMin)
            if(isReach)
            {
                -- 到达攻击范围内
                print("Attack")
--                @E_SELF:Signal(SIG_DEFENSE, 100)
                @E_SELF:Signal(SIG_ATTACK, 20)
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
    WAIT_TIME 3
}