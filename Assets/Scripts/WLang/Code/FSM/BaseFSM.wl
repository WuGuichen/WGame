## BaseFSM

CODE{
    def PatrolEnter(){
        Print("Enter Patrol")
    }
    def PatrolLogic(){
        TickBTree("Test")
    }
    def PatrolEnd(){
        Print("End Patrol")
    }
    
    def ChaseEnter(){
        Print("Enter Chase")
    }
    def ChaseLogic(){
    }
    def ChaseEnd(){
        Print("End Chase")
    }
}

-> Patrol

STATE{
    Patrol : PatrolEnter, PatrolLogic, PatrolEnd
    Chase : ChaseEnter, ChaseLogic, ChaseEnd
    Fight -> BaseFight
}

TRIGGER{
    Patrol -> Chase : SpottedTarget
    Fight -> Patrol : LoseTarget
    Chase -> Fight : ReachTarget
}

TRIGGER_TIME{
    Patrol -> Chase : 2
    Chase -> Fight : 1
}
