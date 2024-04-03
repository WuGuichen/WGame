using System.Collections.Generic;
using UnityEngine;
using WGame.Ability;

public class EventOwnerEntity : EventOwner
{
    private readonly GameEntity _entity;
    private readonly CharacterInitInfo _initInfo;
    private readonly MotionEntity _motion;
    private StateMgr _stateMgr;
    private StateMgr _breakStateMgr;
    private WTypeMap _motionDB;

    private float _gravitySpeedConfig;
    
    public void CleanUpState()
    {
        _breakStateMgr.CheckStateChange();
    }

    public bool IsLockTick { get; set; }
    public bool CheckInput(int inputType)
    {
        if (_entity.isCamera)
        {
            var input = _entity.inputState.state;
            return input.CheckState(inputType);
        }
        else
        {
            var signal = _entity.signalState.state;
            return signal.CheckState(inputType);
        }
    }

    public bool CheckState(int stateMask)
    {
        return _entity.characterState.state.CheckState(stateMask);
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

    public void RegisterTrigger(int triggerType, TriggerAddType addType, int triggerTimes)
    {
        throw new System.NotImplementedException();
    }

    public EventOwnerEntity(GameEntity entity)
    {
        _entity = entity;
        _motionDB = entity.motionDB.data;
        _gravitySpeedConfig = Contexts.sharedInstance.setting.gameSetting.value.PlayerConfig.GravitySpeed;
        _initInfo = entity.characterInfo.value;
        _motion = entity.linkMotion.Motion;
        _stateMgr = entity.characterState.state;
        _breakStateMgr = new StateMgr();
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
        _motion.motionService.service.AnimProcessor.PlayAnimationClip(clip, duration*0.001f, offsetStart*0.001f, layer, resetLayer);
    }

    public void SetAbilityBreak(int stateMask)
    {
        _breakStateMgr.EnableState(stateMask);
    }

    public bool CanBreakState(int stateMask)
    {
        return _breakStateMgr.CheckState(stateMask);
    }

    public bool TryGetNextAbilityID(int inputType, out int id, out int motionType)
    {
        id = -1;
        var motionDB = _entity.motionDB.data;
        motionType = -1;
        switch (inputType)
        {
            case InputType.LocalMotion:
                if (_breakStateMgr.CheckState(MotionType.LocalMotion))
                {
                    motionType = MotionType.LocalMotion;
                }
                break;
            case InputType.Attack:
                if (_breakStateMgr.CheckState(MotionType.Attack1))
                {
                    motionType = MotionType.Attack1;
                }
                else if (_breakStateMgr.CheckState(MotionType.Attack2))
                {
                    motionType = MotionType.Attack2;
                }
                else if (_breakStateMgr.CheckState(MotionType.Attack3))
                {
                    motionType = MotionType.Attack3;
                }
                else if (_breakStateMgr.CheckState(MotionType.JumpAttack))
                {
                    motionType = MotionType.JumpAttack;
                }
                break;
            case InputType.Defense:
                if (_breakStateMgr.CheckState(MotionType.Defense))
                {
                    motionType = MotionType.Defense;
                }
                break;
            case InputType.Jump:
                if (_breakStateMgr.CheckState(MotionType.Jump))
                {
                    motionType = MotionType.Jump;
                }
                break;
            case InputType.Step:
                if (_breakStateMgr.CheckState(MotionType.Step))
                {
                    motionType = MotionType.Step;
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
