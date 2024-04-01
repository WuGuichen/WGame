using TWY.Physics;
using UnityEngine;

public interface IGameViewService
{
    int InstanceID { get; }
    float Height { get; }
    float HalfHeight { get; }
    Vector3 GetCameraPos();
    Vector3 LocalizeVectorXY(Vector2 vector, bool isFocus = false);
    Transform Model { get; }
    Transform Head { get; }
    Vector3 FocusPoint { get; }
    Vector2 PlanarPosition { get; }
    GameEntity GetEntity();
    void Destroy();
    void OnDead();
    Vector3 HeadPos { get; }
    void BeFocused(bool value);
    IGameViewService OnInit(GameEntity entity);
    AABBF Bounds { get; }
    float Radius { get; }
    Unity.Mathematics.float3 Size { get; }
}
