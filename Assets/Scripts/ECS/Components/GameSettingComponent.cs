using Entitas;
using Entitas.CodeGeneration.Attributes;

[Setting, Unique]
public class GameSettingComponent : IComponent
{
    public GameSetting value;
}
