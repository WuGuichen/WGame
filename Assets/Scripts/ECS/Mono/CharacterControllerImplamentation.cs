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
        Physics.autoSyncTransforms = true;
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
            _controller.SimpleMove(value);
        } 
    }

    public void OnFixedUpdate(float deltaTime)
    {
        if (_controller.isGrounded)
        {
            speedVertical = 0;
        }
        else
        {
            speedVertical += deltaTime*entity.charGravity.value;
        }
    }

    public void SetEntity(GameEntity entity)
    {
        this.entity = entity;
    }
}
