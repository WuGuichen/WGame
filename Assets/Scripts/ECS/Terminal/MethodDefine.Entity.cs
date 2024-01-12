/// <summary>
/// 这里写实体方法，用E_前缀
/// </summary>
public partial class MethodDefine
{
    /// <summary>
    /// 设置当前移动速度为初始值*rate%, nil则设置为初始速度
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="rate">百分比值，可以小于0</param>
    /// <param name="reset">是否先重置</param>
    private static void SetMoveSpeedRate(in GameEntity entity, int rate, bool reset = true)
    {
        var _initInfo = entity.characterInfo.value;
        var real = (reset ? _initInfo.moveSpeed : entity.movementSpeed.value) * rate*0.01f;
        entity.ReplaceMovementSpeed(real);
    }

    /// <summary>
    /// 重置移动速度
    /// </summary>
    /// <param name="entity"></param>
    private static void ResetMoveSpeed(in GameEntity entity)
    {
        var _initInfo = entity.characterInfo.value;
        entity.ReplaceMovementSpeed(_initInfo.moveSpeed);
    }

    /// <summary>
    /// 设置当前巡逻点为index或者除当前巡逻点外其他随机点
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="index">巡逻点索引</param>
    private static void SetNewPatrolIndex(in GameEntity entity, int index)
    {
        var moveAgent = entity.aiAgent.service.MoveAgent;
        moveAgent.SetNewPatrolPointIndex(index);
    }
    
    private void BindEntityMethod()
    {
    #region 设置属性(Set)
        SetMethod("E_SetMoveSpeedRate", (list, interpreter) =>
        {
            if (CheckEntity(interpreter.ParseInt(list, 0), out var entity))
                return;
            var rate = interpreter.ParseInt(list, 1, 100);
            var reset = interpreter.ParseBool(list, 2, true);
            SetMoveSpeedRate(entity, rate, reset);
        });
        SetMethod("E_ResetMoveSpeed", (list, interpreter) =>
        {
            if (CheckEntity(interpreter.ParseInt(list, 0), out var entity))
                return;
            ResetMoveSpeed(entity);
        });
        
        // 设置当前巡逻点为index或者除当前巡逻点外其他随机点
        // index->int|nil
        SetMethod("E_SetNewPatrolIndex", (list, interpreter) =>
        {
            if (CheckEntity(interpreter.ParseInt(list, 0), out var entity))
                return;
            SetNewPatrolIndex(entity,interpreter.ParseInt(list, 1, -1));
        });
        
        // 设置探测参数
        // degree->int|nil, 探测角度
        // radiusWarning -> float|nil, 第一层探测半径
        // radiusSpotted -> float|nil, 第二层探测半径
        SetMethod("E_SetDetectParam", (list, interpreter) =>
        {
            if (CheckEntity(interpreter.ParseInt(list, 0), out var _entity))
                return;
            var sensor = _entity.linkSensor.Sensor;
            if (interpreter.TryParseInt(list, 1, out var degree))
                sensor.ReplaceDetectCharacterDegree(degree);
            if (interpreter.TryParseFloat(list, 2, out var radiusWarning))
                sensor.ReplaceDetectWarningRadius(radiusWarning);
            if (interpreter.TryParseFloat(list, 3, out var radiusSpotted))
                sensor.ReplaceDetectSpottedRadius(radiusSpotted);
        });
        
        // 重置探测参数
        SetMethod("E_ResetDetectParam", (list, interpreter) =>
        {
            if (CheckEntity(interpreter.ParseInt(list, 0), out var _entity))
                return;
            var sensor = _entity.linkSensor.Sensor;
            var info = _entity.characterInfo.value;
            sensor.ReplaceDetectCharacterDegree(info.DetectDegree);
            sensor.ReplaceDetectWarningRadius(info.DetectRadius1);
            sensor.ReplaceDetectSpottedRadius(info.DetectRadius2);
        });
        
    #endregion
        
    #region 获取属性(Get)
        // 获取除了curIndex所在巡逻点的其他巡逻点位置, nil则获取除当前位置外的其他随机位置
        // curIndex->int|nil, 巡逻点索引
        // return->vector3, 坐标
        SetMethod("E_GetRandomPatrolPos", (list, interpreter) =>
        {
            if (CheckEntity(interpreter.ParseInt(list, 0), out var _entity))
                return;
            var moveAgent = _entity.aiAgent.service.MoveAgent;
            var index = interpreter.ParseInt(list, 1, moveAgent.CurPatrolIndex);
            var pos = moveAgent.GetOtherPatrolPoint(index);
            interpreter.SetRetrun(pos); 
        });
    #endregion
    
    #region AI行为(Do)
        // 移动到距离巡逻点位index,reachDist距离内
        // index->int|nil, 巡逻点索引
        // reachDist->int|nil, 完成移动距离限制
        // return->bool, 是否完成移动
        SetMethod("E_DoMoveToPatrolPoint", (list, interpreter) =>
        {
            if (CheckEntity(interpreter.ParseInt(list, 0), out var _entity))
                return;
            var moveAgent = _entity.aiAgent.service.MoveAgent;
            int index = interpreter.ParseInt(list, 1, moveAgent.CurPatrolIndex);
            int reachDist = interpreter.ParseInt(list, 2, 20);
            var res = moveAgent.MoveToPatrolPoint(index, reachDist*0.01f);
            interpreter.SetRetrun(res);
        });
        
        // 移动到世界坐标点
        // point->vector3, 目标坐标
        // reachDist->int|nil, 完成移动距离限制
        // return->bool, 是否完成移动
        SetMethod("E_DoMoveToPoint", (list, interpreter) =>
        {
            if (CheckEntity(interpreter.ParseInt(list, 0), out var _entity))
                return;
            var moveAgent = _entity.aiAgent.service.MoveAgent;
            var point = interpreter.ParseVector3(list, 1);
            int reachDist = interpreter.ParseInt(list, 2, 20);
            interpreter.SetRetrun(moveAgent.MoveToPoint(point, reachDist*0.01f));
        });
        
        // 移动到实体位置
        // point->vector3, 目标坐标
        // reachDist->int|nil, 完成移动距离限制
        // return->bool, 是否完成移动
        SetMethod("E_DoMoveToEntity", (list, interpreter) =>
        {
            if (CheckEntity(interpreter.ParseInt(list, 0), out var _entity))
                return;
            var moveAgent = _entity.aiAgent.service.MoveAgent;
            var id = interpreter.ParseInt(list, 0);
            int reachDist = interpreter.ParseInt(list, 1, 20);
            interpreter.SetRetrun(moveAgent.MoveToEntity(id, reachDist*0.01f));
        });
        
    #endregion
    }
}
