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

    private static int currentCameraEntityID = 0;
    public static int CurCameraEntityID => currentCameraEntityID;
    
    /// <param name="entity">受击者</param>
    /// <param name="hitInfo">受击信息</param>
    public static void DoGotHit(GameEntity entity, ContactInfo hitInfo)
    {
        if (!entity.hasLinkAbility || !entity.linkAbility.Ability.hasAbilityGotHit)
            return;
        
        entity.notice.service.Notice(WGame.Notice.MessageDB.Getter.GetBehitted(hitInfo));
        entity.linkAbility.Ability.abilityGotHit.service.OnGotHit(entity, hitInfo);
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
        if (currentCameraEntityID > 100)
        {
            var entity = Contexts.sharedInstance.game.GetEntityWithEntityID(currentCameraEntityID);
            if (entity != null && entity.isEnabled)
            {
                entity.isCamera = false;
                if (entity.hasUIHeadPad)
                    entity.uIHeadPad.service.IsActive = true;
                if (entity.hasFocusEntity)
                {
                    entity.gameViewService.service.BeFocused(false);
                }
            }
        }
        var entt = Contexts.sharedInstance.game.GetEntityWithEntityID(uid);
        if (entt != null && entt.isEnabled)
        {
            entt.isCamera = true;
            currentCameraEntityID = uid;
            CharacterModel.Inst.currentControlledCharacterID = uid;
            EventCenter.Trigger(EventDefine.OnControlCharacterChanged, WEventContext.Get(uid));
            if(entt.hasUIHeadPad)
                entt.uIHeadPad.service.IsActive = false;
        }
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
}
