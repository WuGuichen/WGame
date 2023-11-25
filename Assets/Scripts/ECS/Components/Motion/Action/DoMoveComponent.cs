using Entitas;

[Motion]
public class DoMoveComponent : IComponent
{
    public UnityEngine.Vector3 tarPos;
}

[Motion]
public class DoMoveSpeedComponent : IComponent
{
    public float value;
}

[Motion]
public class DoMoveTypeComponent : IComponent
{
    public DoMoveType type;
}

public enum DoMoveType
{
    Lerp,
}