using UnityEngine;

public interface Interactable
{
    public void Interact();
    public Vector3 TagPos { get; }
    public int UID { get; }
}
