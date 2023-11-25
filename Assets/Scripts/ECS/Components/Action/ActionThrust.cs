using Entitas;

[Game]
public class ActionThrust : IComponent
{
    public UnityEngine.Vector3[] targetPositions;
    public float[] durations;
    public int[] types;
}
