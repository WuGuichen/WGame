using UnityEngine;

public interface IGameViewService
{
    int InstanceID { get; }
    float Height { get; }
    Vector3 GetCameraPos();
    Vector3 LocalizeVectorXY(Vector2 vector, bool isFocus = false);
    Transform Model { get; }
    Vector3 Position { get; }
    Vector2 PlanarPosition { get; }
    GameEntity GetEntity();
    void Destroy();
    void OnDead();
    Vector3 HeadPos { get; }
    void BeFocused(bool value);
    void Thrust();
    void OnUpdateMove(float deltaTime);
    Transform FocusPoint { get; }
    IGameViewService OnInit(int instID);
    int GetHashCode();
}
