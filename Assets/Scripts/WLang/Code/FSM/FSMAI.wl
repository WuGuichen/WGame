## FSMAI

-> Patrol
STATE{
    Patrol : OnAIPatrolEnter, OnAIPatrolLogic, OnAIAttackExit
    Chase : OnAIChaseEnter, OnAIChaseLogic, OnAIChaseExit
    Search : ()=>{Print("寻找目标") OnAISearchEnter()}
    Fight -> FSMFight
}

TRIGGER{
    Patrol -> Chase : SpottedTarget
    Fight -> Search : LoseTarget
    Chase -> Fight : ReachTarget
}
TRIGGER_TIME{
    Search -> Patrol : 2
}
CONDITION{
    Chase -> Patrol : NoDetectedCharacter
}