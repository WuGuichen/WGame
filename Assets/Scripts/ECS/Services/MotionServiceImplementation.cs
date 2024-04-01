using BaseData;
using CleverCrow.Fluid.BTs.Trees;
using Entitas.Unity;
using UnityEngine;
using Motion;

[RequireComponent(typeof(MotionAnimationProcessor))]
[RequireComponent(typeof(Animator))]
public class MotionServiceImplementation : MonoBehaviour, IMotionService
{
    private IFactoryService _factoryService;
    private ITimeService _timeService;
    private InputContext _inputContext;
    private EventNodeScriptableObject currentMotion;

    private MotionEntity entity;
    private GameEntity character;

    #region 状态参数
    
    private float elapsedTime;
    private float ElapsedTime
    {
        get
        {
            return elapsedTime;
        }
        set
        {
            elapsedTime = value;
            checkTime = (elapsedTime) * 1000;
        }
    }

    private float checkTime = 0f;
    private float hasCheckCodeTime = 0f;
    #endregion

    private MotionAnimationProcessor animationProcessor;
    public MotionAnimationProcessor AnimProcessor => animationProcessor;

    private MotionAnimTriggerProcessor animTriggerProcessor;
    public MotionAnimTriggerProcessor AnimTriggerProcessor => animTriggerProcessor;
    private MotionEventTriggerProcessor eventTriggerProcessor;

    private CharacterData _characterData;
    
    public IMotionService OnInit(MotionEntity entity)
    {
        this.entity = entity;
        this.gameObject.Link(entity);
        this.character = this.entity.linkCharacter.Character;
        // _characterData = GameData.Tables.TbCharacter.Get(character.characterInfo.value.);
        if (animTriggerProcessor != null)
            return this;

        _factoryService = Contexts.sharedInstance.meta.factoryService.instance;
        _timeService = Contexts.sharedInstance.meta.timeService.instance;
        _inputContext = Contexts.sharedInstance.input;
        animationProcessor = GetComponent<MotionAnimationProcessor>();
        animationProcessor.OnInit();
        animTriggerProcessor = new MotionAnimTriggerProcessor(this);
        eventTriggerProcessor = new MotionEventTriggerProcessor(this, character);
        
        animTriggerProcessor.RegisterEntity();
        return this;
    }

    #region 处理Motion状态

    private void ProcessLocalMotion()
    {
        if (ElapsedTime > 9f && character.isOnGroundState)
        {
            // SwitchMotion(entity.motionSpare.UID);
        }

        if (character.isMoving || (character.hasAiAgent && character.aiAgent.service.IsActing))
        {
            ElapsedTime = 1f;
        }
    }
    
    private void EnterLocalMotion()
    {
        SwitchMotion(entity.motionLocalMotion.UID);
    }

    public void SwitchMotion(int newID, bool isNet = true)
    {
        if (isNet)
        {
            if (character.hasNetAgent)
            {
                var agent = character.netAgent.Agent;
                agent.SwitchMotion(newID);
                if (!WNetMgr.Inst.IsServer)
                {
                    if (agent.IsServerAgent)
                    {
                        WLogger.Print("客户端改变服务端");
                        return;
                    }
                    else if (agent.IsOwner)
                    {
                    }
                }
            }
        }
        else
        {
            if (character.hasNetAgent)
            {
                var agent = character.netAgent.Agent;
                if (entity.motionStart.UID == newID)
                {
                    // 预测执行服务端数据成功
                    if (!WNetMgr.Inst.IsServer)
                    {
                        if (agent.IsOwner)
                        {
                            // 取消服务端动作
                            return;
                        }
                    }
                }
            }
        }

        if(entity.hasDoMove)
            entity.RemoveDoMove();
        if (entity.hasMotionStart)
        {
            // 结束上一motion的处理
            entity.ReplaceMotionEnd(entity.motionStart.UID);
        }

        // if (entity.motionStart.UID == entity.motionJump.UID && newID == entity.motionLocalMotion.UID)
        // {
        //     // 跳跃落地动画
        //     entity.ReplaceMotionStart(entity.motionJumpLand.UID);
        // }
        // else
        // {
            entity.ReplaceMotionStart(newID);
            // }
    }

