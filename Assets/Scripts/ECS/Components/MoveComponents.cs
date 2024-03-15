using Entitas;
using UnityEngine;
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