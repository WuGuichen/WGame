using System;
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
        private int _bindingIndex;
        private InputBinding _oldInputBinding;
        private Action _onRebindCompeleted;
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
        
        public void RebindInputSetting(InputAction action, Action onCompleted = null, bool ignoreMouse = true, int bindIndex = 0)
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
            var index = bindIndex;
            _oldInputBinding = _action.bindings[index];
            _bindingIndex = index;
            _onRebindCompeleted = onCompleted;
            if (_action.bindings[index] == null || _action.bindings[index].isComposite)
            {
                WLogger.Print("暂时没有支持这种键位更改, 本次操作无效");
                CleanUpOperation();
                return;
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
            _rebindingOperation.WithCancelingThrough("<Keyboard>/escape");

            _rebindingOperation.Start();
        }

        public void CancelRebinding()
        {
            _rebindingOperation?.Cancel();
        }

        private void OnRebindComplete(InputActionRebindingExtensions.RebindingOperation operation)
        {
            WLogger.Print("Compeleted:" + _action.GetBindingDisplayString());

            ResolveConflict(_action, _bindingIndex, _oldInputBinding);
            
            if (_isEnable)
            {
                _action.actionMap.Enable();
            }

            CleanUpOperation();
        }

        // 方法参数很冗余，但问题不大
        private void ResolveConflict(InputAction action, int index, InputBinding oldInputBinding)
        {
            _bindingIndex = index;
            _oldInputBinding = oldInputBinding;
            _action = action;
            var newInputBinding = _action.bindings[_bindingIndex];
            foreach (var binding in _action.actionMap.bindings)
            {
                if (binding.action == newInputBinding.action)
                {
                    continue;
                }

                if (binding.effectivePath == newInputBinding.effectivePath)
                {
                    var oldAction = _action.actionMap.FindAction(binding.action);
                    
                    oldAction.ApplyBindingOverride(_bindingIndex, _oldInputBinding.effectivePath);
                }
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void CleanUpOperation()
        {
            _rebindingOperation?.Dispose();
            _rebindingOperation = null;
            _inputActionReference = null;
            _oldInputBinding = new InputBinding(null);
            _bindingIndex = 0;
            _action = null;
            _onRebindCompeleted?.Invoke();
            _onRebindCompeleted = null;
            _isEnable = false;
        }

        public void ResetBindData(InputAction action, int bindingIndex = 0)
        {
            var oldInputBinding = action.bindings[bindingIndex];
            action.RemoveBindingOverride(bindingIndex);
            _isEnable = action.actionMap.enabled;
            if (_isEnable)
            {
                action.actionMap.Disable();
            }
            ResolveConflict(action, bindingIndex, oldInputBinding);
            if (_isEnable)
            {
                action.actionMap.Enable();
            }
            CleanUpOperation();
            WLogger.Print("重置成功：" + action.GetBindingDisplayString(bindingIndex));
        }

        public void Initialize()
        {
            var data = PlayerPrefs.GetString(SaveKeyName, "");
            if (!string.IsNullOrEmpty(data))
            {
                InputAsset.LoadBindingOverridesFromJson(data);
                WLogger.Print("LoadKey:" +SaveKeyName);
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
