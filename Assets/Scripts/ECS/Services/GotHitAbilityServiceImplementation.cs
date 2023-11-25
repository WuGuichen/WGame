using Motion;
using UnityEngine;
using Weapon;
using WGame.Attribute;

public class GotHitAbilityServiceImplementation : IGotHitService
{
    private int curHP;

    private int CurHP
    {
        get => curHP;
        set
        {
            curHP = value;
        }
    }
    public GotHitAbilityServiceImplementation()
    {
    }

    public void OnGotHit(GameEntity entity, ContactInfo hitInfo)
    {
        if (!entity.hasLinkMotion)
            return;

        if (entity.isDeadState)
            return;

        var vm = entity.linkVM.VM.vMService.service;
        var attackerPos = hitInfo.entity.gameViewService.service.PlanarPosition;
        var myPos = entity.gameViewService.service.PlanarPosition;
        var hitDir = (myPos - attackerPos).normalized;
        var thrustFwd = hitInfo.entity.attribute.value.Get(WAttrType.AtkForceFwd);

        var isFwd = Vector3.Dot(entity.gameViewService.service.Model.forward,
            hitInfo.pos - entity.gameViewService.service.Position) > 0;
        
        vm.Set("THRUST_FWD", thrustFwd);
        vm.Set("E_ATTACKER", hitInfo.entity.entityID.id);
        vm.Set("HIT_DIR", new Vector3(hitDir.x, 0, hitDir.y), false);
        vm.Set("HIT_IS_FWD", isFwd);
        vm.Set("HIT_POS", hitInfo.pos, false);
        if (MotionIDs.onHitDict.TryGetValue(entity.linkMotion.Motion.motionStart.UID, out var motionName))
        {
            vm.Call(motionName);
        }
        else
        {
            vm.Call("GotHit1");
        }
    }

    public void OnHitTarget(GameEntity entity, ContactInfo hitInfo)
    {
        if (hitInfo.entity.isDeadState)
            return;
        var vm = entity.linkVM.VM.vMService.service;
        vm.Set("E_HIT_TARGET", hitInfo.entity.entityID.id);
        // vm.Call("HitTarget1");
    }
}
