using UnityEngine;

public interface IWeaponService
{
    void UpdateCollider();
    
    Transform WeaponHandle { get; }
    bool HasWeapon { get; }
    void OnDropWeapon(GameEntity entity, WeaponEntity weapon);
}
