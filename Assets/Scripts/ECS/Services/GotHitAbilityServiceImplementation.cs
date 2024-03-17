using Motion;
using UnityEngine;
using Weapon;
using WGame.Attribute;
using WGame.Res;

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

    /// <param name="entity">自己</param>
    /// <param name="hitInfo">受击信息</param>
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
            hitInfo.pos - entity.position.value) > 0;
        
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
        
        MethodDefine.AddHatePointTo(hitInfo.entity.instanceID.ID, entity, 360*3, HatePointType.BeHitted);
    }

    public void OnGotHit(GameEntity entity, SensorEntity sensor, int parts)
    {
        ActionHelper.DoMove(entity, sensor.moveDirection.value*sensor.moveInfo.value.Speed, 4f);
        var model = entity.gameViewService.service.Model;
        EffectMgr.LoadEffect("HCFX_Stun", model, entity.gameViewService.service.HeadPos, Quaternion.identity, 2f);
    }

    public void OnHitTarget(GameEntity entity, ContactInfo hitInfo)
    {
        if (hitInfo.entity.isDeadState)
            return;
        var vm = entity.linkVM.VM.vMService.service;
        vm.Set("E_HIT_TARGET", hitInfo.entity.entityID.id);
        // vm.Call("HitTarget1");
        // WAudioMgr.Inst.SetSoundVolume(Random.Range(0, 100));
        // WAudioMgr.Inst.PlaySound("Play_Bloody_punch", hitInfo.entity.gameViewService.service.Model.gameObject);
    }
}
