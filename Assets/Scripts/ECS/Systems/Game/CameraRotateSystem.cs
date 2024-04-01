using Entitas;
using UnityEngine;
using WGame.UI;

public class CameraRotateSystem : IInitializeSystem, IExecuteSystem
{
    private readonly GameContext _gameContext;
    private readonly SettingContext _settingContext;
    private readonly InputContext _inputContext;
    private readonly ITimeService _timeService;

    private readonly Transform _cameraTransform;
    private readonly Transform _cameraPivotTransform;
    private readonly ICameraService _cameraService;

    private float _lookAngle;
    private float _currentLookAngle;
    private float _pivotAngle;
    private bool _needRefreshAngle = false;
    private float _minPivotAngle;
    private float _maxPivotAngle;

    private readonly IGroup<GameEntity> _cameraGroup;

    public CameraRotateSystem(Contexts contexts)
    {
        _gameContext = contexts.game;
        _settingContext = contexts.setting;
        _inputContext = contexts.input;

        _timeService = contexts.meta.timeService.instance;
        _cameraTransform = contexts.meta.mainCameraService.service.Root;
        _cameraPivotTransform = contexts.meta.mainCameraService.service.Pivot;
        _cameraService = contexts.meta.mainCameraService.service;
        _cameraGroup = _gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.GameViewService, GameMatcher.Camera));
    }

    public void Initialize()
    {
        GameSetting.CameraConfigs configs = _settingContext.gameSetting.value.CameraConfig;
        _minPivotAngle = configs.MinimumPivotAngle;
        _maxPivotAngle = configs.MaximumPivotAngle;
        _gameContext.ReplaceCameraPivotSpeed(configs.CameraPivotSpeed);
        _gameContext.ReplaceCameraLookSpeed(configs.CameraLookSpeed);
    }

    public void Execute()
    {
        // var deltaTime = _timeService.DeltaTime(1f);
        var deltaTime = _timeService.FixedDeltaTime; 
        if (_cameraService.IsAutoControl)
        {
            _needRefreshAngle = true;
        }
        else
        {
            if (_needRefreshAngle)
            {
                _pivotAngle = _cameraPivotTransform.localEulerAngles.x;
                if (_pivotAngle > _maxPivotAngle || _pivotAngle < _minPivotAngle)
                {
                    if (_pivotAngle > 0)
                    {
                        _pivotAngle -= 360;
                    }
                    else if (_pivotAngle < 0)
                    {
                        _pivotAngle += 360;
                    }
                }
                _lookAngle = _cameraTransform.eulerAngles.y;
                
                _needRefreshAngle = false;
            }

            foreach (var entity in _cameraGroup)
            {
                Vector2 look = _inputContext.lookInput.value;
                MainModel.Inst.IsFocus = entity.hasFocusEntity;
                if (MainModel.Inst.IsFocus)
                {
                    if (entity.focusEntity.entity.isDeadState)
                    {
                        entity.ReplaceActionFocus(FocusType.Switch, 18f);
                        _lookAngle = _cameraTransform.eulerAngles.y;
                        _pivotAngle = _cameraPivotTransform.localEulerAngles.x;
                    }
                    else
                    {
                        // Contexts.sharedInstance.meta.mainCameraService.service.Camera.LookAt(entity.focus.target);
                        var tarPos = entity.focusEntity.entity.gameViewService.service.FocusPoint;
                        var distSqr = (tarPos - _cameraTransform.position).sqrMagnitude;
                        if (distSqr <= 0)
                            distSqr = 0.1f;
                        var focusPos = tarPos;
                        var rate = 0.3f;

                        var offsetY = Mathf.Sqrt(distSqr) * rate;
                        focusPos.y -= offsetY;
                        if (focusPos.y > 0)
                            focusPos.y = 0;
                        // lastOffsetY = focusPos.y;


                        MainModel.Inst.FocusPosition = tarPos;
                        _cameraTransform.LookAt(focusPos);
                        var tmpRot = _cameraTransform.eulerAngles;
                        // WLogger.Info(tmpRot);
                        _lookAngle = Mathf.LerpAngle(_lookAngle, tmpRot.y, _gameContext.cameraLookSpeed.value*deltaTime);
                        _pivotAngle = tmpRot.x;
                    }
                }
                else if (entity.isActionLookFwd)
                {
                    _lookAngle = entity.gameViewService.service.Model.eulerAngles.y;
                    _pivotAngle = 8f;
                    entity.isActionLookFwd = false;
                }
                else
                {
                    _lookAngle += (look.x * _gameContext.cameraLookSpeed.value*deltaTime);
                    _pivotAngle += (look.y * _gameContext.cameraPivotSpeed.value*deltaTime);
                }

                _pivotAngle = Mathf.Clamp(_pivotAngle, _minPivotAngle, _maxPivotAngle);

                Vector3 rotation = Vector3.zero;
                rotation.y = _lookAngle;
                var tarRot = Quaternion.Euler(rotation);
                _cameraTransform.rotation = tarRot;
                entity.ReplaceActionCameraRotate(tarRot);

                rotation = Vector3.zero;
                rotation.x = _pivotAngle;
                tarRot = Quaternion.Euler(rotation);
                _cameraPivotTransform.localRotation = tarRot;
            }
        }

        _cameraService.Process(deltaTime);
    }
}
