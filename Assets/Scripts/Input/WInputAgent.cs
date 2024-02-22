using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WGame.Input
{
    public abstract class WInputAgent
    {
        private InputAction _action;
        private bool _isEnable;
        private InputActionReference _inputActionReference;
        private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;
        protected abstract string SaveKeyName { get;}
        public abstract InputActionAsset InputAsset { get; }
        private string[] _inputTypeList;
        private List<InputAction[]> _inputActions;

        public string[] InputTypeList
        {
            get
            {
                if (_inputTypeList == null)
                {
                    _inputTypeList = InputAsset.actionMaps.Select(c => c.name).ToArray();
                }

                return _inputTypeList;
            }
        }

        public List<InputAction[]> InputActions{
            get
            {
                if (_inputActions == null)
                {
                    _inputActions = InputAsset.actionMaps.Select(c => c.actions.ToArray()).ToList();
                }

                return _inputActions;
            }
        }


        private string _curMapName;
        private string _curTypeName;

        public void RebindInputSetting(string keyName)
        {
            var action = InputAsset.FindAction(keyName);
            RebindInputSetting(action);
        }
        public void RebindInputSetting(int typeIndex, int actionIndex)
        {
            if (InputActions.Count < typeIndex)
            {
                if (InputActions[typeIndex].Length < actionIndex)
                {
                    RebindInputSetting(InputActions[typeIndex][actionIndex]);
                }
                else
                {
                    WLogger.Error("没有相应的按键类型: " + actionIndex + ",最大：" + _inputActions[typeIndex].Length);
                }
            }
            else
            {
                WLogger.Error("没有相应的输入类型: " + typeIndex + " 最大：" + InputActions.Count);
            }
        }
        
        public void RebindInputSetting(InputAction action, bool ignoreMouse = true)
        {
            WLogger.Print("开始重载");
            if (_action != null)
            {
                WLogger.Error("有未完成的重载，本次操作无效");
                return;
            }
            _action = action;
            if (_action == null)
            {
                WLogger.Error("没有传入要修改的键位");
                return;
            }
            _isEnable = _action.actionMap.enabled;
            if (_isEnable)
            {
                _action.actionMap.Disable();
            }
            _inputActionReference = InputActionReference.Create(_action);
            if (_inputActionReference == null)
            {
                WLogger.Error("没有传入要修改的键位引用");
                return;
            }
            _rebindingOperation?.Cancel();
            var index = _action.GetBindingIndex();
            WLogger.Print("Index:" + index);
            for (var i = 0; i < _action.bindings.Count; i++)
            {
            }
            if (index >= _action.bindings.Count)
            {
                WLogger.Print("超出键位限制，本次更改无效:" + _action.bindings.Count);
                CleanUpOperation();
                return;
            }
            _rebindingOperation = _inputActionReference.action.PerformInteractiveRebinding(index)
                .OnMatchWaitForAnother(0.1f)
                .OnCancel(operation =>
                {
                    WLogger.Print("Cancel:" + _action.GetBindingDisplayString());
                    CleanUpOperation();
                } )
                .OnComplete(OnRebindComplete);
            if (ignoreMouse)
            {
                _rebindingOperation.WithControlsExcluding("Mouse");
            }

            _rebindingOperation.Start();
        }

        private void OnRebindComplete(InputActionRebindingExtensions.RebindingOperation operation)
        {
            WLogger.Print("Compeleted:" + _action.GetBindingDisplayString());
            if (_isEnable)
            {
                _action.actionMap.Enable();
            }
            CleanUpOperation();
        }

        private void CleanUpOperation()
        {
            _rebindingOperation?.Dispose();
            _rebindingOperation = null;
            _action = null;
        }

        public void ResetBindData(InputAction action)
        {
            action.RemoveAllBindingOverrides();
        }

        public void Initialize()
        {
            var data = PlayerPrefs.GetString(SaveKeyName, "");
            if (!string.IsNullOrEmpty(data))
            {
                InputAsset.LoadBindingOverridesFromJson(data);
                WLogger.Print("LoadKey" +SaveKeyName);
            }
        }

        private void SaveInput()
        {
            var data = InputAsset.SaveBindingOverridesAsJson();
            WLogger.Print("Save! " + SaveKeyName);
            PlayerPrefs.SetString(SaveKeyName, data);
        }

        public void Destroy()
        {
            SaveInput();
        }
    }
}
