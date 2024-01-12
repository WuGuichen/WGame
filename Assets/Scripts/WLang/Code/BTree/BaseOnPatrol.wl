## BaseOnPatrol

SELECTOR{
    DO{
        -- 移动到选中巡逻点
        isReached = S_DoMoveToPatrolPoint()
        if isReached {
            -- 巡逻点已到达，选中另一个巡逻点, 并触发下一逻辑
            S_SetNewPatrolIndex()
            return FAIL
        }
        -- 巡逻中
        return SUCCESS
    }
    -- 到达巡逻点后停止一段时间
    WAIT_TIME 2
}