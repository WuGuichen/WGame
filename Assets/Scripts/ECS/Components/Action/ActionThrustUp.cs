using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game, Event(EventTarget.Self)]
public class ActionThrustUp : IComponent
{
    public float value;
}
