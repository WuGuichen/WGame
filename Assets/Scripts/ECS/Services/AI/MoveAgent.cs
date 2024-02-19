using CleverCrow.Fluid.BTs.Trees;
using Oddworm.Framework;
using Pathfinding;
using UnityEngine;

public class MoveAgent
{
    private Seeker _seeker;
    private GameEntity _entity;
    private Vector3[] _patrolPoints;
    private int _curPatrolIndex = 0;
    private CharacterInitInfo _initInfo;
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
    private IVMService _vmService;
    private Transform _transform;
    private readonly ITimeService _time;
    private Vector3 _originPos;

    // state
    private bool isMoving = false;

    private BehaviorTree _onPatrolTree;

    public MoveAgent(IAiAgentService service ,Seeker seeker, GameEntity entity, Vector3[] patrolPoints, bool isDynamic)
    {
        _initInfo = entity.characterInfo.value;
        _seeker = seeker;
        _entity = entity;
        if(entity.hasLinkVM)
            _vmService = entity.linkVM.VM.vMService.service;
        _transform = _entity.gameViewService.service.Model;
        _patrolPoints = new Vector3[patrolPoints.Length];
        // 不考虑y轴
        _originPos = entity.position.value;
        if (isDynamic)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                _patrolPoints[i] = patrolPoints[i] + _originPos;
            }
        }
        else
        {
            _patrolPoints = patrolPoints;
        }
        CurPatrolIndex = Random.Range(0, _patrolPoints.Length);
        _tarPos = _patrolPoints[CurPatrolIndex];
        _time = Contexts.sharedInstance.meta.timeService.instance;
    }

    public bool MoveToPoint(Vector3 point, float reachDist = 0.2f)
    {
        return MoveToTargetPos(point, reachDist);
    }

    /// <summary>
    /// 移动到实体的目标半径
    /// </summary>
    /// <param name="id">实体id</param>
    /// <param name="reachDist">目标半径</param>
    /// <param name="threshold">目标半径容差</param>
    /// <returns>是否到达</returns>
    public bool MoveToEntity(int id, float reachDist = 0.2f, float threshold = 1f)
    {
        // 不存在目标
        if (id < 0)
        {
            WLogger.Info("空目标："+id);
            return false;
        }
        var tarEntity = EntityUtils.GetGameEntity(id);
        if (tarEntity == null)
            return false;
        _tarPos = tarEntity.position.value;
        var dist = DetectMgr.Inst.GetDistance(id, _entity.instanceID.ID);
        var angle = DetectMgr.Inst.GetAngle(_entity.instanceID.ID, id);   
        // 角度不满足
        var needRotate = angle > 5f;
        // 距离满足
        var isReached = MoveToTargetPos(_tarPos, dist, reachDist, threshold);
        if (isReached)
        {
            if (needRotate)
            {
                _entity.isNotMove = true;
                return false;
            }
            _entity.isNotMove = false;
            return true;
        }

        _entity.isNotMove = false;
        return false;
    }

    private bool MoveToTargetPos(Vector3 pos, float reachDist, float threshold = 0.5f)
    {
        var tmp = _entity.position.value - pos;
        tmp.y = 0;
        _entity.isNotMove = false;
        return MoveToTargetPos(pos, tmp.magnitude, reachDist, threshold);
    }

    // private void MoveToTargetPos(Vector3 pos)
    // {
    //     DbgDraw.Cube(pos, Quaternion.identity, new Vector3(0.2f, 0.2f, 0.2f), Color.red);
    //     var tar = pos - _entity.position.value;
    //     tar.y = 0;
    //     
    //     StartMove(tar.normalized);
    // }
    // ReSharper disable Unity.PerformanceAnalysis
    private bool MoveToTargetPos(Vector3 pos, float dist, float reachDist, float threshold = 0.5f)
    {
        DbgDraw.Cube(pos, Quaternion.identity, new Vector3(0.2f, 0.2f, 0.2f), Color.white);
        bool reverse = reachDist > dist;
        reachDist += (reverse ? -threshold : threshold);
        
        // UpdateRotateDir
        var tar = pos - _entity.position.value;
        tar.y = 0;
        
        StartMove(tar.normalized);
        if (reverse)
        {
            if (dist > reachDist)
            {
                return true;
            }
        }
        else
        {
            if (dist < reachDist)
            {
                return true;
            }
        }

        return false;
    }

    public void StopMove()
    {
        if (isMoving)
        {
            isMoving = false;
            _entity.ReplaceMoveDirection(Vector3.zero);
        }
    }

    private void StartRotate(Quaternion quaternion)
    {
        // _entity.ReplaceRotate
    }
    private void StartMove(Vector3 dir)
    {
        isMoving = true;
        _entity.ReplaceMoveDirection(dir);
    }

    /// <summary>
    /// 移动到指定巡逻点
    /// </summary>
    /// <param name="index">巡逻点下标</param>
    /// <param name="reachDist">判定为到达的距离</param>
    /// <returns></returns>
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
    
    /// <summary>
    /// 获取除当前巡逻点外的巡逻点
    /// </summary>
    /// <param name="curIndex"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 随机设置新的巡逻点
    /// </summary>
    /// <param name="index"></param>
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

    /// <summary>
    /// 设置移速倍率
    /// </summary>
    /// <param name="rate"></param>
    /// <param name="reset"></param>
    public void SetMoveSpeedRate(float rate = 1f, bool reset = true)
    {
        var real = (reset ? _initInfo.moveSpeed : _entity.movementSpeed.value) * rate;
        _entity.ReplaceMovementSpeed(real);
    }

    public void Dispose()
    {
    }
}
