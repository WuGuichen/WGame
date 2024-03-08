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
    bool FocusWasPressed { get; }
    bool DefensePressing { get; }
    bool SpecialWasPressed { get; }
    void Dispose();
    void CleanUp();
}
