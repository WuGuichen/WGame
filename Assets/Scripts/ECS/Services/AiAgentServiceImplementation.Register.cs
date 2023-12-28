public partial class AiAgentServiceImplementation
{
    #region 注册方法

    private void InitMethod()
    {
        // 设置参数方法
        // ==================== end ========================
        // AI行为方法
        // ==================== end ========================
        // 获取参数方法
        // ==================== end ========================
        // 其他方法
        SetMethod("TickBTree", ((list, interpreter) => {
            if(list.Count > 0)
                TickBTree(list[0].Text);
        }));
        // ==================== end ========================
        // 旧方法
        // ==================== end ========================
        SetMethod("MoveToTarget", MoveToTarget);
        SetMethod("OnTestWaitEnter", OnTestWaitEnter);
        SetMethod("OnTestAttackEnter", OnTestAttackEnter);
        SetMethod("SetPatrolPointTarget", SetPatrolPointTarget);
        SetMethod("NoDetectedCharacter", (list, interpreter) => { interpreter.SetRetrun(!_entity.hasDetectedCharacter);});
        SetMethod("HasDetectedCharacter", (list, interpreter) => { interpreter.SetRetrun(_entity.hasDetectedCharacter);});
        
    }
    
    #endregion

}
