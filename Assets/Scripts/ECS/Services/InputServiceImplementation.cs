using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using WGame.UI;

public class InputServiceImplementation : IInputService
{
    private readonly InputAction MoveAxis;
    private readonly InputAction LookAxis;
    private readonly InputAction Attack;
    private readonly InputAction Jump;
    private readonly InputAction Step;
    private readonly InputAction Defense;
    private readonly InputAction Interact;
    private MyController _input;
    public Vector2 Move => MoveAxis.ReadValue<Vector2>();
    public Vector2 Look => LookAxis.ReadValue<Vector2>();
    public bool InteractWasPressed => Interact.WasPressedThisFrame();
    public bool AttackWasPressed => Attack.WasPressedThisFrame();
    public bool AttackIsPressed => Attack.IsPressed();
    public bool AttackIsHolding => attackIsHold;
    public bool AttackWasReleased => Attack.WasReleasedThisFrame();
    public bool JumpWasPressed => Jump.WasPressedThisFrame();
    private bool jumpIsHold = false;
    private bool jumpReleased = true;
    private bool attackIsHold = false;
    private bool attackReleased = true;
    public bool JumpIsHolding => jumpIsHold;
    public bool JumpWasReleased => jumpReleased;

    public bool StepWasPressed => _input.GamePlay.Step.WasPressedThisFrame();
    public bool StepIsHolding => stepIsHold;
    private bool stepIsHold = false;
    private bool stepReleased = true;

    public bool FocusWasPressed => _input.GamePlay.Focus.WasPressedThisFrame();
    private bool defensePressing = false;
    public bool DefensePressing => defensePressing;
    public bool SpecialWasPressed => _input.GamePlay.Special.WasPressedThisFrame();


    public InputServiceImplementation()
    {
        var actions = SettingDefine.Inst.InputActions;
        if (_input == null)
        {
            _input = new MyController();
            Attack = actions[SettingDefine.attack];
            Jump = actions[SettingDefine.jump];
            Step = actions[SettingDefine.step];
            MoveAxis = actions[SettingDefine.move];
            LookAxis = actions[SettingDefine.look];
            Defense = actions[SettingDefine.defense];
            Interact = actions[SettingDefine.interact];
        }

        Jump.performed += ctx =>
        {
            jumpReleased = false;
            if (ctx.interaction is HoldInteraction)
            {
                jumpIsHold = true;
            }
        };
        Jump.canceled += ctx =>
        {
            jumpReleased = true;
        };

        Step.performed += ctx =>
        {
            stepReleased = false;
            if (ctx.interaction is HoldInteraction)
            {
                stepIsHold = true;
            }
        };

        Step.canceled += ctx =>
        {
            stepReleased = true;
        };

        Attack.performed += ctx =>
        {
            attackReleased = false;
            if (ctx.interaction is HoldInteraction)
            {
                attackIsHold = true;
            }
        };
        
        Attack.canceled += ctx =>
        {
            attackReleased = true;
            attackIsHold = false;
        };

        Defense.started += ctx =>
        {
            defensePressing = true;
        };
        Defense.canceled += ctx =>
        {
            defensePressing = false;
        };
        
        
        _input.Enable();
    }

    public void Dispose()
    {
        _input.Disable();
    }

    public void CleanUp()
    {
        if(jumpReleased)
            jumpIsHold = false;
        if (attackReleased)
            attackReleased = false;
    }
}
