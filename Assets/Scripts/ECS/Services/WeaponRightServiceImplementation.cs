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
            entity.linkMotion.Motion.motionService.service.ResetMotion();
        }
        else
        {
            WLogger.Error("移除不存在的武器？");
        }
    }

    public void ReleaseWeapon(GameEntity entity)
    {
        entity.RemoveLinkWeapon();
    }

    public void SetWeaponOwnerEntity(GameEntity entity)
    {
        throw new System.NotImplementedException();
    }

    public void LinkWeapon(GameEntity entity, WeaponEntity weaponEntity)
    {
        // 确保当前没有武器
        // 添加武器
        entity.AddLinkWeapon(weaponEntity);
        // weaponEntity.weaponWeaponView.service.EquipToCharacter(entity);
    }
}
