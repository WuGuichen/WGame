using System.Collections.Generic;
using BaseData;
using Entitas.Unity;
using UnityEngine;
using WGame.Ability;

[RequireComponent(typeof(MotionAnimationProcessor))]
[RequireComponent(typeof(Animator))]
public class MotionAbilityImplementation : MonoBehaviour, IMotionService
{
    private ITimeService _timeService;

    private EventOwner _eventOwner;
    private AbilityStatusCharacter _curMotionAbilityStatus;
    private float _curAbilitySpeed;
    private bool _hasChangeAbilitySpeed;

    private MotionEntity entity;
    private GameEntity character;
    private WTypeMap motionDB;

    private int _curAnimGroup;

    #region 状态参数

    private float elapsedTime;
    private float ElapsedTime
    {
        get
        {
            return elapsedTime;
        }
        set
        {
            elapsedTime = value;
            checkTime = (elapsedTime) * 1000;
        }
    }

    private float checkTime = 0f;
    private float hasCheckCodeTime = 0f;
    #endregion

    private MotionAnimationProcessor animationProcessor;
    public MotionAnimationProcessor AnimProcessor => animationProcessor;

    private int _curMotionType = -1;
    private int _endMotionType = -1;

    private CharacterData _characterData;
    
    public IMotionService OnInit(MotionEntity entity)
    {
        this.entity = entity;
        this.gameObject.Link(entity);
        this.character = this.entity.linkCharacter.Character;
        motionDB = character.motionDB.data;
        _curMotionType = MotionType.LocalMotion;
        _endMotionType = MotionType.LocalMotion;
        _timeService = Contexts.sharedInstance.meta.timeService.instance;
        // _inputContext = Contexts.sharedInstance.input;
        animationProcessor = GetComponent<MotionAnimationProcessor>();
        animationProcessor.OnInit();
        return this;
    }

    #region 处理Motion状态

    private void EnterLocalMotion()
    {
        TransMotionByMotionType(MotionType.LocalMotion);
    }

    public void SwitchMotion(int newID, bool isNet = true)
    {
        if (isNet)
        {
            if (character.hasNetAgent)
            {
                var agent = character.netAgent.Agent;
                agent.SwitchMotion(newID);
                if (!WNetMgr.Inst.IsServer)
                {
                    if (agent.IsServerAgent)
                    {
                        WLogger.Print("客户端改变服务端");
                        return;
                    }
                    else if (agent.IsOwner)
                    {
                    }
                }
            }
        }
        else
        {
            if (character.hasNetAgent)
            {
                var agent = character.netAgent.Agent;
                if (entity.motionStart.UID == newID)
                {
                    // 预测执行服务端数据成功
                    if (!WNetMgr.Inst.IsServer)
                    {
                        if (agent.IsOwner)
                        {
                            // 取消服务端动作
                            WLogger.Print("fuwu");
                            return;
                        }
                    }
                }
            }
        }

        if(entity.hasDoMove)
            entity.RemoveDoMove();
        if (entity.hasMotionStart)
        {
            // 结束上一motion的处理
            entity.ReplaceMotionEnd(entity.motionStart.UID);
        }

        entity.ReplaceMotionStart(newID);
    }

    public bool TryGetCurAbilityProperty(string name, out TAny value)
    {
        return _curMotionAbilityStatus.TryGetProperty(name, out value);
    }

    public void SetLocalMotion(int animGroup)
    {
        if (_curAnimGroup == animGroup)
        {
            AnimProcessor.ResetState(true);
            return;
        }
        _curAnimGroup = animGroup;
        var cfg = GameData.Tables.TbCharAnim.Get(animGroup);
        animationProcessor.RefreshLocalMotionClip(LocalMotionType.Idle, cfg.Idle);
        animationProcessor.RefreshLocalMotionClip(LocalMotionType.Walk_F, cfg.WalkF);
        animationProcessor.RefreshLocalMotionClip(LocalMotionType.Run_F, cfg.RunF);
        animationProcessor.RefreshLocalMotionClip(LocalMotionType.Walk_B, cfg.WalkB);
        animationProcessor.RefreshLocalMotionClip(LocalMotionType.Run_B, cfg.RunB);
        animationProcessor.RefreshLocalMotionClip(LocalMotionType.Walk_L, cfg.WalkL);
        animationProcessor.RefreshLocalMotionClip(LocalMotionType.Run_L, cfg.RunL);
        animationProcessor.RefreshLocalMotionClip(LocalMotionType.Walk_R, cfg.WalkR);
        animationProcessor.RefreshLocalMotionClip(LocalMotionType.Run_R, cfg.RunR);
        AnimProcessor.ResetState(true);
    }

