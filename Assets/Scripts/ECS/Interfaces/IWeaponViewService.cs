namespace Weapon
{
    public interface IWeaponViewService
    {
        IWeaponViewService RegisterEntity(WeaponEntity entity);

        void Push();
        void SetDropState(ref WeaponInfo info);

        void StartHitTargets();
        void EndHitTargets();

        void OnUpdateAttackSensor();
        void Destroy(bool onlyThis = false);
        void UnLinkCharacter(GameEntity entity);
        void LinkToCharacter(GameEntity entity);
    }
}