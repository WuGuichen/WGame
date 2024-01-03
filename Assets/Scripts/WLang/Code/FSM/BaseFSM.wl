## BaseFSM

-- CODE中的代码以Entity为作用域生效（不同的实体可以用同一个变量名获取不同的值
-- , 而同一个实体的同一状态机和行为树的CODE作用域相同）
CODE{
    fsmName = "BaseFSM"
    import BaseFSMCode
}

-> Patrol

-- 下面的名字都是处理过的缩写,在外部使用要用常数名，即Patrol = SD_PATROL, LoseTarget = SD_LOSE_TARGET
STATE{
    Patrol : PatrolEnter, PatrolLogic, PatrolEnd
    Chase : ChaseEnter, ChaseLogic, ChaseEnd
    Fight -> BaseFight
}

-- 事件触发转换：当前事件 -> 下一事件 : 触发事件名，条件函数(可省)
TRIGGER{
    Patrol -> Chase : SpottedTarget
    Fight -> Patrol : LoseTarget
    Chase -> Fight : ReachTarget
    Chase -> Patrol : LoseTarget
}

TRIGGER_TIME{
--    Patrol -> Chase : 2
--    Chase -> Fight : 1
}
