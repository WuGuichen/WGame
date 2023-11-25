using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game, Weapon]
public class EntityIDComponent : IComponent
{
    [PrimaryEntityIndex]
    public int id;
}
