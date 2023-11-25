using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

[Input, Unique]
public class LeftMouseComponent : IComponent
{
}

[Input, Unique]
public class RightMouseComponent : IComponent
{
}

[Input]
public class MouseDownComponent : IComponent
{
    public Vector2 position;
}

[Input]
public class MousePositionComponent : IComponent
{
    public Vector2 position;
}

[Input]
public class MouseUpComponent : IComponent
{
    public Vector2 position;
}

[Input, Unique]
public class MoveInputComponent : IComponent
{
    public Vector2 value;
}

[Input, Unique]
public class LookInputComponent : IComponent
{
    public Vector2 value;
}

[Input, Unique]
public class AttackInputComponent : IComponent
{
    public bool value;
}

[Input, Unique]
public class AttackHoldInputComponent : IComponent
{
    public bool value;
}

[Input, Unique]
public class JumpInputComponent : IComponent
{
    public bool value;
}

[Input, Unique]
public class StepInputComponent : IComponent
{
    public bool value;
}

[Input, Unique]
public class FocusInputComponent : IComponent
{
    public bool value;
}

[Input, Unique]
public class RunInputComponent : IComponent
{
    public bool value;
}

[Input, Unique]
public class DefenseComponent : IComponent
{
    public bool value;
}