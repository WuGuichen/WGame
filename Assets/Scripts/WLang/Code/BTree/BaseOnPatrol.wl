## BaseOnPatrol

SELECTOR{
    DO{
        isReached = DoMoveToPatrolPoint()
        if isReached {
            SetNewPatrolIndex()
            return FAIL
        }
        return SUCCESS
    }
    WAIT 200
}