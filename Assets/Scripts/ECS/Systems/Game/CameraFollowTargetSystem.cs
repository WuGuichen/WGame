using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class CameraFollowTargetSystem : IExecuteSystem, IInitializeSystem
{
    private readonly Transform _camemraTransform;
    private readonly GameContext _gameContext;
    private readonly SettingContext _settingContext;

    private Vector3 cameraFollowVelocity = Vector3.zero;
    private IGameViewService _target;
    private IGroup<GameEntity> _cameraGroup;
    private readonly Transform _camTrans;
    private float curDis;
    
    public CameraFollowTargetSystem(Contexts contexts)
    {
        _camemraTransform = contexts.meta.mainCameraService.service.Root;
        _gameContext = contexts.game;
        _cameraGroup = _gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.Camera, GameMatcher.GameViewService));
        _settingContext = contexts.setting;
        _camTrans = contexts.meta.mainCameraService.service.Camera;
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

            var tarPos = Vector3.SmoothDamp(_camemraTransform.position,
                pos, ref cameraFollowVelocity,
                _gameContext.cameraSmoothTime.value);

            _camemraTransform.position = tarPos;

            break;
        }

    }
}
