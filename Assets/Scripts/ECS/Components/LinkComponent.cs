using Entitas;
using Entitas.VisualDebugging.Unity;

[Game][DontDrawComponent]
public class LinkMotionComponent : IComponent
{
    public MotionEntity Motion;
}

[Motion, Weapon, Ability, VM][DontDrawComponent]
public class LinkCharacterComponent : IComponent
{
    public GameEntity Character;
}

[Game][DontDrawComponent]
public class LinkWeaponComponent : IComponent
{
    public WeaponEntity Weapon;
}

[Game][DontDrawComponent]
public class LinkAbilityComponent : IComponent
{
    public AbilityEntity Ability;
}

[Game]
[DontDrawComponent]
public class LinkVM : IComponent
{
    public VMEntity VM;
}