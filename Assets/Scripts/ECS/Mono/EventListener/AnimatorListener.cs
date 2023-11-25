using Entitas;
using UnityEngine;

public class AnimatorListener : MonoBehaviour, IMoveDirectionListener, IEventListener
{
    private readonly int ForwardId = Animator.StringToHash("Forward");
    private readonly int RightId = Animator.StringToHash("Right");
    private readonly int PlaySpeed = Animator.StringToHash("PlaySpeed");
    
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnMoveDirection(GameEntity entity, Vector3 value)
    {
        if (entity.hasFocus)
        {
            anim.SetFloat(ForwardId, value.z);
            anim.SetFloat(RightId, value.x);
        }
        else
        {
            anim.SetFloat(ForwardId, value.magnitude);
        }
    }

    public void RegisterEventListener(Contexts contexts, IEntity entity)
    {
        ((GameEntity)entity).AddMoveDirectionListener(this);
    }
}
