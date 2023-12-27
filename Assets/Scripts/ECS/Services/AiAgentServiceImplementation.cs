using System;
using System.Collections;
using System.Collections.Generic;
using BaseData;
using CleverCrow.Fluid.BTs.Trees;
using Pathfinding;
using UnityEngine;
using UnityHFSM;
using WGame.Runtime;

public class AiAgentServiceImplementation : IAiAgentService
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
    private State<int, int> statePatrol;
    private State<int, int> stateChase;
    private State<int, int> stateWait;
    private State<int, int> stateAttack;

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
        bTreeAgent = BTreeAgent.Get(_vmService);
        _motion = _entity.linkMotion.Motion.motionService.service as MotionServiceImplementation;

        // var vm = _entity.linkVM.VM.vMService.service;
        
        InitMethod();
        fsmAgent.SetObject(_aiCfg.BaseFSM);
    }

    private void InitMethod()
    {
        SetMethod("MoveToTarget", MoveToTarget);
        SetMethod("OnTestWaitEnter", OnTestWaitEnter);
        SetMethod("OnTestAttackEnter", OnTestAttackEnter);
        SetMethod("SetPatrolPointTarget", SetPatrolPointTarget);
        SetMethod("NoDetectedCharacter", (list, interpreter) => { interpreter.SetRetrun(!_entity.hasDetectedCharacter);});
        SetMethod("HasDetectedCharacter", (list, interpreter) => { interpreter.SetRetrun(_entity.hasDetectedCharacter);});
        SetMethod("TickBTree", ((list, interpreter) =>
        {
            if(list.Count > 0)
                TickBTree(list[0].Text);
        }));
        
        SetMethod("OnAIPatrolEnter", (list, interpreter) =>
        {
            OnPatrolEnter();
        });
        SetMethod("OnAIPatrolLogic", (list, interpreter) =>
        {
            OnPatrolLogic();
        });
        SetMethod("OnAIPatrolExit", (list, interpreter) =>
        {
            OnPatrolExit();
        });
        
        SetMethod("OnAIChaseEnter", (list, interpreter) =>
        {
            OnChaseEnter();
        });
        
        SetMethod("OnAIChaseLogic", (list, interpreter) =>
        {
            OnChaseLogic();
        });
        
        SetMethod("OnAIChaseExit", (list, interpreter) =>
        {
            OnChaseExit();
        });
        
        SetMethod("OnFightAttackEnter", (list, interpreter) =>
        {
            fightAgent.OnAttackEnter();
        });
        
        SetMethod("OnFightAttackLogic", (list, interpreter) =>
        {
            fightAgent.OnAttackLogic();
        });
        
        SetMethod("OnFightAttackExit", (list, interpreter) =>
        {
            fightAgent.OnAttackExit();
        });
        
        SetMethod("OnAISearchEnter", (list, interpreter) =>
        {
        });
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

    private void InitState()
    {
        statePatrol = new State<int, int>();
        stateChase = new State<int, int>();
        
        stateWait = new State<int, int>();
        stateAttack = new State<int, int>();
    }
    
    private void InitFSM()
    {
        InitState();
        fsm = new StateMachine<int, int, int>();

        var fsmFight = new StateMachine<int, int, int>(needsExitTime: true);
        fsmFight.AddState(StateDefine.Wait,stateWait);
        fsmFight.AddState(StateDefine.Attack, stateAttack);
        fsmFight.AddTransition( new TransitionAfter<int>(StateDefine.Wait, StateDefine.Attack, 2f));
        fsmFight.AddTransition( new TransitionAfter<int>(StateDefine.Attack, StateDefine.Wait, 2f));
        fsmFight.AddExitTransition(StateDefine.Wait);

        fsm.SetStartState(StateDefine.Patrol);

        fsm.AddState(StateDefine.Patrol, statePatrol);
        fsm.AddState(StateDefine.Chase, stateChase);
        fsm.AddState(StateDefine.Fight, fsmFight);
        
        fsm.AddTriggerTransition(StateDefine.SpottedTarget,StateDefine.Patrol, StateDefine.Chase);
        fsm.AddTriggerTransition(StateDefine.LoseTarget, StateDefine.Fight, StateDefine.Chase, _ => _entity.hasDetectedCharacter);
        fsm.AddTriggerTransition(StateDefine.LoseTarget, StateDefine.Fight, StateDefine.Patrol, _ => !_entity.hasDetectedCharacter);
        fsm.AddTransition(StateDefine.Chase, StateDefine.Patrol, _ => !_entity.hasDetectedCharacter);
        fsm.AddTriggerTransition(StateDefine.ReachTarget, StateDefine.Chase, StateDefine.Fight);
        
        fsm.Init();
    }

    private void OnPatrolEnter()
    {
        _entity.ReplaceMovementSpeed(_initInfo.moveSpeed*_initInfo.PatrolMul);
        moveAgent.OnPatrolEnter();
    }
    
    private void OnPatrolLogic()
    {
        moveAgent.OnPatrolLogic();
    }
    
    private void OnPatrolExit()
    {
        _entity.ReplaceMovementSpeed(_initInfo.moveSpeed);
        moveAgent.OnPatrolExit();
    }

    private void OnChaseEnter()
    {
        _entity.ReplaceMovementSpeed(_initInfo.moveSpeed*_initInfo.ChaseMul);
        // _entity.ReplaceFocus(_entity.detectedCharacter.entity.gameViewService.service.FocusPoint);
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
        if(_entity.hasFocus)
            _entity.RemoveFocus();
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
        if (!_entity.isDeadState && !_entity.isCamera)
        {
            UnityEngine.Profiling.Profiler.BeginSample("FSMLogic");
            fsmAgent.OnUpdate();
            IsActing = true;
            UnityEngine.Profiling.Profiler.EndSample();
        }
        else
        {
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
