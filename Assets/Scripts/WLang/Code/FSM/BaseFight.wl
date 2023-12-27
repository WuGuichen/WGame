## BaseFight

CODE{
    loseTarget = SD_LOSE_TARGET
    def FightEnter(){
        Print("Enter Fight !!!!!")
    }
    
    def FightLogic(){
        @E_SELF:TriggerFSM(loseTarget)
    }
    
    def FightEnd(){
        Print("End Fight")
    }
}

STATE{
    Wait
    Attack : FightEnter, FightLogic, FightEnd
}

TRIGGER_TIME{
    Wait -> Attack : 2
    Attack -> Wait : 2
}

-> Wait
<- Attack