    public void SetLocalMotion(int animGroup)
    {
        var cfg = GameData.Tables.TbCharAnim.Get(animGroup);
        animationProcessor.RefreshAnimClip(LocalMotionType.Idle, cfg.Idle);
        animationProcessor.RefreshAnimClip(LocalMotionType.Walk_F, cfg.WalkF);
        animationProcessor.RefreshAnimClip(LocalMotionType.Run_F, cfg.RunF);
        animationProcessor.RefreshAnimClip(LocalMotionType.Walk_B, cfg.WalkB);
        animationProcessor.RefreshAnimClip(LocalMotionType.Run_B, cfg.RunB);
        animationProcessor.RefreshAnimClip(LocalMotionType.Walk_L, cfg.WalkL);
        animationProcessor.RefreshAnimClip(LocalMotionType.Run_L, cfg.RunL);
        animationProcessor.RefreshAnimClip(LocalMotionType.Walk_R, cfg.WalkR);
        animationProcessor.RefreshAnimClip(LocalMotionType.Run_R, cfg.RunR);
    }

    public void ResetMotion()
    {
        SetLocalMotion(1);
        entity.ReplaceMotionAttack1(MotionIDs.Attack1_0);
        if(entity.hasMotionAttack2)
            entity.RemoveMotionAttack2();
        if(entity.hasMotionAttack3)
            entity.RemoveMotionAttack3();
    }

    public void Initialize()
    {
    }

    // 开始新的motion
    public void StartMotion(int motionID)
    {
        currentMotion = _factoryService.GetMotion(motionID);
        ElapsedTime = 0.0f;
        hasCheckCodeTime = -1f;
        animTriggerProcessor.ResetState(currentMotion);
        eventTriggerProcessor.ResetState();
        // 进入后执行第一帧
        if (currentMotion.maxTime == 0)
        {
            animationProcessor.ResetState(true);
        } else
        {
            animationProcessor.ResetState(false);
        }
    }

    public void TransMotionByMotionType(int type)
    {
        var id = GetMotionIDByMotionType(type);
        if (id > 0)
            SwitchMotion(id);
        else
            EnterLocalMotion();
    }

    public void UpdateMotion()
    {
        if (currentMotion.animationNodes == null) return;
        animationProcessor.OnUpdate(checkTime, _timeService.DeltaTime(character.characterTimeScale.rate));
        animTriggerProcessor.OnUpdate(checkTime);
        eventTriggerProcessor.OnUpdate(elapsedTime);
        
        ProcessNodes();
        if (currentMotion.UID == entity.motionLocalMotion.UID)
        {
            ProcessLocalMotion();
        }
        else
        {
            // 检查自动切换
            if (currentMotion.maxTime != 0 && checkTime*0.001f >= currentMotion.maxTime)
            {
                if (!character.isDeadState)
                {
                    EnterLocalMotion();
                    return;
                }
            }
        }


        // 检查手动切换
        for (int i = 0; i < currentMotion.nextReaction.Length; i++)
        {
            var preBreaking = currentMotion.nextBreaking[i];
            if (preBreaking > 0 && checkTime <= currentMotion.breakTime[i])
            {
                int nextMotionID = CheckMotionIDByMotionType(preBreaking);
                if (nextMotionID > 0)
                {
                    SwitchMotion(nextMotionID);
                    return;
                }
            }
            var preReaction = currentMotion.nextReaction[i];
            if (preReaction > 0 && checkTime >= currentMotion.transTime[i])
            {
                // 按顺序检查是否执行下一motion
                int nextMotionID = CheckMotionIDByMotionType(preReaction);
                if (nextMotionID > 0)
                {
                    SwitchMotion(nextMotionID);
                    return;
                }
            }
        }

        if (animTriggerProcessor.LoopTrigger)
        {
            ElapsedTime = animTriggerProcessor.GetLoopTriggerTime();
        }
        else
        {
            ElapsedTime += _timeService.DeltaTime(character.characterTimeScale.rate) * animationProcessor.AnimSpeed;
        }
    }

