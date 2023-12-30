## BaseFightCode

waitTarget = 0
def WaitEnter()
{
    Print("Wait")
    waitTarget = E_MAX_HATE_ENTITY
    print(E_MAX_HATE_ENTITY)
}

def FightEnter(){
}

def FightLogic(){
    TickBTree("BaseOnFight")
    if E_MAX_HATE_RANK < 2{
        @E_SELF:TriggerFSM(fsmName, SD_LOSE_TARGET)
    }
}

def FightEnd(){
    Print("End Fight...........")
}
