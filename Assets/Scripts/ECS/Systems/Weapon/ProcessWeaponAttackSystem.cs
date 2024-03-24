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
            var character = entity.linkCharacter.Character;
            if (character.hasNetAgent)
            {
                if (WNetMgr.Inst.IsSelfClient(character.netAgent.Agent))
                {
                    // 只有本地玩家可以检测自己的攻击是否击中本地位置的敌人
                    entity.weaponWeaponView.service.OnUpdateAttackSensor();
                }
            }
            else
            {
                entity.weaponWeaponView.service.OnUpdateAttackSensor();
            }
        }
    }
}