    private int CheckMotionIDByMotionType(int checkID)
    {
        // WLogger.Print(checkID +"c:" + character.instanceID.ID + "a:" + character.hasSignalDefense);
        return checkID switch
        {
            MotionType.LocalMotion when (character.isPrepareLocalMotionState && entity.hasMotionLocalMotion) => entity.motionLocalMotion.UID,
            MotionType.Step when (character.isPrepareStepState && entity.hasMotionStepFwd) => entity.motionStepFwd.UID,
            MotionType.Attack1 when (character.isPrepareAttackState && entity.hasMotionAttack1) => entity.motionAttack1.UID,
            MotionType.Attack2 when (character.isPrepareAttackState && entity.hasMotionAttack2) => entity.motionAttack2.UID,
            MotionType.Attack3 when (character.isPrepareAttackState && entity.hasMotionAttack3) => entity.motionAttack3.UID,
            MotionType.Jump when (character.isPrepareJumpState && entity.hasMotionJump) => entity.motionJump.UID,
            MotionType.Defense when(character.isPrepareDefenseState && entity.hasMotionDefense && _inputContext.defense.value) => entity.motionDefense.UID,
            MotionType.JumpAttack1 when(character.isPrepareJumpAttackState && entity.hasMotionJumpAttack) => entity.motionJumpAttack.UID,
            _ => -1,
        };
    }
    
    private int GetMotionIDByMotionType(int checkID)
    {
        return checkID switch
        {
            MotionType.LocalMotion when ( entity.hasMotionLocalMotion) => entity.motionLocalMotion.UID,
            MotionType.Step when (entity.hasMotionStepFwd) => entity.motionStepFwd.UID,
            MotionType.Attack1 when (entity.hasMotionAttack1) => entity.motionAttack1.UID,
            MotionType.Attack2 when (entity.hasMotionAttack2) => entity.motionAttack2.UID,
            MotionType.Attack3 when (entity.hasMotionAttack3) => entity.motionAttack3.UID,
            MotionType.Jump when (entity.hasMotionJump) => entity.motionJump.UID,
            MotionType.Defense when(entity.hasMotionDefense) => entity.motionDefense.UID,
            MotionType.JumpAttack1 when(entity.hasMotionJumpAttack) => entity.motionJumpAttack.UID,
            _ => -1,
        };
    }
    #endregion
    
    #region 处理节点
    private void ProcessNodes()
    {
        if (currentMotion.animationNodes != null)
        {
            for(int i = 0; i < currentMotion.animationNodes.Count; i++)
                animationProcessor.ProcessAnimationNode(currentMotion.animationNodes[i]);
        }
        // 指向第i个节点列表
        if (currentMotion.triggerAnimationNodes != null)
        {
            for(int i = 0; i < currentMotion.triggerAnimationNodes.Count; i++)
                animTriggerProcessor.Process(currentMotion.triggerAnimationNodes[i]);
        }
        // currentMotion.triggerAnimationNodes?.ForEach(animTriggerProcessor.Process);
        if (currentMotion.eventTriggerNodes != null)
        {
            for(int i = 0; i < currentMotion.eventTriggerNodes.Count; i++)
                eventTriggerProcessor.Process(currentMotion.eventTriggerNodes[i]);
        }
        // currentMotion.eventTriggerNodes?.ForEach(eventTriggerProcessor.Process);
        if (currentMotion.byteCodeCommandNodes != null)
        {
            for(int i = 0; i < currentMotion.byteCodeCommandNodes.Count; i++)
                ProcessByteCodeCommandNode(currentMotion.byteCodeCommandNodes[i]);
        }
        // currentMotion.byteCodeCommandNodes?.ForEach(ProcessByteCodeCommandNode);
        hasCheckCodeTime = checkTime;
    }

    private void ProcessByteCodeCommandNode(ByteCodeCommandNode node)
    {
        if (!node.active)
            return;
        if (hasCheckCodeTime < node.time && checkTime >= node.time)
        {
            MotionHelper.Inst.InterpretByteCodeNode(node, character);
        }
    }
    
    public void OnMotionExit()
    {
        eventTriggerProcessor.OnMotionEnd();
    }

    public bool CheckMotionType(int motionType)
    {
        var id = GetMotionIDByMotionType(motionType);
        return entity.motionStart.UID == id;
    }

    #endregion

    private void OnAnimatorMove()
    {
        if(animationProcessor != null)
            animationProcessor.OnUpdateAnimator();
    }

    public GameEntity LinkEntity => character;

    public void Destroy()
    {
        gameObject.Unlink();
        this.character = null;
    }
}
