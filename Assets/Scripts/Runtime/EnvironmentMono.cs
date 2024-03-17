using UnityEngine;

public class EnvironmentMono : MonoBehaviour
{
    [SerializeField]
    private Transform obstacle;
    public Transform Obstacle => obstacle;
    [SerializeField]
    private Transform items;
    public Transform Items => items;
    [SerializeField]
    private Transform characters;
    public Transform Characters => characters;
    [SerializeField]
    private Transform characterRoot;
    public Transform CharacterRoot => characterRoot;
}
