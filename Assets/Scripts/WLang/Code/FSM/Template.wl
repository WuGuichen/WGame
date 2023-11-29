## Template

-> S1
STATE{
    S1 : OnEnterMethodRef, OnUpdateMethodRef, nil
    S2
    S3 : OtherFSM
}

TRIGGER{
    S1 -> S2 : TriggerNameID, ConditionMethod
    S2 -> S1 : T1
    Any -> S3 : TriggerNameID
}
TRIGGER_TIME{
    S1 -> S2 : number
}
CONDITION{
    Any -> S1 : ConditionMethod
}