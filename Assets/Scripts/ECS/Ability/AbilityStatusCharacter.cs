using System.Collections.Generic;
using UnityEngine;
using WGame.Ability;
using WGame.Notice;
using WGame.Res;

public class AbilityStatusCharacter : AbilityStatus
{
    #region pool

    private static Stack<AbilityStatusCharacter> _pool = new();

    public static AbilityStatusCharacter Get(GameEntity entity, AbilityData abilityData)
    {
        if (_pool.Count > 0)
        {
            return _pool.Pop().Init(entity, abilityData);
        }

        return new AbilityStatusCharacter(entity, abilityData);
    }

    public static void Push(AbilityStatusCharacter status)
    {
        status.Reset();
        _pool.Push(status);
    }
    
    private AbilityStatusCharacter(GameEntity entity, AbilityData abilityData)
    {
        Init(entity, abilityData);
    }

    private AbilityStatusCharacter Init(GameEntity entity, AbilityData abilityData)
    {
        _entity = entity;
        _cameraService = Contexts.sharedInstance.meta.mainCameraService.service;
        if (entity.hasLinkMotion)
        {
            _motionService = entity.linkMotion.Motion.motionService.service;
        }
        else
        {
            WLogger.Error("请给有MotionService的entity使用");
        }
        Initialize(abilityData);
        return this;
    }
    
    #endregion

    private GameEntity _entity;
    private IMotionService _motionService;
    private ICameraService _cameraService;
    private int _rotCameraCount = 0;
    private int _moveCameraCount = 0;
    
    protected override void OnStart()
    {
    }

    protected override void OnEnterDurationDoAction(EventDoAction actionData, float duration)
    {
        switch (actionData.ActionType)
        {
            case WActionType.SetUnbalance:
                _entity.isUnbalanced = actionData.ActionParam.Value.AsBool();
                break;
            case WActionType.MoveCamera:
                _cameraService.Move(actionData.ActionParam.Value.AsVector3(), WEaseType.QuadOut, duration*0.001f, 0.2f);
                _moveCameraCount++;
                break;
            case WActionType.RotateCamera:
                _cameraService.Rotate(actionData.ActionParam.Value.AsVector3(), WEaseType.QuadOut, 1000f/duration, 0.2f);
                _rotCameraCount++;
                break;
        }
    }

    protected override void OnEndDoAction(EventDoAction actionData)
    {
        switch (actionData.ActionType)
        {
            case WActionType.SetUnbalance:
                _entity.isUnbalanced = false;
                break;
            case WActionType.MoveCamera:
                if (--_moveCameraCount <= 0)
                {
                    _cameraService.StopMove();
                }
                break;
            case WActionType.RotateCamera:
                _rotCameraCount--;
                if (_rotCameraCount <= 0)
                {
                    _cameraService.StopRotate();
                }
                break;
        }
    }

    protected override void OnTriggerDoAction(EventDoAction actionData)
    {
    }

    protected override void OnEnterDurationPlayAnim(EventPlayAnim animData)
    {
        var clip = WAbilityMgr.Inst.GetAnimClip(animData.AnimName);
        _motionService.AnimProcessor.RootMotionRate = 100;
        _motionService.AnimProcessor.PlayAnimationClip(clip);
    }

    protected override void OnEndPlayAnim(EventPlayAnim animData)
    {
        _motionService.TransMotionByMotionType(MotionType.LocalMotion);
    }

    protected override void OnTriggerPlayEffect(EventPlayEffect effectData)
    {
        EffectMgr.LoadEffect(effectData.AddressName, _entity.gameViewService.service.Model, _entity.position.value,
            Quaternion.identity);
    }

    protected override void OnNoticeMessage(EventNoticeMessage message)
    {
            _entity.notice.service.TriggerReceiver(NoticeDB.OnUseAbility,
                MessageDB.Getter.GetCastSkill(new EntityMoveInfo(EntityMoveType.Move_Dir_Linear, 10f)));
    }
}
