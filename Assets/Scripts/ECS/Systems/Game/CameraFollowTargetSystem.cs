using Entitas;
using UnityEngine;

public class CameraFollowTargetSystem : IExecuteSystem, IInitializeSystem
{
    private readonly Transform _camemraTransform;
    private readonly GameContext _gameContext;
    private readonly SettingContext _settingContext;
    private readonly ITimeService _time;
    private readonly ICameraService _cameraService;

    private Vector3 cameraFollowVelocity = Vector3.zero;
    private IGameViewService _target;
    private IGroup<GameEntity> _cameraGroup;
    private readonly Transform _camTrans;
    private float curDis;
    private float deadZoomY = 2.2f;
    private float deadZoomX = 0.4f;
    private float deadZoomZ = 0.1f;
    private float camY = 0f;
    private float camZ = 0f;
    private float camX = 0f;
    
    public CameraFollowTargetSystem(Contexts contexts)
    {
        _camemraTransform = contexts.meta.mainCameraService.service.Root;
        _gameContext = contexts.game;
        _cameraGroup = _gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.Camera, GameMatcher.GameViewService));
        _cameraService = contexts.meta.mainCameraService.service;
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
            var deadY = deadZoomY;
            var deadX = deadZoomX;
            var deadZ = deadZoomZ;
            if (entity.groundSensor.intersect)
            {
                deadY = 0.4f;
                pos.y -= deadY;
            }

            // if (EntityUtils.CheckMotion(entity, MotionType.LocalMotion))
            // {
            //     deadXZ = 0.2f;
            // }
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

            var camFwd = _cameraService.CachedFwd;
            var camPos = _camemraTransform.position;
            // tarPos = _camemraTransform.InverseTransformVector(tarPos);
            // camPos = _camemraTransform.InverseTransformVector(camPos);
            var dX = tarPos.x - camPos.x;
            var dZ = tarPos.z - camPos.z;
            var dY = tarPos.y - camPos.y;

            SmoothDeadZoom(ref camY, dY, deadY, camPos.y, deltaTime);
            SmoothDeadZoom(ref camZ, dZ, deadX, camPos.z, deltaTime, 2f);
            SmoothDeadZoom(ref camX, dX, deadX, camPos.x, deltaTime, 2f);

            var newTarPos = new Vector3(camX, camY, camZ);
            
            _camemraTransform.position = newTarPos;

            break;
        }

    }

    private static void SmoothDeadZoom(ref float camTarPos, float dPos, float deadZoom, float camPos, float deltaTime, float dampSpeed = 1f)
    {
        var absDPos = Mathf.Abs(dPos);
        var moveRate = (absDPos > deadZoom) ? absDPos * (dampSpeed / deadZoom) : dampSpeed;
        if (absDPos > deadZoom)
        {
            // 超死区
            var fixPos = deltaTime * moveRate;
            var deltaPos = absDPos - deadZoom;
            if(fixPos > deltaPos)
            {
                fixPos = deltaPos;
            }
            if (dPos < 0)
            {
                camTarPos -= fixPos;
            }
            else
            {
                camTarPos += fixPos;
            }
        }
        else
        {
            camTarPos = camPos;
        }
        // if (dPos < deadZoom)
        // {
        //     if (dPos > -deadZoom)
        //     {
        //         camTarPos = camPos;
        //     }
        //     else
        //     {
        //         camTarPos -= deltaTime * moveRate;
        //         if (camTarPos < -deadZoom)
        //         {
        //             camTarPos = -deadZoom;
        //         }
        //     }
        // }
        // else
        // {
        //     if (dPos < deadZoom)
        //     {
        //         camTarPos = camPos;
        //     }
        //     else
        //     {
        //         // 超出上限(camTarPos )
        //         var changePos = deltaTime * moveRate;
        //         if (changePos > deadZoom)
        //         {
        //             camTarPos = deadZoom;
        //         }
        //         camTarPos += changePos;
        //     }
        // }
    }
}
