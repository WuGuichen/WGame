using CrashKonijn.Goap.Interfaces;
using UnityEngine;

public class EntityTarget : ITarget
{
    private GameEntity entity;
    public GameEntity Entity => entity;

    public EntityTarget(GameEntity entity)
    {
        this.entity = entity;
    }
    public Vector3 Position => entity.position.value;

    public void Move(Vector3 dir)
    {
        entity.ReplaceMoveDirection(entity.gameViewService.service.Model.forward);
    }
}
