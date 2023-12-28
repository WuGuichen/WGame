using CleverCrow.Fluid.BTs.Trees;
using Pathfinding;
using UnityEngine;
using WGame.Runtime;

public class MoveAgent
{
    private Seeker _seeker;
    private GameEntity _entity;
    private Vector3[] _patrolPoints;
    private int _curPatrolIndex = 0;
    public int CurPatrolIndex
    {
        get => _curPatrolIndex;
        set
        {
            _curPatrolIndex = value; 
            if(_vmService != null)
                _vmService.Set("E_CUR_PATROL_INDEX", value);
        }
    }

    private Transform _target;
    private Vector3 _tarPos;
    private Vector3 _targetDirRotation;
    private Quaternion _targetDirQuaternion;
    private IAiAgentService _service;
    private IVMService _vmService;
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
        if(entity.hasLinkVM)
            _vmService = entity.linkVM.VM.vMService.service;
        _transform = _entity.gameViewService.service.Model;
        _patrolPoints = patrolPoints;
        CurPatrolIndex = Random.Range(0, _patrolPoints.Length);
        _tarPos = _patrolPoints[CurPatrolIndex];
        _time = Contexts.sharedInstance.meta.timeService.instance;
        EventCenter.AddListener(EventDefine.OnBTreeHotUpdate, UpdateTree);
    }

    private void UpdateTree(WEventContext ctx)
    {
        if (ctx.pString == "OnPatrol")
        {
        }
    }

    public bool MoveToPoint(Vector3 point, float reachDist = 0.2f)
    {
        UpdateRotateDir();
        RotateToTarget();
        _tarPos = point;
        var fwd = _transform.forward;
        fwd.y = 0;
        if (Vector3.Angle(fwd, _targetDirRotation) > 0.1f)
        {
            needRotate = true;
            StopMove();
        }

        var pos = _transform.position;
        if (new Vector2(pos.x - _tarPos.x, pos.z - _tarPos.z).sqrMagnitude < (reachDist*reachDist))
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
                , _entity.rotationSpeed.value * _entity.animRotateMulti.rate * _time.DeltaTime);

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

    public void StopMove()
    {
        if (isMoving)
        {
            isMoving = false;
            _entity.ReplaceMoveDirection(Vector3.zero);
        }
    }

    private void StartMove(Vector3 dir)
    {
        isMoving = true;
        _entity.ReplaceMoveDirection(dir);
    }

    public bool MoveToPatrolPoint(int index, float reachDist = 0.2f)
    {
        if (index > _patrolPoints.Length)
        {
            WLogger.Error("超出巡逻点范围：" + index +"要限制在：" +(_patrolPoints.Length-1));
            return false;
        }

        CurPatrolIndex = index;
        return MoveToPoint(_patrolPoints[index], reachDist);
    }
    
    public Vector3 GetOtherPatrolPoint(int curIndex)
    {
        int len = _patrolPoints.Length;
        if (len < 2)
        {
            WLogger.Error("巡逻点数少于两个");
            return Vector3.zero;
        }
        int randNum = Random.Range(1, len-1);
        curIndex += randNum;
        if (curIndex >= len)
            curIndex -= len;
        CurPatrolIndex = curIndex;
        var res = _patrolPoints[CurPatrolIndex];
        return res;
    }

    public void SetNewPatrolPointIndex(int index = -1)
    {
        int curIndex = index;
        if (curIndex < 0)
        {
            curIndex = CurPatrolIndex;
        }
        else
        {
            CurPatrolIndex = index;
            return;
        }
        int len = _patrolPoints.Length;
        if (len < 2)
        {
            WLogger.Error("巡逻点数少于两个");
            return;
        }
        int randNum = Random.Range(1, len-1);
        curIndex += randNum;
        if (curIndex >= len)
            curIndex -= len;
        CurPatrolIndex = curIndex;
    }
    public void SetPatrolPointTarget()
    {
        StopMove();
        _tarPos = GetOtherPatrolPoint(_curPatrolIndex);
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
