using CleverCrow.Fluid.BTs.Trees;
using Pathfinding;
using UnityEngine;
using UnityHFSM;
using WGame.Runtime;

public class MoveAgent
{
    private Seeker _seeker;
    private GameEntity _entity;
    private Vector3[] _patrolPoints;
    private int _curPatrolIndex = 0;
    private Transform _target;
    private Vector3 _tarPos;
    private Vector3 _targetDirRotation;
    private Quaternion _targetDirQuaternion;
    private IAiAgentService _service;
    private Transform _transform;
    private readonly ITimeService _time;

    // state
    private bool isMoving = false;
    private bool needRotate = false;

    private BehaviorTree _onPatrolTree;

    public MoveAgent(IAiAgentService service ,Seeker seeker, GameEntity entity, Vector3[] patrolPoints)
    {
        _service = service;
        _seeker = seeker;
        _entity = entity;
        _transform = _entity.gameViewService.service.Model;
        _patrolPoints = patrolPoints;
        _curPatrolIndex = Random.Range(0, _patrolPoints.Length);
        _tarPos = _patrolPoints[_curPatrolIndex];
        _time = Contexts.sharedInstance.meta.timeService.instance;
        InitTree();
        EventCenter.AddListener(EventDefine.OnBTreeHotUpdate, UpdateTree);
    }

    private void UpdateTree(WEventContext ctx)
    {
        if (ctx.pString == "OnPatrol")
        {
            InitTree();
        }
    }

    private void InitTree()
    {
        var service = _entity.linkVM.VM.vMService.service;
        GameObject obj;
        if (_onPatrolTree != null)
        {
            obj = _onPatrolTree.Root.Owner;
            _onPatrolTree.Reset();
        }
        else
        {
            obj = _entity.gameViewService.service.Model.gameObject;
            // service.Set("MoveToTarget", Method.Get("MoveToTarget", (list, interpreter) =>
            // {
            //     if (list.Count == 0)
            //     {
            //         interpreter.SetRetrun(MoveToTarget());
            //     }
            //     else
            //     {
            //         if(list[0].Type == BaseDefinition.TYPE_FLOAT)
            //             interpreter.SetRetrun(MoveToTarget(list[0].ToFloat(interpreter.Definition)));
            //         else
            //             interpreter.SetRetrun(MoveToTarget(list[0].Value));
            //     }
            // }));
            // service.Set("SetPatrolPointTarget", Method.Get("SetPatrolPointTarget", (list, interpreter) =>
            // {
            //     SetPatrolPointTarget();
            // }));
        }

        // var builder = service.AppendBehaviorTree("OnPatrol", obj);
        // _onPatrolTree = builder.TREE.Build();
        // var motion = _entity.linkMotion.Motion.motionService.service as MotionServiceImplementation;
        // motion.OnPatrolTree = _onPatrolTree;
    }

    public bool MoveToTarget(float sqrReachDist = 0.2f)
    {
        UpdateRotateDir();
        RotateToTarget();
        if (_target != null)
        {
            _tarPos = _target.position;
        }
        var fwd = _transform.forward;
        fwd.y = 0;
        if (Vector3.Angle(fwd, _targetDirRotation) > 0.1f)
        {
            needRotate = true;
            StopMove();
        }

        var pos = _transform.position;
        if (new Vector2(pos.x - _tarPos.x, pos.z - _tarPos.z).sqrMagnitude < sqrReachDist)
        {
            return true;
        }
        else
        {
            if(!isMoving)
                StartMove(_targetDirRotation);
        }

        return false;
    }

    private void RotateToTarget()
    {
        if (!needRotate)
            return;

        var fwd = _transform.forward;
        fwd.y = 0;
        var angle = Vector3.Angle(fwd, _targetDirRotation);
        if (angle < 0.1f)
        {
            // 旋转到位了
            needRotate = false;
            // 开始移动
            UpdateRotateDir();
            StartMove(_targetDirRotation);
        }
        else
        {
            UpdateRotateDir();
            // 旋转目标
            var rot = Quaternion.RotateTowards(_transform.rotation, _targetDirQuaternion
                , _entity.rotationSpeed.value * _entity.animRotateMulti.rate * _time.deltaTime);

            _transform.rotation = rot;
        }
    }

    private void UpdateRotateDir()
    {
        var tar = _tarPos - _transform.position;
        tar.y = 0;
        if (tar == Vector3.zero)
            _targetDirRotation = _transform.forward;
        else
            _targetDirRotation = tar.normalized;
        
        _targetDirQuaternion = Quaternion.LookRotation(_targetDirRotation);
    }

    private void StopMove()
    {
        isMoving = false;
        _entity.ReplaceMoveDirection(Vector3.zero);
    }

    private void StartMove(Vector3 dir)
    {
        isMoving = true;
        _entity.ReplaceMoveDirection(dir);
    }

    public void SetPatrolPointTarget()
    {
        StopMove();
        int len = _patrolPoints.Length;
        if (len < 2)
        {
            WLogger.Error("巡逻点数少于两个");
            return;
        }
        
        int randNum = Random.Range(1, len-1);
        _curPatrolIndex += randNum;
        if (_curPatrolIndex >= len)
            _curPatrolIndex -= len;
        _tarPos = _patrolPoints[_curPatrolIndex];
    }

    public void OnPatrolEnter()
    {
        UpdateRotateDir();
        needRotate = true;
    }

    public void OnPatrolLogic()
    {
        // if(MoveToTarget())
        //     OnReachPatrolPoint();
        if (_entity.hasDetectedCharacter)
        {
            // 发现目标
            _service.TriggerFSM(StateDefine.SpottedTarget);
        }
        UnityEngine.Profiling.Profiler.BeginSample("FSM_BTreeTick");
        _onPatrolTree.Tick();
        UnityEngine.Profiling.Profiler.EndSample();
    }
    
    public void OnPatrolExit()
    {
        
    }

    public void OnChaseEnter()
    {
        RefreshTarget();
    }

    public void RefreshTarget()
    {
        if (_entity.hasDetectedCharacter)
            _target = _entity.detectedCharacter.entity.gameViewService.service.Model;
        else 
            _target = null;
    }
    
    public void OnChaseLogic()
    {
        if (MoveToTarget())
        {
            StopMove();
            _service.TriggerFSM(StateDefine.ReachTarget);
        }
    }
    public void OnChaseExit()
    {
        
    }
    
    public void Dispose()
    {
        EventCenter.AddListener(EventDefine.OnBTreeHotUpdate, UpdateTree);
    }
}
