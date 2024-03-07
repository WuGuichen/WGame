using System.Collections.Generic;
using UnityEngine;
using WGame.Ability;
using WGame.Res;

public class AbilityStatusCharacter : AbilityStatus
{
    #region pool

    private static Stack<AbilityStatusCharacter> _pool = new();

    public static AbilityStatusCharacter Get(GameEntity entity, AbilityData abilityData)
    {
        if (_pool.Count > 0)
        {
            return _pool.Pop().Init(entity, abilityData);
        }

        return new AbilityStatusCharacter(entity, abilityData);
    }

    public static void Push(AbilityStatusCharacter status)
    {
        status.Reset();
        _pool.Push(status);
    }
    
    private AbilityStatusCharacter(GameEntity entity, AbilityData abilityData)
    {
        Init(entity, abilityData);
    }

    private AbilityStatusCharacter Init(GameEntity entity, AbilityData abilityData)
    {
        _entity = entity;
        Initialize(abilityData);
        return this;
    }
    
    #endregion

    private GameEntity _entity;
    
    protected override void OnStart()
    {
    }

    protected override void OnDurationPlayAnim(EventPlayAnim animData)
    {
    }

    protected override void OnTriggerPlayEffect(EventPlayEffect effectData)
    {
        EffectMgr.LoadEffect(effectData.AddressName, _entity.gameViewService.service.Model, _entity.position.value,
            Quaternion.identity);
    }

    protected override void OnEnd()
    {
    }
}
