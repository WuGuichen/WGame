using Entitas;
using UnityEngine;

public class CameraFollowTargetSystem : IExecuteSystem, IInitializeSystem
{
    private readonly Transform _camemraTransform;
    private readonly GameContext _gameContext;
    private readonly SettingContext _settingContext;
    private readonly ITimeService _time;

    private Vector3 cameraFollowVelocity = Vector3.zero;
    private IGameViewService _target;
    private IGroup<GameEntity> _cameraGroup;
    private readonly Transform _camTrans;
    private float curDis;
    private float deadZoomY = 1.2f;
    private float camY = 0f;
    
    public CameraFollowTargetSystem(Contexts contexts)
    {
        _camemraTransform = contexts.meta.mainCameraService.service.Root;
        _gameContext = contexts.game;
        _cameraGroup = _gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.Camera, GameMatcher.GameViewService));
        _settingContext = contexts.setting;
        _camTrans = contexts.meta.mainCameraService.service.Camera;
        _time = contexts.meta.timeService.instance;
    }

    public void Initialize()
    {
        // _target = _gameContext.localPlayerEntity.gameViewService.service;
        _gameContext.ReplaceCameraSmoothTime(_settingContext.gameSetting.value.CameraConfig.CameraSmoothTime);
    }

    public void Execute()
    {
        Vector3 pos = _camemraTransform.position;
        foreach (var entity in _cameraGroup)
        {
            _target = entity.gameViewService.service;
            pos = _target.GetCameraPos();
            pos.y -= deadZoomY;
            if (entity.hasFocusEntity)
            {
                // pos = (pos + entity.focus.target.position) / 2;
                // var param = (entity.focus.target.position - _target.Position);
                // var tarDis = param.magnitude;
                // float dis = Mathf.SmoothDamp(_camTrans.localPosition.z, tarDis, ref curDis, _gameContext.cameraSmoothTime.value);
                // _camTrans.localPosition = new Vector3(0, 0, -dis - 3f);
            }
            else
            {
                // _camTrans.localPosition = new Vector3(0, 0, -2.4f);
            }

            var deltaTime = _time.FixedDeltaTime;
            var tarPos = Vector3.SmoothDamp(_camemraTransform.position,
                pos, ref cameraFollowVelocity,
                _gameContext.cameraSmoothTime.value*deltaTime);
            
            var camPos = _camemraTransform.position;
            var dX = tarPos.x - camPos.x;
            var dY = tarPos.y - camPos.y;
            if (dY < deadZoomY)
            {
                if (dY > -deadZoomY)
                {
                    camY = camPos.y;
                }
                else
                {
                    camY -= deltaTime;
                }
            }
            else
            {
                if (dY < deadZoomY)
                {
                    camY = camPos.y;
                }
                else
                {
                    camY += deltaTime;
                }
            }

            tarPos.y = camY;
            
            _camemraTransform.position = tarPos;

            break;
        }

    }
}
