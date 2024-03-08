using System;
using System.Collections.Generic;
using BaseData;
using CleverCrow.Fluid.BTs.Trees;
using CrashKonijn.Goap.Behaviours;
using Pathfinding;
using UnityEngine;
using WGame.GOAP;

public partial class AiAgentServiceImplementation : IAiAgentService
{
    private GameEntity _entity;
    private Seeker _seeker;

    private readonly IVMService _vmService;
    private readonly ICharacterUIService _uiService;
    private float cooldownTimer;

    private int currentPathIndex;
    private float preSqrDist;

    private readonly MoveAgent moveAgent;
    private readonly FSMAgent fsmAgent;
    private readonly BTreeAgent bTreeAgent;
    private readonly CharacterInitInfo _initInfo;

    private readonly CharAI _aiCfg = null;
    private readonly IFactoryService _factory;

    public AiAgentServiceImplementation(GameEntity entity, Seeker seeker, Vector3[] patrolPoints, bool isDynamicLoad)
    {
        _factory = Contexts.sharedInstance.meta.factoryService.instance;
        _seeker = seeker;
        _entity = entity;
        _aiCfg = entity.characterInfo.value.AICfg;
        _initInfo = entity.characterInfo.value;
        moveAgent = new MoveAgent(this, seeker, entity, patrolPoints, isDynamicLoad);
        _uiService = entity.uIHeadPad.service;
        _vmService = _entity.linkVM.VM.vMService.service;
        fsmAgent = FSMAgent.Get(_vmService);
        fsmAgent.SetFSMConfig(_aiCfg);
        bTreeAgent = BTreeAgent.Get(_vmService);
        InitMethod();
    }

    private void SetMethod(string name, Action<List<Symbol>, Interpreter> callback)
    {
        _vmService.Set(name, Method.Get(name, callback));
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
            UnityEngine.Profiling.Profiler.EndSample();
            IsActing = true;
            // UpdateGOAPBrain();
        }
        else
        {
            fsmAgent.SetFSMState(false);
            // _uiService.SetMessage(null);
            IsActing = false;
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

    public MoveAgent MoveAgent => moveAgent;
    public FSMAgent FSMAgent => fsmAgent;

    private WAgentBrainBase goapBrain;

    public void Initialize()
    {
        // var agent = _entity.gameViewService.service.Model
        //     .GetOrAddComponent<AgentBehaviour>();
        // goapBrain = new BaseAgentBrain(agent, _entity, _factory.GOAPRunner.GetGoapSet("Base"), WDistanceObserver.entity);
        // if (_entity.isCampRed)
        // {
        //     goapBrain.SetEnable(true);
        // }
        // else
        // {
        //     goapBrain.SetEnable(false);
        // }
    }
    
    private void UpdateGOAPBrain()
    {
        goapBrain.OnUpdate();
    }

    public void Destroy()
    {
        FSMAgent.Push(fsmAgent);
        MoveAgent.Dispose();
    }
}
