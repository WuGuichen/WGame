using System;
using System.Collections.Generic;
using DG.Tweening;
using Oddworm.Framework;
using UnityEngine;
using Weapon;
using WGame.Ability;
using WGame.Res;
using WGame.UI;
using WGame.Runtime;
using WGame.Utils;

public partial class ActionHelper
{
    private static readonly int playerLayer = LayerMask.NameToLayer("Player");
    private static readonly int enemyLayer = LayerMask.NameToLayer("Enemy");
    private static readonly int enemySensorLayer = LayerMask.NameToLayer("EnemyHitSensor");
    private static readonly int playerSensorLayer = LayerMask.NameToLayer("PlayerHitSensor");

    private static int currentCameraEntityID = -1;
    public static int CurCameraEntityID => currentCameraEntityID;

    /// <param name="entity">受击者</param>
    /// <param name="hitInfo">受击信息</param>
    public static void DoGotHit(GameEntity entity, ContactInfo hitInfo)
    {
        if (!entity.hasLinkAbility || !entity.linkAbility.Ability.hasAbilityGotHit)
            return;

        var ability = entity.linkAbility.Ability;
        // if (ability.abilityParryAttack.value.Parry(hitInfo))
        // {
        //     return;
        // }
        var ctx = TriggerContext.Get(TriggerEventType.BeHit);
        // ctx.AddProperty("attacker", DataType.Int, hitInfo.entity.instanceID.ID);
        ctx.AddProperty("victim", DataType.Int, entity.instanceID.ID);
        TriggerMgr.Inst.Trigger(ctx);
        entity.notice.service.Notice(WGame.Notice.MessageDB.Getter.GetBehitted(hitInfo));
        ability.abilityGotHit.service.OnGotHit(entity, hitInfo);
    }

    public static void DoHitTarget(GameEntity entity, ContactInfo hitInfo)
    {
        if (!entity.hasLinkAbility || !entity.linkAbility.Ability.hasAbilityGotHit)
            return;
        entity.linkAbility.Ability.abilityGotHit.service.OnHitTarget(entity, hitInfo);
    }
    
    public static void DoReachPoint(GameEntity entity, Vector3 point)
    {
        entity.aiAgent.service.StartPath(point);
    }

    public static void DoSetCharacterCameraByID(int uid)
    {
        if (currentCameraEntityID == uid)
            return;
        var entity = EntityUtils.GetGameEntity(currentCameraEntityID);
        if (entity != null && entity.isEnabled)
        {
            if (entity.hasUIHeadPad)
                entity.uIHeadPad.service.IsActive = true;
            if (entity.hasFocusEntity)
            {
                entity.gameViewService.service.BeFocused(false);
            }

            if (entity.hasLinkSensor)
            {
                var sensorService = entity.linkSensor.Sensor.detectorCharacterService.service;
                sensorService.ClearHateInfoBuffer();
                // sensorService.UpdateDetect(0);
            }

            if (entity.hasAiAgent)
            {
                entity.aiAgent.service.UpdateFSM();
            }
            entity.isCamera = false;
        }

        var entt = EntityUtils.GetGameEntity(uid);
        if (entt != null && entt.isEnabled)
        {
            entt.isCamera = true;
            if (entt.hasUIHeadPad)
                entt.uIHeadPad.service.IsActive = false;
        }
        else
        {
            currentCameraEntityID = uid;
        }
        currentCameraEntityID = uid;
        CharacterModel.Inst.currentControlledCharacterID = uid;
        EventCenter.Trigger(EventDefine.OnControlCharacterChanged, uid);
    }

    public static void DoExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static void DoMove(GameEntity entity, Vector3 tarPos, float speed, DoMoveType type = DoMoveType.Lerp)
    {
        var motion = entity.linkMotion.Motion;
        motion.ReplaceDoMove(tarPos);
        motion.ReplaceDoMoveSpeed(speed);
        motion.ReplaceDoMoveType(type);
    }

