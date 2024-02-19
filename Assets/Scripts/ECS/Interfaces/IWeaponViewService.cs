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
        void UnLinkCharacter(GameEntity entity, bool resetLocalMotion = false);
        void LinkToCharacter(GameEntity entity);
    }
}