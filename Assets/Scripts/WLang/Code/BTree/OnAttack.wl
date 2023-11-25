## OnAttack

    SELECTOR{
        DO{
            hasTarget = HasDetectedCharacter()
            if MoveToTarget(2){
                if hasTarget{
                    @E_SELF:Signal(SIG_ATTACK, 0.2)
                    return FAIL
                }
                else{
                    Print("Lose")
                    TriggerFSM(SD_LOSE_TARGET)
                }
                return FAIL
            }
            if hasTarget == false{
                Print("Lose")
                TriggerFSM(SD_LOSE_TARGET)
            }
            return SUCCESS
        }
        WAIT 200
    }