using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerImplamentation : MonoBehaviour, IRigidbodyService
{
    private CharacterController _controller;
    private float speedVertical;
    private GameEntity entity;
    private ITimeService _timeService;
    
    private LinkedList<CharacterMoveInfo> _moveInfos = new();
    public bool IsCharacterBeMoved => _moveInfos.Count > 0;
    private LinkedList<CharacterMoveInfo> _removeMoveInfos = new();

    public IRigidbodyService OnInit()
    {
        if (_controller != null)
            return this;
        _controller = GetComponent<CharacterController>();
        _timeService = Contexts.sharedInstance.meta.timeService.instance;
        return this;
    }

    public CharacterMoveInfo AddMoveRequest(Vector3 deltaPos, float duration, WEaseType easeType, bool ignoreTimeScale = false)
    {
        var info = new CharacterMoveInfo()
        {
            DeltaPos = deltaPos,
            Duration = duration,
            EaseType = easeType,
            IgnoreCharTimeScale = ignoreTimeScale
        };
        _moveInfos.AddLast(info);
        return info;
    }

    public Vector3 GetSensorBoundsCenter()
    {
        throw new System.NotImplementedException();
    }

    public Vector3 Velocity
    {
        get => _controller.velocity;
        set
        {
            value.y += speedVertical;
            _controller.Move(value);
        } 
    }

    public void MovePosition(Vector3 dir)
    {
        _controller.Move(dir);
    }

    public void OnFixedUpdate(float deltaTime)
    {
        if (entity.groundSensor.intersect)
        {
            speedVertical = 0;
        }
        else
        {
            if (_moveInfos.Count <= 0)
            {
                speedVertical += deltaTime*entity.charGravity.value*deltaTime;
            }
        }
    }

    public void OnUpdate(float deltaTime)
    {
        var anim = entity.linkMotion.Motion.motionService.service.AnimProcessor;
        foreach (var moveInfo in _moveInfos)
        {
            if (moveInfo.OnUpdate(anim, deltaTime, _timeService.TimeDeltaTime))
            {
                _removeMoveInfos.AddLast(moveInfo);
            }
        }

        if (_removeMoveInfos.Count > 0)
        {
            foreach (var info in _removeMoveInfos)
            {
                _moveInfos.Remove(info);
            }

            _removeMoveInfos.Clear();
        }
    }

    public void SetEntity(GameEntity entity)
    {
        this.entity = entity;
    }
}
