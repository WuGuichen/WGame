using CleverCrow.Fluid.BTs.Trees;
using UnityEngine;
using UnityHFSM;
using WGame.Runtime;

public class FightAgent
{
    private readonly IAiAgentService _service;
    private readonly GameEntity _entity;
    
    private BehaviorTree _onAttackTree;
    private readonly BaseData.CharAI _aiCfg = null;
    public FightAgent(IAiAgentService service, GameEntity entity)
    {
        _service = service;
        _entity = entity;
        _aiCfg = _entity.characterInfo.value.AICfg;
        
        InitTree();
        EventCenter.AddListener(EventDefine.OnBTreeHotUpdate, UpdateTree);
    }

    private void UpdateTree(WEventContext ctx)
    {
        if (ctx.pString == _aiCfg.OnFightLogicBTree)
        {
            InitTree();
        }
    }

    private void InitTree()
    {
        // var service = _entity.linkVM.VM.vMService.service;
        // var obj = _entity.gameViewService.service.Model.gameObject;

        // var builder = service.AppendBehaviorTree(_aiCfg.OnFightLogicBTree, obj);
        // _onAttackTree = builder.TREE.Build();
        
        // var motion = _entity.linkMotion.Motion.motionService.service as MotionServiceImplementation;
        // motion.OnAttackTree = _onAttackTree;
    }

    public void OnAttackEnter()
    {
        if (_entity.hasDetectedCharacter)
        {
            _entity.ReplaceFocusEntity(_entity.detectedCharacter.entity);
            _entity.ReplaceFocus(_entity.detectedCharacter.entity.gameViewService.service.Model);
            _service.MoveAgent.RefreshTarget();
        }
        else
        {
            _service.TriggerFSM(StateDefine.LoseTarget);
            _service.MoveAgent.RefreshTarget();
        }
    }
    public void OnAttackLogic()
    {
        _onAttackTree.Tick();
    }

    private void DoAttack()
    {
        int num = Random.Range(0, 2);
        if(num == 0)
            _entity.ReplaceSignalAttack(0.3f);
        else
        {
            _entity.ReplaceSignalStep(0.3f);
        }
    }
    public void OnAttackExit()
    {
        _service.MoveAgent.RefreshTarget();
    }
}
