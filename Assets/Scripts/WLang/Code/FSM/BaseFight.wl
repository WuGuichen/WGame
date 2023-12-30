## BaseFight

CODE{
    import BaseFightCode
}

STATE{
    Wait : WaitEnter
    Attack : FightEnter, FightLogic, FightEnd
}

TRIGGER_TIME{
    Wait -> Attack : 2
    Attack -> Wait : 2
}

-> Wait
<- Attack