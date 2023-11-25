using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

[Game, Event(EventTarget.Self)]
public class ActionCameraRotate : IComponent
{
    public Quaternion rot;
}
