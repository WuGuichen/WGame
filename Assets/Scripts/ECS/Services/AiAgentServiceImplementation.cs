using System;
using System.Collections;
using System.Collections.Generic;
using BaseData;
using CleverCrow.Fluid.BTs.Trees;
using Pathfinding;
using UnityEngine;
using UnityHFSM;

public partial class AiAgentServiceImplementation : IAiAgentService
{
    private GameEntity _entity;
    private Seeker _seeker;
    private Path _path;

    private readonly ITimeService _timeService;
    private readonly IVMService _vmService;
    private readonly ICharacterUIService _uiService;
    private const float COOL_DOWN_TIME = 0.2f;
    private float cooldownTimer;

    private int currentPathIndex;
    private float preSqrDist;

    private StateMachine<int, int, int> fsm;

    private readonly MoveAgent moveAgent;
    private readonly FightAgent fightAgent;
    private readonly FSMAgent fsmAgent;
    private readonly BTreeAgent bTreeAgent;

    private readonly MotionServiceImplementation _motion;

    private readonly CharacterInitInfo _initInfo;

    private readonly CharAI _aiCfg = null;

    private Path path
    {
        get => _path;
        set
        {
            _path = value;
            if (_path == null)
            {
                _entity.ReplaceMoveDirection(Vector3.zero);
            }
        }
    }

    public AiAgentServiceImplementation(GameEntity entity, Seeker seeker, Vector3[] patrolPoints)
    {
        _seeker = seeker;
        _entity = entity;
        _aiCfg = entity.characterInfo.value.AICfg;
        _initInfo = entity.characterInfo.value;
        moveAgent = new MoveAgent(this, seeker, entity, patrolPoints);
        fightAgent = new FightAgent(this, entity);
        _timeService = Contexts.sharedInstance.meta.timeService.instance;
        _uiService = entity.uIHeadPad.service;
        _vmService = _entity.linkVM.VM.vMService.service;
        fsmAgent = FSMAgent.Get(_vmService);
        fsmAgent.SetFSMConfig(_aiCfg);
        bTreeAgent = BTreeAgent.Get(_vmService);
        _motion = _entity.linkMotion.Motion.motionService.service as MotionServiceImplementation;

        InitMethod();
    }
    
    private void SetPatrolPointTarget(List<Symbol> list, Interpreter interpreter)
    {
        moveAgent.SetPatrolPointTarget();
    }
    private void MoveToTarget(List<Symbol> list, Interpreter interpreter)
    {
        UnityEngine.Profiling.Profiler.BeginSample("FSM_BTree_MoveToTarget");
        if (list.Count == 0)
        {
            interpreter.SetRetrun(moveAgent.MoveToTarget());
        }
        else
        {
            if (list[0].Type == BaseDefinition.TYPE_FLOAT)
                interpreter.SetRetrun(moveAgent.MoveToTarget(list[0].ToFloat(interpreter.Definition)));
            else
                interpreter.SetRetrun(moveAgent.MoveToTarget(list[0].Value));
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    private void OnTestWaitEnter(List<Symbol> param, Interpreter interpreter)
    {
        WLogger.Print(_entity.entityID.id.ToString() + "Wait");
    }
    
    private void OnTestAttackEnter(List<Symbol> param, Interpreter interpreter)
    {
        WLogger.Print(_entity.entityID.id.ToString() + "Attack");
    }

    private void SetMethod(string name, Action<List<Symbol>, Interpreter> callback)
    {
        _vmService.Set(name, Method.Get(name, callback));
    }

    private void OnChaseEnter()
    {
        _entity.ReplaceMovementSpeed(_initInfo.moveSpeed*_initInfo.ChaseMul);
        _entity.ReplaceFocusEntity(_entity.detectedCharacter.entity);
        moveAgent.OnChaseEnter();
    }
    
    private void OnChaseLogic()
    {
        moveAgent.OnChaseLogic();
    }
    
    private void OnChaseExit()
    {
        _entity.ReplaceMovementSpeed(_initInfo.moveSpeed);
        moveAgent.OnChaseExit();
        // if(_entity.hasFocus)
        //     _entity.RemoveFocus();
        if(_entity.hasFocusEntity)
            _entity.RemoveFocusEntity();
    }

    private void OnAttackEnter()
    {
        _entity.ReplaceKeepTargetDistance(1f);
        fightAgent.OnAttackEnter();
    }

    private void OnAttackLogic()
    {
        fightAgent.OnAttackLogic();
    }
    
    private void OnAttackExit()
    {
        if(_entity.hasKeepTargetDistance)
            _entity.RemoveKeepTargetDistance();
        fightAgent.OnAttackExit();
    }
    
    IEnumerator MoveTo(Vector3 pos)
    {
        while (true)
        {
            yield return null;
        }
    }

    private bool isActing = false;
    public bool IsActing
    {
        get => isActing;
        set
        {
            isActing = value;
        }
    }
    
    private Dictionary<string, BehaviorTree> _treeDict = new();
    private List<BehaviorTree> _treeList = new();

    public void StartPath(Vector3 tarPos)
    {
        _seeker.StartPath(_entity.position.value, tarPos);
    }

    public void UpdateFSM()
    {
        if (IsActing)
        {
            MoveAgent.StopMove();
        }
        if (!_entity.isDeadState && !_entity.isCamera)
        {
            fsmAgent.SetFSMState(true);
            UnityEngine.Profiling.Profiler.BeginSample("FSMLogic");
            fsmAgent.OnUpdate();
            IsActing = true;
            UnityEngine.Profiling.Profiler.EndSample();
        }
        else
        {
            fsmAgent.SetFSMState(false);
            _uiService.SetMessage(null);
            IsActing = false;
        }
    }

    public void OnDetectCharacter(GameEntity entity)
    {
        if (!_entity.hasDetectedCharacter && !entity.isDeadState)
        {
            _entity.ReplaceDetectedCharacter(entity);
        }
        else
        {
            
        }
    }

    public void TriggerFSM(int type)
    {
        TriggerFSM(_aiCfg.BaseFSM, type);
    }

    public void TriggerFSM(string name, int type)
    {
        fsmAgent.Trigger(name, type);
    }

    /// <summary>
    /// 设置BehaviorTree，已有同名的会被替换
    /// </summary>
    public void SetBTree(string name)
    {
        if (_treeDict.TryGetValue(name, out var tree))
        {
            tree.Reset();
        }
        else
        {
            var wbTree = _vmService.AppendBehaviorTree(name, _entity.gameViewService.service.Model.gameObject);
            if (wbTree != null)
            {
                tree = wbTree.TREE_BUILDER.Build();
            }
            else
            {
                WLogger.Warning("没有加载成功");
            }
        }
    }

    public void TickBTree(string name)
    {
        bTreeAgent.TickTree(name);
    }

    public MoveAgent MoveAgent => moveAgent;
    public FightAgent FightAgent => fightAgent;
    public FSMAgent FSMAgent => fsmAgent;

    public void Destroy()
    {
        fsm.RequestExit();
        
        FSMAgent.Push(fsmAgent);
    }
}
