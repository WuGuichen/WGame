using System.Collections.Generic;
using Entitas;
using WGame.Attribute;

public class ProcessMotionStartSystem : ReactiveSystem<MotionEntity>
{
    public ProcessMotionStartSystem(Contexts contexts) : base(contexts.motion)
    {
        
    }


    protected override ICollector<MotionEntity> GetTrigger(IContext<MotionEntity> context)
    {
        return context.CreateCollector(MotionMatcher.MotionStart);
    }

    protected override bool Filter(MotionEntity entity)
    {
        return entity.hasMotionStart;
    }

    protected override void Execute(List<MotionEntity> entities)
    {
        entities.ForEach(OnMotionStart);
    }

    private static void OnMotionStart(MotionEntity entity)
    {
        var character = entity.linkCharacter.Character;
        character.signalState.state.ResetStates();
        character.isLockPlanarVec = false;
        
        character.ReplaceAnimMoveMulti(100);
        character.ReplaceAnimRotateMulti(100);
        if(character.hasLinkWeapon)
            character.linkWeapon.Weapon.weaponWeaponView.service.EndHitTargets();

        var anim = entity.motionService.service.AnimProcessor;
        anim.RootMotionRate = 0;
        character.attribute.value.Set(WAttrType.ImpactVec, 0);

        character.isRotateInFocus = false;

        entity.motionService.service.StartMotion(entity.motionStart.UID);
        // 清除输入指令
        if (character.hasSignalAttack)
            character.RemoveSignalAttack();
        if (character.hasSignalJump)
            character.RemoveSignalJump();
        if (character.hasSignalStep)
            character.RemoveSignalStep();
        if (character.hasSignalLocalMotion)
            character.RemoveSignalLocalMotion();
        if (character.hasSignalDefense )
            character.RemoveSignalDefense();
            
        // if (character.hasNotice)
        // {
            // var noticeService = character.notice.service;
            //
            // // 根据动作类型处理行为
            // var newID = entity.motionStart.UID;
            // var motionDB = character.motionDB.data;
            // if (newID != motionDB.Get(MotionType.FinishAttack))
            // {
            // }
            // else if (newID == motionDB.Get(MotionType.Step))
            // {
            //     noticeService.AddReciever(NoticeDB.OnStepBeHit, 1f);
            // }
        // }
    }
}
