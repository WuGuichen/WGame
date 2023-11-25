using Entitas;
using Entitas.CodeGeneration.Attributes;

[Meta, Unique]
public class TimeServiceComponent : IComponent
{
    public ITimeService instance;
}