    public void SetMotionID(int motionType, string name)
    {
        bool isEmpty = name.Length == 0;
        if (isEmpty)
        {
            motionDB.Remove(motionType);
        }
        else
        {
            motionDB.Set(motionType, WAbilityMgr.Inst.GetAbilityID(name));
        }
    }

    public void Initialize()
    {
        _curMotionAbilityStatus = AbilityStatusCharacter.Empty();
        _curAbilitySpeed = 1f;
        _eventOwner = character.linkAbility.Ability.abilityService.service.Owner;
    }

    // 开始新的motion
    public void StartMotion(int motionID)
    {
        // currentMotion = _factoryService.GetMotion(motionID);
        var data = WAbilityMgr.Inst.GetAbility(motionID);
        if (_curMotionAbilityStatus.IsEnable)
        {
            AbilityStatusCharacter.Push(_curMotionAbilityStatus);
        }
        _curMotionAbilityStatus = AbilityStatusCharacter.Get(_eventOwner, data);

        if (_curMotionAbilityStatus.TryGetProperty("speed", out var value))
        {
            _curAbilitySpeed = value.AsFloat();
            animationProcessor.SetAnimSpeed(_curAbilitySpeed);
            _hasChangeAbilitySpeed = true;
        }
        else
        {
            _curAbilitySpeed = 1f;
            animationProcessor.SetAnimSpeed();
            _hasChangeAbilitySpeed = false;
        }
        
        ElapsedTime = 0.0f;
        hasCheckCodeTime = -1f;
        
        animationProcessor.ResetState(false);
    }

    public void TransMotionByMotionType(int type, int id = -1, bool isOverride = false)
    {
        if (id < 0)
        {
            id = GetMotionIDByMotionType(type);
        }
        else
        {
            if (isOverride)
            {
                motionDB.Set(type, id);
            }
        }
        if (id > 0)
        {
            SwitchMotion(id);
            _curMotionType = type;
        }
        else
        {
            if (type != MotionType.LocalMotion)
            {
                EnterLocalMotion();
                _curMotionType = MotionType.LocalMotion;
            }
        }
    }

    public void UpdateMotion()
    {
        ElapsedTime += _timeService.DeltaTime(character.characterTimeScale.rate) * animationProcessor.AnimSpeed;
        var deltaTime = _timeService.DeltaTime(character.characterTimeScale.rate);
        animationProcessor.OnUpdate(checkTime, deltaTime);
        var abilityDeltaTime = _hasChangeAbilitySpeed ? deltaTime * _curAbilitySpeed : deltaTime;
        _curMotionAbilityStatus.Process((_eventOwner.IsLockTick) ?  0f : abilityDeltaTime);
        if (_curMotionAbilityStatus.IsToEnd)
        {
            EnterLocalMotion();
        }
        _eventOwner.CleanUpState();
    }

    public int CurrentMotionType => _curMotionType;

    private int GetMotionIDByMotionType(int checkID)
    {
        return motionDB.Get(checkID);
    }
    #endregion
    
    public void OnMotionExit()
    {
        if (_endMotionType == MotionType.FinishAttack)
        {
            if (character.hasFinishAtkTarget)
            {
                character.RemoveFinishAtkTarget();
            }
        }
        _endMotionType = _curMotionType;
    }

    public void SetAnimSpeed(float value)
    {
        animationProcessor.SetAnimSpeed(_curAbilitySpeed * value);
    }

    public bool CheckMotionType(int motionType)
    {
        return _curMotionType == motionType;
    }

    private void OnAnimatorMove()
    {
        if(animationProcessor != null)
            animationProcessor.OnUpdateAnimator();
    }

    public GameEntity LinkEntity => character;

    public void Destroy()
    {
        gameObject.Unlink();
        this.character = null;
    }
}
