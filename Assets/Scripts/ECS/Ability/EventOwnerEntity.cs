using UnityEngine;
using WGame.Ability;
using WGame.Attribute;
using WGame.Utils;

public class EventOwnerEntity : EventOwner
{
    private readonly GameEntity _entity;
    public int EntityID { get; private set; }
    private readonly CharacterInitInfo _initInfo;
    private readonly MotionEntity _motion;
    private readonly IMotionService _motionService;
    private StateMgr _stateMgr;
    private StateMgr _breakStateMgr;
    private StateMgrSingle _areaMgr;
    private WTypeMap _motionDB;
    private WAttribute _attribute;

    private float _gravitySpeedConfig;

    private WTypeMap _triggerTimesDict = new WTypeMap();

    public void CleanUpState()
    {
        _breakStateMgr.CheckStateChange();
    }

    public bool IsLockTick { get; set; }
    public bool CheckInput(int inputType, bool usePreInput)
    {
        if (!usePreInput && _entity.isCamera)
        {
            var input = _entity.inputState.state;
            return input.Check(inputType);
        }
        else
        {
            var signal = _entity.signalState.state;
            return signal.Check(inputType);
        }
    }

    public bool CheckState(int stateMask)
    {
        return _entity.characterState.state.Check(stateMask);
    }

    public void SetMoveSpeed(int rate)
    {
        _entity.ReplaceAnimMoveMulti(rate);
    }

    public void SetRotateSpeed(int rate)
    {
        _entity.ReplaceAnimRotateMulti(rate);
    }

    public void SetRootMotion(int rate)
    {
        _entity.linkMotion.Motion.motionService.service.AnimProcessor.RootMotionRate = rate;
    }

    public void SetCustomValue(string name, TAny value)
    {
        throw new System.NotImplementedException();
    }

    public void SetAnimGroup(int id)
    {
        _motion.motionService.service.SetLocalMotion(id);
    }

    public void Move(Vector3 deltaPos)
    {
        _motion.motionService.service.AnimProcessor.DeltaEventMovePos += deltaPos;
        // _entity.rigidbodyService.service.MovePosition(deltaPos);
    }

    public void SetGravity(float value, bool reset = false)
    {
        if (reset)
        {
            _entity.ReplaceCharGravity(_gravitySpeedConfig);
            return;
        }
        _entity.ReplaceCharGravity(value);
    }

    public void AddNoticeReceiver(int key, int times = 10, bool replace = false)
    {
        _entity.notice.service.AddReciever(key, times, replace);
    }
    
    public void RemoveNoticeReceiver(int key)
    {
        _entity.notice.service.RemoveReciever(key);
    }

    public bool TryGetFocusPosition(out Vector3 pos)
    {
        if (_entity.hasFocusEntity)
        {
            pos = _entity.focusEntity.entity.position.value;
            return true;
        }

        pos = Vector3.zero;
        return false;
    }

    public bool TryGetFocusDistance(out float dist)
    {
        if (_entity.hasFocusEntity)
        {
            dist = DetectMgr.Inst.GetDistance(_entity.focusEntity.entity.instanceID.ID, EntityID);
            return true;
        }

        dist = -1;
        return false;
    }

    public bool TryGetAngleFocusToEntity(out float angle)
    {
        if (_entity.hasFocusEntity)
        {
            angle = DetectMgr.Inst.GetAngle(_entity.focusEntity.entity.instanceID.ID, EntityID);
            return true;
        }

        angle = -1;
        return false;
    }

    public bool TryGetAngleToFocus(out float angle)
    {
        if (_entity.hasFocusEntity)
        {
            angle = DetectMgr.Inst.GetAngle(EntityID, _entity.focusEntity.entity.instanceID.ID);
            return true;
        }

        angle = -1;
        return false;
    }

    public void SetIsInPerfectArea(int areaType, bool value)
    {
        if (value)
        {
            _areaMgr.EnableState(areaType);
        }
        else
        {
            _areaMgr.DisableState(areaType);
        }
    }

    public void ApplyHitToFinishAtkTarget(int hitRate)
    {
        if (_entity.hasFinishAtkTarget)
        {
            _entity.finishAtkTarget.target.attribute.value.OnGotHit(hitRate, _entity, _attribute);
        }
        else
        {
            WLogger.Print("有错误");
        }
    }

    public EventOwnerEntity(GameEntity entity)
    {
        _entity = entity;
        EntityID = entity.instanceID.ID;
        _motionDB = entity.motionDB.data;
        _attribute = entity.attribute.value;
        _gravitySpeedConfig = Contexts.sharedInstance.setting.gameSetting.value.PlayerConfig.GravitySpeed;
        _initInfo = entity.characterInfo.value;
        _motion = entity.linkMotion.Motion;
        _motionService = _motion.motionService.service;
        _stateMgr = entity.characterState.state;
        _breakStateMgr = new StateMgr();
        _areaMgr = entity.perfectTimeArea.state;
        _stateMgr.onStateEnable += change =>
        {
            if ((change & AStateType.EnableWeapon) != 0)
            {
                SetWeaponState(true);
            }
        };
        _stateMgr.onStateDisable += change =>
        {
            if ((change & AStateType.EnableWeapon) != 0)
            {
                SetWeaponState(false);
            }
        };
    }
    