    public static void DoFinishAttack(AbilityEntity attacker)
    {
        var victim = EntityUtils.GetGameEntity(attacker.abilityBackStab.EntityID);
        victim.linkMotion.Motion.motionService.service.TransMotionByMotionType(MotionType.VictimFinishAttack, AbilityIDs.LS_FinishVictim);
        attacker.linkCharacter.Character.linkMotion.Motion.motionService.service.TransMotionByMotionType(MotionType.FinishAttack, AbilityIDs.LS_FinishAtk);
        // bool done = victim.linkAbility.Ability.abilityService.service.Do("BeFinishAtk", true);
        // if (done)
        // {
        //     attacker.abilityService.service.Do("FinishAtk", true);
        // }
    }

    public static void DoDropObject(DropObjectInfo info, Vector3 startPos, Vector3 targetPos)
    {
        ObjectPool.Inst.GetOrNewGameObject("DropObject", out var obj);
        var mono = obj.GetComponent<InteractableObjectMono>();
        string effName;
        string effGet;
        switch (info.Rare)
        {
            default:
                effName = "HCFX_Marble_01";
                effGet = "HCFX_Marble_01_Get";
                break;
        }
        var trans = obj.transform;
        if (!mono)
        {
            obj.layer = LayerMask.NameToLayer("DropItem");
            mono = obj.AddComponent<InteractableObjectMono>();
            var col = obj.AddComponent<SphereCollider>();
            col.radius = 0.4f;
        }

        DbgDraw.Sphere(trans.position, Quaternion.identity, new Vector3(0.4f, 0.4f, 0.4f), Color.yellow,2f);
        EffectMgr.LoadEffect(effName, trans, Vector3.zero, Quaternion.identity, 9999999f, o =>
        {
            mono.Initialize(o, effGet);
            trans.position = startPos;
            trans.DOJump(targetPos, 4f, 1, 2f);
        });
    }

    public static void DropWeapon(WeaponEntity weapon, Vector3 endPos)
    {
        var dropInfo = new DropObjectInfo(1);
        weapon.isDestroyed = true;
        DoDropObject(dropInfo, weapon.weaponWeaponView.service.Position, endPos);
    }

    public static void DoCreateWeapon(int typeID, Action<WeaponEntity> onComplete) 
    {
        var data = GameData.Tables.TbWeapon[typeID];
        WeaponMgr.Inst.GetWeaponObj(data.ObjectId, o =>
        {
            var weapon = Contexts.sharedInstance.weapon.CreateEntity();
            var service = o.GetComponent<IWeaponViewService>();
            weapon.AddWeaponWeaponView(service.RegisterEntity(weapon));
            weapon.AddWeaponTypeID(typeID);
            onComplete.Invoke(weapon);
        });
    }

    public static void DoEquipWeaponToEntity(int typeID, GameEntity entity)
    {
        if (entity == null || entity.isEnabled == false)
            return;
        if (entity.hasLinkWeapon)
        {
            WLogger.Print("已有装备");
            return;
        }

        DoCreateWeapon(typeID, weapon =>
        {
            var data = GameData.Tables.TbWeapon[weapon.weaponTypeID.id];
            var motion = entity.linkMotion.Motion.motionService.service;
            motion.SetLocalMotion(data.AnimGroupId);
            var motionEntt = entity.linkMotion.Motion;
            weapon.weaponWeaponView.service.LinkToCharacter(entity);
            motion.SetMotionID(MotionType.Attack1, data.Attack1);
            motion.SetMotionID(MotionType.Attack2, data.Attack2);
            motion.SetMotionID(MotionType.Attack3, data.Attack3);
        });
    }

    public static void Dispose()
    {
        currentCameraEntityID = -1;
    }
}
public struct DropObjectInfo
{
    public int Rare { get; }

    public DropObjectInfo(int rare)
    {
        Rare = rare;
    }
}
    
