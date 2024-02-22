using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using WGame.Input;

public class WInputAgentMyController : WInputAgent, IInputService
{
    private MyController _input = new MyController();
    public MyController Input => _input;
    // private WInput _input = new WInput();
    // public WInput Input => _input;
    protected override string SaveKeyName => "MyControllerBindData";
    public override InputActionAsset InputAsset => _input.asset;
    
    private readonly InputAction MoveAxis;
    private readonly InputAction LookAxis;
    private readonly InputAction Attack;
    private readonly InputAction Jump;
    private readonly InputAction Defense;
    private readonly InputAction Interact;
    
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
    public bool JumpWasReleased => Jump.WasReleasedThisFrame();

    public bool StepWasPressed => _input.GamePlay.Step.WasPressedThisFrame();

    public bool FocusWasPressed => _input.GamePlay.Focus.WasPressedThisFrame();
    private bool defensePressing = false;
    public bool DefensePressing => defensePressing;

    public WInputAgentMyController()
    {
        Attack = _input.GamePlay.Attack;
        Jump = _input.GamePlay.Jump;
        MoveAxis = _input.GamePlay.Move;
        LookAxis = _input.GamePlay.Look;
        Defense = _input.GamePlay.Defense;
        Interact = _input.GamePlay.Interact;
        
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
        Initialize();
    }
    public void Dispose()
    {
        _input.Disable();
        Destroy();
    }

    public void CleanUp()
    {
        if(jumpReleased)
            jumpIsHold = false;
        if (attackReleased)
            attackReleased = false;
    }
}
