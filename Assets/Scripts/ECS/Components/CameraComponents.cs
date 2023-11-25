using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
public class CameraComponent : IComponent
{
}

[Game]
public class CameraDirectionComponent : IComponent
{
    public float angle;
}

[Game]
public class CameraDistanceComponent : IComponent
{
    public float distance;
}

/// <summary>
/// 相机跟随速度
/// </summary>
[Game, Unique]
public sealed class CameraLookSpeedComponent : IComponent
{
    public float value;
}

/// <summary>
/// 相机旋转速度
/// </summary>
[Game, Unique]
public sealed class CameraPivotSpeedComponent : IComponent
{
    public float value;
}

/// <summary>
/// 相机平滑跟随
/// </summary>
[Game, Unique]
public sealed class CameraSmoothTimeComponent : IComponent
{
    public float value;
}