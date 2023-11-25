## OnPatrol

    SELECTOR{
        DO{
            if MoveToTarget(){
                SetPatrolPointTarget()
                return FAIL
            }
            return SUCCESS
        }
        WAIT 200
    }