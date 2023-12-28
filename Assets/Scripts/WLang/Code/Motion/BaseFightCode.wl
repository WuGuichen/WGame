## BaseFightCode

    loseTarget = SD_LOSE_TARGET
    def FightEnter(){
        Print("Enter Fight !!!!!")
    }

    def FightLogic(){
        @E_SELF:TriggerFSM(loseTarget)
    }

    def FightEnd(){
        Print("End Fight...........")
    }
