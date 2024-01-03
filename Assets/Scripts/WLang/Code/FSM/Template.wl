## Template

-> S1          -- 进入时的状态, 可以放在任意位置，但必须要有
-- 状态列表，基本有如下三种表达方式，用Sn或别的定义好的状态名标注。
-- FSM相关定义都在StateDefine中，具体定义见ConstDefine, FSM文件内定义名用简称，(即原本为SD_DEFINE_NAME变为defineName)
STATE{
    S1 : OnEnterMethodRef, OnUpdateMethodRef, nil           -- onEnter，onUpdate, onEnd三种方法的表达式
    S2                                                      -- 不定义的即为空方法
    S3 -> OtherFSM                                          -- 可以定义为另外的状态机
}

-- 触发转换类型, Any关键字表示任意状态
TRIGGER{
    S1 -> S2 : TriggerNameID, ConditionMethod               -- 分别为触发事件名，转换条件
    S2 -> S1 : T1                                           -- 用T1表示匿名触发事件
    Any -> S3 : TriggerNameID                               -- 表示任意状态转变
}
-- 延迟转换类型
TRIGGER_TIME{
    S1 -> S2 : number
    S2 -> S3 : 1, Condition                                 -- 分别为上一状态持续时间和转换条件
}
-- 根据条件转换, 条件满足就转换
CONDITION{
    Any -> S1 : ConditionMethod                             
}