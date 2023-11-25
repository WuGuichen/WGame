using Entitas;
using UnityEngine;

public class UpdateWeaponColliderSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _entities;
    public UpdateWeaponColliderSystem(Contexts contexts)
    {
        _entities = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.WeaponService));
    }

    public void Execute()
    {
        foreach (var entity in _entities)
        {
            entity.weaponService.service.UpdateCollider();
        }
    }
}
