## OnPatrol

    SELECTOR{
        DO{
//            if MoveToTarget(){
//                a = 1
//            }
            if MoveToTarget(){
                SetPatrolPointTarget()
                return FAIL
            }
            return SUCCESS
        }
        WAIT 200
    }