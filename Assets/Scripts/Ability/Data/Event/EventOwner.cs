using UnityEngine;

namespace WGame.Ability
{
    public interface EventOwner
    {
        int EntityID { get; }
        void PlayAnim(string animName, int offsetStart, int offsetEnd, int duration, int layer, bool resetLayer);
        void SetAnimSpeed(float rate);
        void SetAbilityBreak(int stateMask);
        void SetProperty<T>(string name, Utils.DataType type,T value);
        void SetAttribute(int attrID, int value);
        void SetAreaAttr(int areaType, bool isEndArea);
        bool CanBreakState(int stateMask);
        bool TryGetNextAbilityID(int inputType, out int id, out int motionType);
        void SetMotionAbility(int motionType, bool force = false);
        void SetAbility(int abilityType, bool force = false);
        void SetWeaponState(bool enable);
        
        void EnableStates(int typeMask);
        void CleanUpState();
        bool IsLockTick { get; set; }
        bool CheckInput(int inputType, bool usePreInput);
        bool CheckState(int stateMask);
        void SetMoveSpeed(int rate);
        void SetRotateSpeed(int rate);
        void SetRootMotion(int rate);
        void SetCustomValue(string name, TAny value);
        void SetAnimGroup(int id);
        void Move(Vector3 deltaPos);
        void SetGravity(float value, bool reset = false);
        void AddNoticeReceiver(int key, int times = 10, bool replace = false);
        void RemoveNoticeReceiver(int key);
        bool TryGetFocusPosition(out Vector3 pos);
        bool TryGetFocusDistance(out float dist);
        bool TryGetAngleFocusToEntity(out float angle);
        bool TryGetAngleToFocus(out float angle);
        void SetIsInPerfectArea(int areaType, bool value);
        void ApplyHitToFinishAtkTarget(int hitRate);
    }
}