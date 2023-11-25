using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

[Game][DontGenerate]
public class ActionMove : IComponent
{
    public Vector3 addPos;
    public float duration;
    public int type;
}

[Game]
public class ActionForce : IComponent
{
    public Vector3 dir;
}