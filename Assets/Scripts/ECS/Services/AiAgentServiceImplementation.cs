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

    private readonly MotionServiceImplementation _motion;
    private Dictionary<string, WFSM> _stateMachines = new();
    private List<StateMachine<int, int, int>> _fsmList = new();
    private List<WFSM> _fsmListWaitRemove =new();

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
        _motion = _entity.linkMotion.Motion.motionService.service as MotionServiceImplementation;
        _treeDict = new Dictionary<string, BehaviorTree>();

        // var vm = _entity.linkVM.VM.vMService.service;
        
        InitMethod();
        // InitFSM();
        
        
        
        EventCenter.AddListener(EventDefine.OnFSMHotUpdate, RefreshFSM);
    }

    private void InitMethod()
    {
        SetMethod("MoveToTarget", MoveToTarget);
        SetMethod("OnTestWaitEnter", OnTestWaitEnter);
        SetMethod("OnTestAttackEnter", OnTestAttackEnter);
        SetMethod("SetPatrolPointTarget", SetPatrolPointTarget);
        SetMethod("NoDetectedCharacter", (list, interpreter) => { interpreter.SetRetrun(!_entity.hasDetectedCharacter);});
        SetMethod("HasDetectedCharacter", (list, interpreter) => { interpreter.SetRetrun(_entity.hasDetectedCharacter);});
        
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

    private void RefreshFSM(WEventContext context)
    {
        SetFSM(context.pString, true);
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
        _entity.ReplaceFocus(_entity.detectedCharacter.entity.gameViewService.service.FocusPoint);
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

    public void StartPath(Vector3 tarPos)
    {
        _seeker.StartPath(_entity.gameViewService.service.Position, tarPos);
    }

    public void UpdateFSM()
    {
        if (!_entity.isDeadState && !_entity.isCamera)
        {
            for (int i = 0; i < _fsmList.Count; i++)
            {
                _fsmList[i].OnLogic();
                _uiService.SetMessage("state: " + ConstDefine.GetStateDefineName(_fsmList[i].ActiveStateName));
            }
            IsActing = true;
        }
        else
        {
            _uiService.SetMessage(null);
            IsActing = false;
        }

        if (_fsmListWaitRemove.Count > 0)
        {
            for (int i = 0; i < _fsmListWaitRemove.Count; i++)
            {
                // todo
                _fsmList.Remove(_fsmListWaitRemove[i].FSM);
            }
            _fsmListWaitRemove.Clear();
        }
    }

    public void OnDetectCharacter(GameEntity entity)
    {
        if (!_entity.hasDetectedCharacter && !entity.isDeadState)
        {
            _entity.ReplaceDetectedCharacter(entity);
        }
    }

    public void TriggerFSM(int type)
    {
        TriggerFSM(_aiCfg.BaseFSM, type);
    }

    public void TriggerFSM(string name, int type)
    {
        if (_stateMachines.TryGetValue(name, out var fsm))
        {
            fsm.FSM.Trigger(type);
        }
    }


    public void SetFSM(string name, bool isRefresh = false)
    {
        if (_stateMachines.TryGetValue(name, out var fsm))
        {
            fsm.FSM.RequestExit();
            _fsmList.Remove(fsm.FSM);
            if (isRefresh)
            {
                _vmService.ReleaseWObject(fsm);
                fsm = _vmService.GetFSM(name);
            }
        }

        if (!isRefresh)
        {
            fsm = _vmService.GetFSM(name);
        }

        if (fsm != null)
        {
            _stateMachines[name] = fsm;
            fsm.FSM.Init();
            _fsmList.Add(fsm.FSM);
        }
    }

    private Dictionary<string, BehaviorTree> _treeDict;
    public void SetBTree(string name)
    {
        if (_treeDict.TryGetValue(name, out var tree))
        {
            tree.Reset();
        }
    }

    public void UpdateBTree(string name)
    {
        throw new NotImplementedException();
    }

    public void RemoveFSM(string name)
    {
        if (_stateMachines.TryGetValue(name, out var fsm))
        {
            if(_fsmListWaitRemove.Contains(fsm)== false)
                _fsmListWaitRemove.Add(fsm);
        }
    }

    public StateMachine<int, int, int> GetFSM(string name)
    {
        if (_stateMachines.TryGetValue(name, out var fsm))
        {
            return fsm.FSM;
        }

        return null;
    }

    public MoveAgent MoveAgent => moveAgent;
    public FightAgent FightAgent => fightAgent;

    public void Dispose()
    {
        fsm.RequestExit();
        for (int i = 0; i < _fsmList.Count; i++)
        {
            _fsmList[i].RequestExit();
        }

        foreach (var kv in _stateMachines)
        {
            var wfsm = kv.Value;
            _vmService.ReleaseWObject(wfsm);
        }
    }
}