    public void PlayAnim(string animName, int offsetStart, int offsetEnd, int duration, int layer, bool resetLayer)
    {
        var clip = WAbilityMgr.Inst.GetAnimClip(animName);
        _motionService.AnimProcessor.PlayAnimationClip(clip, duration*0.001f, offsetStart*0.001f, layer, resetLayer);
    }

    public void SetAnimSpeed(float rate)
    {
        _motionService.SetAnimSpeed(rate);
    }

    public void SetAbilityBreak(int stateMask)
    {
        _breakStateMgr.EnableState(stateMask);
    }

    public void SetProperty<T>(string name, DataType type, T value)
    {
        _entity.property.value.AddProperty(name, type, value);
    }

    public void SetAttribute(int attrID, int value)
    {
        _attribute.Set(attrID, value);
    }

    public void SetAreaAttr(int areaType, bool isEndArea)
    {
        string vecName = null;
        string dmgName = null;
        switch (areaType)
        {
            case TimeAreaType.PerfectDamage:
                if (isEndArea)
                {
                    vecName = "vec";
                    dmgName = "dmg";
                }
                else
                {
                    vecName = "maxVec";
                    dmgName = "maxDmg";
                }
                break;
            case TimeAreaType.PerfectDamage2:
                if (isEndArea)
                {
                    vecName = "vec2";
                    dmgName = "dmg2";
                }
                else
                {
                    vecName = "maxVec2";
                    dmgName = "maxDmg2";
                }
                break;
            case TimeAreaType.PerfectDamage3:
                if (isEndArea)
                {
                    vecName = "vec3";
                    dmgName = "dmg3";
                }
                else
                {
                    vecName = "maxVec3";
                    dmgName = "maxDmg3";
                }
                break;
            case TimeAreaType.PerfectDamage4:
                if (isEndArea)
                {
                    vecName = "vec4";
                    dmgName = "dmg4";
                }
                else
                {
                    vecName = "maxVec4";
                    dmgName = "maxDmg4";
                }
                break;
            case TimeAreaType.Beginning:
                if (!isEndArea)
                {
                    vecName = "vec";
                    dmgName = "dmg";
                }
                break;
            default:
                return;
        }

        if (vecName != null)
        {
            if (_motionService.TryGetCurAbilityProperty(vecName, out var value))
            {
                _attribute.Set(WAttrType.ImpactVec, value.AsInt());
            }
            else
            {
                WLogger.Error("数据未配置" + vecName);
            }
            if (_motionService.TryGetCurAbilityProperty(dmgName, out var dmgValue))
            {
                _attribute.Set(WAttrType.DmgRate, dmgValue.AsInt());
            }
            else
            {
                WLogger.Error("数据未配置" + dmgName);
            }
        }
    }

    public bool CanBreakState(int stateMask)
    {
        return _breakStateMgr.Check(stateMask);
    }

    public bool TryGetNextAbilityID(int inputType, out int id, out int motionType)
    {
        id = -1;
        var motionDB = _entity.motionDB.data;
        motionType = -1;
        switch (inputType)
        {
            case InputType.LocalMotion:
                if (_breakStateMgr.Check(MotionType.LocalMotion))
                {
                    motionType = MotionType.LocalMotion;
                }
                break;
            case InputType.Attack:
                if (_breakStateMgr.Check(MotionType.Attack1))
                {
                    motionType = MotionType.Attack1;
                }
                else if (_breakStateMgr.Check(MotionType.Attack2))
                {
                    motionType = MotionType.Attack2;
                }
                else if (_breakStateMgr.Check(MotionType.Attack3))
                {
                    motionType = MotionType.Attack3;
                }
                else if (_breakStateMgr.Check(MotionType.JumpAttack))
                {
                    motionType = MotionType.JumpAttack;
                }
                break;
            case InputType.Defense:
                if (_breakStateMgr.Check(MotionType.Defense))
                {
                    motionType = MotionType.Defense;
                }
                break;
            case InputType.Jump:
                if (_breakStateMgr.Check(MotionType.Jump))
                {
                    motionType = MotionType.Jump;
                }
                break;
            case InputType.Step:
                if (_breakStateMgr.Check(MotionType.Step))
                {
                    motionType = MotionType.Step;
                }
                else if (_breakStateMgr.Check(MotionType.StepEmergency))
                {
                    motionType = MotionType.StepEmergency;
                }
                break;
        }

        if(motionType > 0)
        {
            _motionDB.TryGet(motionType, out id);
        }
        return id >= 0;
    }

    public void SetMotionAbility(int motionType, bool force = false)
    {
        if (force || CanBreakState(motionType))
        {
            _entity.linkMotion.Motion.motionService.service.TransMotionByMotionType(motionType);
        }
    }
    
    public void SetAbility(int abilityType, bool force = false)
    {
    
    }

    public void SetWeaponState(bool enable)
    {
        if (_entity.hasLinkWeapon)
        {
            if (enable)
            {
                _entity.linkWeapon.Weapon.weaponWeaponView.service.StartHitTargets();
            }
            else
            {
                _entity.linkWeapon.Weapon.weaponWeaponView.service.EndHitTargets();
            }
        }
    }

    public void EnableStates(int typeMask)
    {
        _stateMgr.EnableState(typeMask);
    }
}
