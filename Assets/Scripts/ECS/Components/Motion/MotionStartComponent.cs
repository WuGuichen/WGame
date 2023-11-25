using Entitas;
using Entitas.CodeGeneration.Attributes;

[Motion, Event(EventTarget.Self)]
public class MotionStartComponent : IComponent
{
    public int UID;
}
