using Entitas;

public class ProcessWeaponAttackSystem : IExecuteSystem
{
    private readonly IGroup<WeaponEntity> _weaponGroup;
    
    public ProcessWeaponAttackSystem(Contexts contexts)
    {
        _weaponGroup = contexts.weapon.GetGroup(WeaponMatcher.OpenSensor);
    }
    
    public void Execute()
    {
        foreach (var entity in _weaponGroup)
        {
            entity.weaponWeaponView.service.OnUpdateAttackSensor();
        }
    }
}
