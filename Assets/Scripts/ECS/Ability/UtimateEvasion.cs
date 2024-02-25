using UnityEngine;

public class UtimateEvasion
{
    public SensorMono sensorMono;

    public UtimateEvasion(SensorMono sensor)
    {
        sensorMono = sensor;
    }

    public void SetPosition(Vector3 position)
    {
        sensorMono.Trans.position = position;
    }

    public void Disable()
    {
        sensorMono.Collider.enabled = false;
    }
    public void Enable()
    {
        sensorMono.Collider.enabled = true;
        sensorMono.RefreshPosition();
    }
    
}
