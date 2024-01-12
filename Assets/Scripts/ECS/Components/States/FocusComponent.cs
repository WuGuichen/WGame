using Entitas;
using UnityEngine;

// [Game]
// public class FocusComponent : IComponent
// {
//     public Transform target;
// }

[Game]
public class KeepTargetDistanceComponent : IComponent
{
    public float value;
}

[Game]
public class TargetPlanarSqrDistance : IComponent
{
    public float value;
}