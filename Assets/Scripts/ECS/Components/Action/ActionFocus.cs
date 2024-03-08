using Entitas;

[Game]
public class ActionFocus : IComponent
{
    public FocusType type;
    public float area;
}

public enum FocusType
{
    Focus,
    Cancel,
    // Up,
    // Down,
    Left,
    Right,
    Switch,
}
