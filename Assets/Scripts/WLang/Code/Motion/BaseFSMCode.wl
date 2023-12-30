## BaseFSMCode

def PatrolEnter(){
    Print("Enter Patrol")
    SetMoveSpeedRate(5)
}

def PatrolLogic(){
    TickBTree("BaseOnPatrol")
    if E_MAX_HATE_RANK >= HRT_DETECT{
       print(fsmName)
        @E_SELF:ChangeFSMState(fsmName, SD_CHASE)
    }
}
def PatrolEnd(){
    Print("End Patrol")
    SetMoveSpeedRate(100)
}

def ChaseEnter(){
    Print("Enter Chase")
}
def ChaseLogic(){
    pos = @E_MAX_HATE_ENTITY:GetPosition()
    isReach = DoMoveToEntity(E_MAX_HATE_ENTITY, 30)
    if isReach{
        print("reach")
        @E_SELF:ChangeFSMState(fsmName, SD_FIGHT)
    }
    else{
        print("moving")
    }
}
def ChaseEnd(){
    Print("End Chase")
}
