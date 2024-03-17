using Entitas;
using UnityEngine;
using WGame.Ability;

//
[Game, Sensor]
public class MoveDirectionComponent : IComponent
{
    public Vector3 value;
}
[Game]
public sealed class MovingComponent : IComponent
{
    
}

[Sensor]
public sealed class MoveInfoComponent : IComponent
{
    public EntityMoveInfo value;
}
