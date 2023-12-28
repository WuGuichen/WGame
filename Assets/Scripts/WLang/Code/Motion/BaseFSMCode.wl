## BaseFSMCode

def PatrolEnter(){
    Print("Enter Patrol")
    SetMoveSpeedRate(10)
}
def PatrolLogic(){
//    TickBTree("Test")
    isReached = DoMoveToPatrolPoint()
    if isReached {
        SetNewPatrolIndex()
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
}
def ChaseEnd(){
    Print("End Chase")
}
