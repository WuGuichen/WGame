using UnityEngine;
using Weapon;
using WGame.Ability;
using WGame.Res;

public class GotHitAbilityServiceImplementation : IGotHitService
{
    /// <param name="entity">自己</param>
    /// <param name="hitInfo">受击信息</param>
    public bool OnGotHit(GameEntity entity, ContactInfo hitInfo)
    {
        if (!entity.hasLinkMotion)
            return false;

        if (entity.isDeadState)
            return false;

        var motion = entity.linkMotion.Motion;

        var isFwd = Vector3.Dot(entity.gameViewService.service.Model.forward,
            hitInfo.pos - entity.position.value) > 0;

        int hitId;
        if (isFwd)
        {
            hitId = AbilityIDs.LS_Hit_Fwd;
        }
        else{
            hitId = AbilityIDs.LS_Hit_Bwd;
        }
        motion.motionService.service.TransMotionByMotionType(MotionType.Hit, hitId);
        
        return true;
    }

    public bool OnGotHit(GameEntity entity, SensorEntity sensor, int parts)
    {
        ActionHelper.DoMove(entity, sensor.moveDirection.value*sensor.moveInfo.value.Speed, 4f);
        var model = entity.gameViewService.service.Model;
        EffectMgr.LoadEffect("HCFX_Stun", model, entity.gameViewService.service.HeadPos, Quaternion.identity, 2f);
        return true;
    }

    public void OnHitTarget(GameEntity entity, ContactInfo hitInfo)
    {
        if (hitInfo.entity.isDeadState)
            return;
        var vm = entity.linkVM.VM.vMService.service;
        vm.Set("E_HIT_TARGET", hitInfo.entity.entityID.id);
    }
}
