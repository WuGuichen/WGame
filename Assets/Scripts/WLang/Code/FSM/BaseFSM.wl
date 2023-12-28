## BaseFSM

CODE{
    import BaseFSMCode
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
