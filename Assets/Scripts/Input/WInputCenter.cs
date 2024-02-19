using UnityEngine;
using WGame.Runtime;
using UnityEngine.InputSystem;

namespace WGame.Input
{
    public class WInputCenter : Singleton<WInputCenter>
    {
        private WInput _input;
        private InputAction _action;
        public InputActionReference _inputActionReference;
        public InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

        public override void InitInstance()
        {
            _input = new WInput();
            var action = _input.Player.Fire;
            _action = action;
            InputActionReference actionReference = InputActionReference.Create(action);
            _inputActionReference = actionReference;
            _inputActionReference.action.started += OnFire;
            _input.Player.Enable();
            TickManager.Inst.AddTick(OnUpdate);
        }

        private void OnFire(InputAction.CallbackContext context)
        {
            WLogger.Print("Fire:" + _inputActionReference.action.GetBindingDisplayString());
        }

        public void OnUpdate()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.M))
            {
                RemapPlayerInput();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.C))
            {
                ResetBindData();
            }
        }
        
        public void RemapPlayerInput()
        {
            WLogger.Print("开始重载");
            _input.Player.Disable();
            _rebindingOperation?.Cancel();
            _rebindingOperation = _inputActionReference.action.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .OnMatchWaitForAnother(0.1f)
                .OnCancel(operation =>
                {
                    WLogger.Print("Cancel:" + _action.GetBindingDisplayString());
                } )
                .OnComplete(OnRebindComplete);

            _rebindingOperation.Start();
        }

        private void OnRebindComplete(InputActionRebindingExtensions.RebindingOperation operation)
        {
            CleanUpOperation();
            WLogger.Print("Compeleted:" + _action.GetBindingDisplayString());
            _input.Player.Enable();
        }

        private void CleanUpOperation()
        {
            _rebindingOperation?.Dispose();
            _rebindingOperation = null;
        }

        private void ResetBindData()
        {
            _action.RemoveAllBindingOverrides();
        }
    }
}
