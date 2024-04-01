using UnityEngine;
using Weapon;
using WGame.UI;
using WGame.Runtime;

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
        // bool done = victim.linkAbility.Ability.abilityService.service.Do("BeFinishAtk", true);
        // if (done)
        // {
        //     attacker.abilityService.service.Do("FinishAtk", true);
        // }
    }

    public static void Dispose()
    {
        currentCameraEntityID = -1;
    }
    
    
}
