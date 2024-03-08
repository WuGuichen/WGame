using UnityEngine;

public interface IRigidbodyService
{
    Vector3 Velocity { get; set; }
    void MovePosition(Vector3 dir);

    void OnFixedUpdate(float deltaTime);
    void SetEntity(GameEntity entity);
    IRigidbodyService OnInit();
}
