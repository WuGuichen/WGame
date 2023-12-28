## BaseFSM

// CODE中的代码以Entity为作用域生效（不同的实体可以用同一个变量名获取不同的值）
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
