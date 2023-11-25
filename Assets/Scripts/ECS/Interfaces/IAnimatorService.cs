using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimatorService
{
    public float stateTime { get; protected set; }
    void SetForwardParam(float value);
    void SetRightParam(float value);
    void SetSpeed(float value);

    void SetAnimatorController(AnimatorOverrideController type);

    void CleanUpEndState();

    void SwitchAnimState();
    
    GameEntity LinkEntity { get; }
}
