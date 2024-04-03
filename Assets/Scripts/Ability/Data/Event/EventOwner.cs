using UnityEngine;

namespace WGame.Ability
{
    public interface EventOwner
    {
        void PlayAnim(string animName, int offsetStart, int offsetEnd, int duration, int layer, bool resetLayer);
        void SetAbilityBreak(int stateMask);
        bool CanBreakState(int stateMask);
        bool TryGetNextAbilityID(int inputType, out int id, out int motionType);
        void SetMotionAbility(int motionType, bool force = false);
        void SetAbility(int abilityType, bool force = false);
        void SetWeaponState(bool enable);
        
        void EnableStates(int typeMask);
        void CleanUpState();
        bool IsLockTick { get; set; }
        bool CheckInput(int inputType);
        bool CheckState(int stateMask);
        void SetMoveSpeed(int rate);
        void SetRotateSpeed(int rate);
        void SetRootMotion(int rate);
        void SetCustomValue(string name, TAny value);
        void SetAnimGroup(int id);
        void Move(Vector3 deltaPos);
        void SetGravity(float value, bool reset = false);
        void RegisterTrigger(int triggerType, TriggerAddType addType, int triggerTimes);
    }
}