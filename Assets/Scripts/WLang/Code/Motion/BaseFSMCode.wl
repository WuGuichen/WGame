## BaseFSMCode
rankChase = HRT_DETECT

def PatrolEnter(){
    -- 设置巡逻移速
    S_SetMoveSpeedRate(50)
    @E_SELF:E_ResetDetectParam()
--    print("Reset")
    -- 取消锁定
--    @E_SELF:E_DoFocusTarget()
}

def PatrolLogic(){
    -- 巡逻行为树
    S_TickBTree("BaseOnPatrol")
    -- 巡逻中仇恨等级达到一定级别后
    if E_MAX_HATE_RANK >= rankChase{
--         切换到追击状态
        @E_SELF:ChangeFSMState(fsmName, SD_CHASE)
    }
}
def PatrolEnd(){
    -- 重置移速
    S_SetMoveSpeedRate()
}

def ChaseEnter(){
    -- 设置追击移速
    S_SetMoveSpeedRate(50)
    S_SetRotateSpeedRate(200)
    @E_SELF:E_SetDetectParam(360, 20, 20)
    @E_SELF:E_DoFocusTarget(E_MAX_HATE_ENTITY)
}
def ChaseLogic(){
    -- 向最高仇恨值目标移动, 30cm内表示到达
    isReach = S_DoMoveToEntity(E_MAX_HATE_ENTITY, 30)
    if isReach{
        -- 到达目标后触发到达目标事件
        @E_SELF:TriggerFSM(fsmName, SD_REACH_TARGET)
    }

    if E_MAX_HATE_RANK < rankChase{
        -- 目标仇恨低时触发丢失目标事件
        @E_SELF:TriggerFSM(fsmName, SD_LOSE_TARGET)
    }
}
def ChaseEnd(){
--    Print("End Chase")
    -- 重置移速
    S_SetMoveSpeedRate()
}
