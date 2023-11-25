## FSMFight

STATE{
    Wait
    Attack : OnFightAttackEnter, OnFightAttackLogic, OnFightAttackEnd
}

TRIGGER_TIME{
    Wait -> Attack : 2
    Attack -> Wait : 2
}

-> Wait