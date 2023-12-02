using Entitas;

[Sensor]
public class SensorCharOpen : IComponent
{
}

[Sensor]
public class SensorCharRadius : IComponent
{
    public float value;
}

[Sensor]
public class DetectCharOpen : IComponent
{
    
}

[Sensor]
public class DetectCharRange : IComponent
{
    public float warning;
    public float spotted;
}

[Sensor]
public class DetectCharDegreeInit : IComponent
{
    public float warning;
    public float spotted;
}

[Sensor]
public class DetectCharDegreeAngle : IComponent
{
    public float warning;
    public float spotted;
}
