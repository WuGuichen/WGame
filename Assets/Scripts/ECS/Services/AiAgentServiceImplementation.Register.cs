// 要按规范添加方法
public partial class AiAgentServiceImplementation
{
    private void InitMethod()
    {
    #region 设置属性(Set)
        // 设置当前移动速度为初始值*rate%, nil则设置为初始速度
        // rate->int|nil, 百分比值，可以小于0
        SetMethod("SetMoveSpeedRate", (list, interpreter) =>
        {
            var real = _initInfo.moveSpeed + interpreter.ParseInt(list, 0, 0) * 0.01f;
            _entity.ReplaceMovementSpeed(real);
        });
        // 设置当前移动速度为初始+value%, nil则设置为初始速度
        // value->int|nil, 百分比值，可以小于0
        SetMethod("SetMoveSpeedAddValue", (list, interpreter) =>
        {
            var value = interpreter.ParseInt(list, 0, 0);
            var real = _initInfo.moveSpeed + value * 0.01f;
            _entity.ReplaceMovementSpeed(real);
        });
    #endregion
        
    #region 获取属性(Get)
        // 获取除了curIndex所在巡逻点的其他巡逻点位置, nil则获取除当前位置外的其他随机位置
        // curIndex->int|nil, 巡逻点索引
        // return->vector3, 坐标
        SetMethod("GetRandomPatrolPos", (list, interpreter) =>
        {
            int index = interpreter.ParseInt(list, 0, moveAgent.CurPatrolIndex);
            var pos = moveAgent.GetOtherPatrolPoint(ref index);
            interpreter.SetRetrun(pos); 
        });
    #endregion
    
    #region AI行为(Do)
        // 移动到距离巡逻点位index,reachDist距离内
        // index->int|nil, 巡逻点索引
        // reachDist->int|nil, 完成移动距离限制
        // return->bool, 是否完成移动
        SetMethod("DoMoveToPatrolPoint", (list, interpreter) =>
        {
            int index = interpreter.ParseInt(list, 0, moveAgent.CurPatrolIndex);
            int reachDist = interpreter.ParseInt(list, 1, 20);
            var res = moveAgent.MoveToPatrolPoint(index, reachDist*0.01f);
            interpreter.SetRetrun(res);
        });
        
        // 移动到世界坐标点
        // point->vector3, 目标坐标
        // reachDist->int|nil, 完成移动距离限制
        // return->bool, 是否完成移动
        SetMethod("DoMoveToPoint", (list, interpreter) =>
        {
            var point = interpreter.ParseVector3(list, 0);
            int reachDist = interpreter.ParseInt(list, 1, 20);
            interpreter.SetRetrun(moveAgent.MoveToPoint(point, reachDist*0.01f));
        });
    #endregion
        
    #region 其他方法(Get)
        SetMethod("TickBTree", ((list, interpreter) => {
            if(list.Count > 0)
                TickBTree(list[0].Text);
        }));
    #endregion
    
    #region 旧方法
        SetMethod("MoveToTarget", MoveToTarget);
        SetMethod("OnTestWaitEnter", OnTestWaitEnter);
        SetMethod("OnTestAttackEnter", OnTestAttackEnter);
        SetMethod("SetPatrolPointTarget", SetPatrolPointTarget);
        SetMethod("NoDetectedCharacter", (list, interpreter) => { interpreter.SetRetrun(!_entity.hasDetectedCharacter);});
        SetMethod("HasDetectedCharacter", (list, interpreter) => { interpreter.SetRetrun(_entity.hasDetectedCharacter);});
    #endregion
    }
    

}
