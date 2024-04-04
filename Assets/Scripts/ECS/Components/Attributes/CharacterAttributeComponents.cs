using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

// [Game]
// public sealed class MovementSpeedComponent : IComponent
// {
//     public float value;
// }
//
// [Game]
// public sealed class RotationSpeedComponent : IComponent
// {
//     public float value;
// }

[Game, Unique]
public sealed class SpawnPointComponent : IComponent 
{
	public Vector3 position;
	public Quaternion rotation;
}
