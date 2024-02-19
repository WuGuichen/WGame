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
    // private bool jumpIsTap = false;
    public bool JumpIsHolding => jumpIsHold;
    public bool JumpWasReleased => Jump.WasReleasedThisFrame();

    public bool StepWasPressed => _input.GamePlay.Step.WasPressedThisFrame();

    public bool FocusWasPressed => _input.GamePlay.Focus.WasPressedThisFrame();
    private bool defensePressing = false;
    public bool DefensePressing => defensePressing;


    public InputServiceImplementation()
    {
        var actions = SettingDefine.Inst.InputActions;
        if (_input == null)
        {
            _input = new MyController();
            Attack = actions[SettingDefine.attack];
            Jump = actions[SettingDefine.jump];
            MoveAxis = actions[SettingDefine.move];
            LookAxis = actions[SettingDefine.look];
            Defense = actions[SettingDefine.defense];
            Interact = actions[SettingDefine.interact];
            // Attack = _input.GamePlay.Attack;
            // Jump = _input.GamePlay.Jump;
            // Defense = _input.GamePlay.Defense;
        }
        // Jump.LoadBindingOverridesFromJson();
        // MoveAxis = new InputAction();
        // MoveAxis.AddCompositeBinding("2DVector")
        //     .With("Up", "<Keyboard>/w")
        //     .With("Down", "<Keyboard>/s")
        //     .With("Left", "<Keyboard>/a")
        //     .With("Right", "<Keyboard>/d");
        // MoveAxis.Enable();

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
