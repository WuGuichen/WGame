using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputService
{
    Vector2 Move { get; }
    Vector2 Look { get; }
    bool InteractWasPressed { get; }
    bool AttackWasPressed { get; }
    bool AttackIsPressed { get; }
    bool AttackIsHolding { get; }
    bool AttackWasReleased { get; }
    bool JumpWasPressed { get; }
    bool JumpIsHolding { get; }
    bool JumpWasReleased { get; }
    bool StepWasPressed { get; }
    bool GetPriorityInput { get; }
    bool FocusWasPressed { get; }
    bool DefensePressing { get; }
    void Dispose();
    void CleanUp();
}
