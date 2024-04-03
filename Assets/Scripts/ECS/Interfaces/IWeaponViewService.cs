using UnityEngine;

namespace Weapon
{
    public interface IWeaponViewService
    {
        IWeaponViewService RegisterEntity(WeaponEntity entity);
        Vector3 Position { get; }

        void Push();
        void SetDropState(ref WeaponInfo info);

        void StartHitTargets();
        void EndHitTargets();

        void OnUpdateAttackSensor();
        void Destroy(bool onlyThis = false);
        // void UnLinkCharacter(GameEntity entity);
        void LinkToCharacter(GameEntity entity);
    }
}