## TestFSM

STATE{
    Wait : OnTestWaitEnter,nil,OnWaitExit
    Attack : OnTestAttackEnter,nil,OnAttackExit
}
TRIGGER_TIME{
    Wait -> Attack : 2
    Attack -> Wait : 2.2
}
TRIGGER{
    Wait -> Attack : SD_SPOTTED_TARGET
}

->Wait
