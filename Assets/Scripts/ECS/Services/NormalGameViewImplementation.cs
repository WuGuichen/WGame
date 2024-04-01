using TWY.Physics;
using Unity.Mathematics;
using UnityEngine;

public class EntityExtraGameView
{
    private IGameViewService _gameViewService;
    public IGameViewService GameViewService => _gameViewService;

    public EntityExtraGameView(Transform trans)
    {
        if (trans.TryGetComponent<IGameViewService>(out var gameView))
        {
            _gameViewService = gameView;
        }
        else
        {
            _gameViewService = trans.gameObject.AddComponent<NormalGameViewImplementation>();
        }
    }

    public void Refresh(GameEntity entity)
    {
        _gameViewService.OnInit(entity);
    }

    public void Dispose()
    {
        _gameViewService.Destroy();
        _gameViewService = null;
    }
}

public class NormalGameViewImplementation : MonoBehaviour, IGameViewService
{
    private GameEntity _entity;
    private Transform _transform;

    public int InstanceID => _entity.instanceID.ID;
    public float Height { get; }
    public float HalfHeight { get; }
    public Vector3 GetCameraPos()
    {
        throw new System.NotImplementedException();
    }

    public void SetMoveVelocity(Vector3 dir)
    {
        _transform.Translate(dir);
    }

    public Vector3 LocalizeVectorXY(Vector2 vector, bool isFocus = false)
    {
        throw new System.NotImplementedException();
    }

    public Transform Model { get; }
    public Transform Head { get; }
    public Vector3 FocusPoint { get; }
    public Vector2 PlanarPosition { get; }
    public GameEntity GetEntity()
    {
        return _entity;
    }

    public void Destroy()
    {
        _entity = null;
    }

    public void OnDead()
    {
        throw new System.NotImplementedException();
    }

    public Vector3 HeadPos { get; }
    public void BeFocused(bool value)
    {
        throw new System.NotImplementedException();
    }

    public IGameViewService OnInit(GameEntity entity)
    {
        _entity = entity;
        return this;
    }

    public AABBF Bounds { get; }
    public float Radius { get; }
    public float3 Size { get; }
}
