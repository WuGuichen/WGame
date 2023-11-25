using Entitas;
using Entitas.CodeGeneration.Attributes;
using WGame.Res;

[Meta][DontGenerate]
public class ResourcesServiceComponent : IComponent
{
    // public IResourcesService service;
}

[Meta, Unique]
public class AssetsServiceComponent : IComponent
{
    public IAssetService service;
}
