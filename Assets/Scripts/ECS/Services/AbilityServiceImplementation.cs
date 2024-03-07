using System.Collections.Generic;
using WGame.Ability;

public class AbilityServiceImplementation : IAbility
{
    private readonly GameEntity _entity;
    private AbilityData _curAbility;

    private LinkedList<AbilityStatus> _abilityStatusList = new();

    public AbilityServiceImplementation(GameEntity entity)
    {
        _entity = entity;
    }
    
    public void Do(string name)
    {
        var ability = WAbilityMgr.Inst.GetAbility(name);
        var status = new AbilityStatus();
        status.Initialize(ability);
        _abilityStatusList.AddLast(status);
        WLogger.Print(ability.Name);
    }

    public void Process(float deltaTime)
    {
        var node = _abilityStatusList.First;
        while (node != null)
        {
            var status = node.Value;
            if (status.IsEnable)
            {
                status.Process(deltaTime);
                node = node.Next;
            }
            else
            {
                var next = node.Next;
                _abilityStatusList.Remove(node);
                node = next;
            }
        }
    }
}
