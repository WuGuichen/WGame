using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerImplamentation : MonoBehaviour, IRigidbodyService
{
    private CharacterController _controller;
    private float speedVertical;
    private GameEntity entity;

    public IRigidbodyService OnInit()
    {
        if (_controller != null)
            return this;
        _controller = GetComponent<CharacterController>();
        return this;
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
        WLogger.Print(dir);
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
            speedVertical += deltaTime*entity.charGravity.value*deltaTime;
        }
    }

    public void SetEntity(GameEntity entity)
    {
        this.entity = entity;
    }
}
