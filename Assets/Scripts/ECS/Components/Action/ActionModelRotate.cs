using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

[Game, Event(EventTarget.Self)]
public class ActionModelRotate : IComponent
{
    public Quaternion rot;
}
