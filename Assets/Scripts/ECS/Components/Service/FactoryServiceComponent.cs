using Entitas;
using Entitas.CodeGeneration.Attributes;

[Meta][Unique]
public class FactoryServiceComponent : IComponent
{
    public IFactoryService instance;
}
