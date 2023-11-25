using UnityEngine;

public interface IRigidbodyService
{
    Vector3 GetSensorBoundsCenter();

    Vector3 Velocity { get; set; }

    void OnFixedUpdate(float deltaTime);
    void SetEntity(GameEntity entity);
    IRigidbodyService OnInit();
}
