using System.Collections.Generic;
using Entitas;
using UnityEngine;
using Mathf = UnityEngine.Mathf;
using Vector3 = UnityEngine.Vector3;

public class UpdateMoveDirectionSystem : IExecuteSystem
{
    private InputContext _inputContext;
    private float curDup;
    private float curDright;
    private readonly IGroup<GameEntity> _movingGroup;
    
    public UpdateMoveDirectionSystem(Contexts contexts)
    {
        _inputContext = contexts.input;
        _movingGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Moveable));
    }

    public void Execute()
    {
        foreach (var entity in _movingGroup)
        {
            UpdateDirection(entity);
        }
    }

    void UpdateDirection(GameEntity entity)
    {
        if (entity.hasAiAgent)
        {
            if (entity.aiAgent.service.IsActing)
            {
                entity.isMoving = entity.moveDirection.value != Vector3.zero;
                return;
            }
        }
        if (entity.isMoveable)
        {
            var move = _inputContext.moveInput.value;

            float tarDup = move.y;
            float tarDright = move.x;

            move.y = Mathf.SmoothDamp(move.y, tarDup, ref curDup, 0.1f);
            move.x = Mathf.SmoothDamp(move.x, tarDright, ref curDright, 0.1f);

            var moveDirection = entity.gameViewService.service.LocalizeVectorXY(move, entity.hasFocusEntity);
            entity.ReplaceMoveDirection(moveDirection);
            entity.isMoving = moveDirection != Vector3.zero;
            if(entity.isMoving)
                entity.ReplaceSignalLocalMotion(1f);
        }
        else
        {
            entity.ReplaceMoveDirection(Vector3.zero);
            entity.isMoving = false;
        }
    }

}
