## BaseFightCode

waitTarget = 0
def WaitEnter()
{
    waitTarget = E_MAX_HATE_ENTITY
}

def FightEnter(){
    @E_SELF:E_SetDetectParam(360, 20, 20)
}

def FightLogic(){
    S_TickBTree("BaseOnFight")
    if E_MAX_HATE_RANK < HRT_FOCUS{
        @E_SELF:TriggerFSM(fsmName, SD_LOSE_TARGET)
        print("Lose Target", E_MAX_HATE_RANK)
    }
}

def FightEnd(){
}
