using UnityEngine;

public class ParryAttack
{
    private readonly GameEntity _entity;
    private readonly IGameViewService _gameView;
    public bool IsEnable { get; set; }
    
    public ParryAttack(GameEntity entity)
    {
        _entity = entity;
        _gameView = _entity.gameViewService.service;
    }

    public bool Parry(Weapon.ContactInfo info)
    {
        if (IsEnable)
        {
            var attackerPos = info.entity.gameViewService.service.PlanarPosition;
            var myPos = _gameView.PlanarPosition;
            var hitDir = (myPos - attackerPos).normalized;
            var isFwd = Vector3.Dot(_gameView.Model.forward,
                info.pos - _entity.position.value) > 0;
            if (isFwd)
            {
                var isRight = Vector3.Dot(_gameView.Model.right,
                    info.pos - _entity.position.value) > 0;
                return true;
            }
        }

        return false;
    }
}
