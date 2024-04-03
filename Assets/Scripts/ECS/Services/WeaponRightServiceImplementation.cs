using UnityEngine;

public class WeaponRightServiceImplementation : MonoBehaviour, IWeaponService
{
    public void UpdateCollider()
    {
        throw new System.NotImplementedException();
    }

    public Transform WeaponHandle => transform;
    public bool HasWeapon { get; }
    public void OnDropWeapon(GameEntity entity, WeaponEntity weapon)
    {
        if (entity.linkWeapon.Weapon == weapon)
        {
            entity.RemoveLinkWeapon();
            // entity.linkMotion.Motion.motionService.service.ResetMotion();
            ActionHelper.DoEquipWeaponToEntity(2, entity);
        }
        else
        {
            WLogger.Error("移除不存在的武器？");
        }
    }
}
