using UnityEngine;

public interface Interactable
{
    public void Interact(GameEntity entity);
    public Vector3 TagPos { get; }
    public int UID { get; }
}
