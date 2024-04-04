using Weapon;

public class NormalHitTargetImpl : IHitTargetService
{
    public bool HitTarget(GameEntity entity, ContactInfo hitInfo)
    {
        if (hitInfo.entity.isDeadState)
            return false;
        var vm = entity.linkVM.VM.vMService.service;
        vm.Set("E_HIT_TARGET", hitInfo.entity.entityID.id);
        return true;
    }
}
