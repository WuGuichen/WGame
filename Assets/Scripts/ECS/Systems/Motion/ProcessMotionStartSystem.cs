using System.Collections.Generic;
using Entitas;

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
        // Debug.Log("MotionStart");
        // entity.RemoveMotionEnd();
        var character = entity.linkCharacter.Character;
        // character.isPrepareAttackState = false;
        // character.isPrepareJumpState = false;
        // character.isPrepareStepState = false;
        // character.isPrepareLocalMotionState = false;
        // character.isPrepareDefenseState = false;
        // character.isPrepareJumpAttackState = false;
        character.signalState.state.ResetStates();
        character.isLockPlanarVec = false;
        
        character.ReplaceAnimMoveMulti(100);
        character.ReplaceAnimRotateMulti(100);
        if(character.hasLinkWeapon)
            character.linkWeapon.Weapon.weaponWeaponView.service.EndHitTargets();

        entity.motionService.service.AnimProcessor.SetAnimSpeed(1);

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
            
        if (character.hasNotice)
        {
            var noticeService = character.notice.service;

            // 根据动作类型处理行为
            var newID = entity.motionStart.UID;
            if (newID == entity.motionLocalMotion.UID)
            {

            }
            else if (newID == entity.motionDefense.UID)
            {
            }
            else if (newID == entity.motionStepFwd.UID)
            {
                noticeService.AddReciever(NoticeDB.OnStepBeHit, 1f);
            }
        }
    }
}
