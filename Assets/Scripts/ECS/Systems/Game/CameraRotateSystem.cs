using Entitas;
using UnityEngine;
using WGame.Attribute;
using WGame.Runtime;
using WGame.UI;

public class CameraRotateSystem : IInitializeSystem, IExecuteSystem
{
    private readonly GameContext _gameContext;
    private readonly SettingContext _settingContext;
    private readonly InputContext _inputContext;
    private readonly ITimeService _timeService;

    private readonly Transform _cameraTransform;
    private readonly Transform _cameraPivotTransform;
    private readonly Transform _camera;

    private float _lookAngle;
    private float _pivotAngle;
    private float _minPivotAngle;
    private float _maxPivotAngle;

    private readonly IGroup<GameEntity> _cameraGroup;

    private float lastOffsetY;

    public CameraRotateSystem(Contexts contexts)
    {
        _gameContext = contexts.game;
        _settingContext = contexts.setting;
        _inputContext = contexts.input;

        _timeService = contexts.meta.timeService.instance;
        _cameraTransform = contexts.meta.mainCameraService.service.Root;
        _cameraPivotTransform = contexts.meta.mainCameraService.service.Pivot;
        _camera = contexts.meta.mainCameraService.service.Camera;
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
        foreach (var entity in _cameraGroup)
        {
            Vector2 look = _inputContext.lookInput.value;
            MainModel.Inst.IsFocus = entity.hasFocus;
            if (entity.hasFocus)
            {
                if (entity.focus.target == null || !entity.focus.target.gameObject.activeInHierarchy)
                {
                    entity.ReplaceActionFocus(FocusType.Switch, 8f);
                    _lookAngle = _cameraTransform.eulerAngles.y;
                    _pivotAngle = _cameraPivotTransform.localEulerAngles.x;
                }
                else
                {
                    // Contexts.sharedInstance.meta.mainCameraService.service.Camera.LookAt(entity.focus.target);
                    var tarPos = entity.focus.target.position;
                    var distSqr = (tarPos - _cameraTransform.position).sqrMagnitude;
                    if (distSqr <= 0)
                        distSqr = 0.1f;
                    var focusPos = tarPos;
                    var rate = 0.3f;
                    float offsetY;
                    if (distSqr > 1)
                    {
                        offsetY = Mathf.Sqrt(distSqr) * rate;
                        focusPos.y -= offsetY;
                        lastOffsetY = focusPos.y;
                    }
                    else
                    {
                        focusPos.y = lastOffsetY + (1 - Mathf.Sqrt(distSqr));
                        if (focusPos.y > 0)
                            focusPos.y = 0;
                    }

                    MainModel.Inst.FocusPosition = tarPos;
                    _cameraTransform.LookAt(focusPos);
                    var tmpRot = _cameraTransform.eulerAngles;
                    // WLogger.Info(tmpRot);
                    _lookAngle = tmpRot.y;
                    _pivotAngle = tmpRot.x;
                }
            }
            else if (entity.isActionLookFwd)
            {
                _lookAngle = entity.gameViewService.service.Model.eulerAngles.y;
                entity.isActionLookFwd = false;
            }
            else
            {
                _lookAngle += (look.x * _gameContext.cameraLookSpeed.value);
                _pivotAngle += (look.y * _gameContext.cameraPivotSpeed.value);
            }
            
            _pivotAngle = Mathf.Clamp(_pivotAngle, _minPivotAngle, _maxPivotAngle);

            Vector3 rotation = Vector2.zero;
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
}
