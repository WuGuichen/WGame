## BaseFightCode

waitTarget = 0
def WaitEnter()
{
    Print("Wait")
    waitTarget = E_MAX_HATE_ENTITY
    print(E_MAX_HATE_ENTITY)
}

def FightEnter(){
    @E_SELF:E_SetDetectParam(360, 20, 20)
}

def FightLogic(){
    S_TickBTree("BaseOnFight")
    if E_MAX_HATE_RANK < HRT_FOCUS{
        @E_SELF:TriggerFSM(fsmName, SD_LOSE_TARGET)
    }
}

def FightEnd(){
}
