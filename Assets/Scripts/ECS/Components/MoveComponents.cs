using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
//
[Game, Event(EventTarget.Self)]
public class MoveDirectionComponent : IComponent
{
    public Vector3 value;
}
[Game]
public sealed class MovingComponent : IComponent
{
    
}