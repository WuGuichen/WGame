## OnAttack_2

SELECTOR{
    // 设置目标检测状态
    DO{
        tarSensorLayer = @E_SELF:GetTargetSensorLayer()
        @E_SELF:SetDetectState(tarSensorLayer, 5, 120)
    }
    // 目标是否丢失
    DO{
        if IsHasTarget(){
            return FAIL
        }
        // 已丢失
        @E_SELF:TriggerFSM(SD_LOSE_TARGET)
        return SUCCESS
    }
    // 目标是否在攻击范围内
    DO{
        // 没看到则继续向目标点探查
        MoveToTarget(2)
        tarSqrDist = GetSqrDist(E_TARGET)
        if tarSqrDist and tarSqrDist < 2{
            // 在范围内, 触发攻击信号
            E_SELF:Signal(SIG_ATTACK, 0.2)
            return FAIL
        }
        // 不在
        return SUCCESS
    }
    // 攻击后随机停顿
    WAIT_TIME 1.5
}