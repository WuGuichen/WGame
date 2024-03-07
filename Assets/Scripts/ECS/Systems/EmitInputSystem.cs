using Entitas;
using UnityEngine;
using WGame.UI;

public class EmitInputSystem : IInitializeSystem, IExecuteSystem, ITearDownSystem
{
    private InputContext _inputContext;
    private IInputService _inputService;
    private readonly MetaContext _metaContext;
    private readonly GameContext _gameContext;

    private MainModel model;
    
    public EmitInputSystem(Contexts contexts)
    {
        _inputContext = contexts.input;
        _metaContext = contexts.meta;
        _gameContext = contexts.game;
    }

    public void Initialize()
    {
        _inputService = _metaContext.inputService.instance;
        _inputContext.SetMoveInput(Vector2.zero);
        _inputContext.SetLookInput(Vector2.zero);
        _inputContext.SetAttackInput(false);
        _inputContext.SetJumpInput(false);
        _inputContext.ReplaceStepInput(false); 
        model = MainModel.Inst;
    }

    public void Execute()
    {
        if (WTerminal.isInTerminal || (Cursor.visible && !model.IsUseJoystick))
        {
            _inputContext.ReplaceMoveInput(Vector2.zero);
            _inputContext.ReplaceLookInput(Vector2.zero);
            _inputContext.ReplaceJumpInput(false);
            _inputContext.ReplaceRunInput(false);
            _inputContext.ReplaceAttackInput(false);
            _inputContext.ReplaceStepInput(false);
            _inputContext.ReplaceFocusInput(false);
            _inputContext.ReplaceDefense(false);
            _inputContext.ReplaceAttackHoldInput(false);
            if (MainModel.Inst.IsUseJoystick)
            {
                model.CleanUpInputs();
            }
            else
            {
                _inputService.CleanUp();
            }
        }
        else
        {
            #if UNITY_EDITOR
            if (false)
            #else
            if (MainModel.Inst.IsUseJoystick )
            #endif
            {
                // 用手柄控制
                model.TickInputUpdate(_metaContext.timeService.instance.DeltaTime);
                _inputContext.ReplaceMoveInput(model.MoveDir);
                if (model.isLooking)
                {
                    _inputContext.ReplaceLookInput(model.LookDir);
                }
                else
                {
                    _inputContext.ReplaceLookInput(Vector2.zero);
                }

                _inputContext.ReplaceJumpInput(model.IsTriggerJump);
                _inputContext.ReplaceRunInput(model.IsRunningState);
                _inputContext.ReplaceAttackInput(model.IsTriggerAttack);
                _inputContext.ReplaceAttackHoldInput(model.IsHoldAttack);
                _inputContext.ReplaceStepInput(model.IsTriggerStep);
                _inputContext.ReplaceFocusInput(model.IsTriggerFocus);
                _inputContext.ReplaceDefense(model.isDefencePressing);
                model.CleanUpInputs();
            }
            else
            {
                _inputContext.ReplaceMoveInput(_inputService.Move);
                _inputContext.ReplaceLookInput(_inputService.Look);
                _inputContext.ReplaceJumpInput(_inputService.JumpWasReleased && !_inputService.JumpIsHolding);
                _inputContext.ReplaceRunInput(_inputService.JumpIsHolding);
                _inputContext.ReplaceAttackInput(_inputService.AttackWasPressed);
                _inputContext.ReplaceAttackHoldInput(_inputService.AttackIsHolding);
                _inputContext.ReplaceStepInput(_inputService.StepWasPressed);
                _inputContext.ReplaceFocusInput(_inputService.FocusWasPressed);
                _inputContext.ReplaceDefense(_inputService.DefensePressing);
                _inputContext.ReplaceSpecial(_inputService.SpecialWasPressed);
                if (_inputService.InteractWasPressed)
                {
                    MainModel.Inst.OnInteractTagClick();
                }

                _inputService.CleanUp();
            }
        }
    }

    public void TearDown()
    {
        _inputService.Dispose();
    }
}
