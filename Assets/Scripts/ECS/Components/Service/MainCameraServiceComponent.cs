using Entitas;
using Entitas.CodeGeneration.Attributes;

[Meta, Unique]
public class MainCameraServiceComponent : IComponent
{
    public ICameraService service;
}
