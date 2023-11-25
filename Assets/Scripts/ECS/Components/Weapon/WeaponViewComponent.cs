using Entitas;

namespace Weapon
{
    [Weapon]
    public class WeaponViewComponent : IComponent
    {
        public IWeaponViewService service;
    }
}