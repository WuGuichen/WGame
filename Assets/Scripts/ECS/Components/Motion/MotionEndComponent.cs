using Entitas;
using Entitas.CodeGeneration.Attributes;

[Motion, Event(EventTarget.Self)]
public class MotionEndComponent : IComponent
{
    public int UID;
}
