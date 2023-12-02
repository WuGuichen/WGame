using Shapes;
using UnityEngine;

public class SensorCharacterImplementation : ISensorService
{
    private WDrawer.CircleDrawInfo drawWarning;
    private WDrawer.CircleDrawInfo drawSpotted;
    private WDrawer.CircleDrawInfo drawSensor;
    private readonly Transform _model;
    private Color _color = Color.cyan*0.6f;
    private Color _colorSpotted = Color.red*0.6f;
    private Color _colorSensor = Color.green*0.6f;
    private Quaternion _rotation = Quaternion.Euler(90, 0, 0);

    private readonly SensorEntity _sensor;

    public SensorCharacterImplementation(Transform model, SensorEntity sensor)
    {
        _sensor = sensor;
        _model = model;
    }
    
    public void UpdateDetectorDrawer()
    {
        if (_sensor.hasDetectCharRange)
        {
            if (drawWarning == null)
                drawWarning = WDrawer.Inst.RegisterCircle(_model, _model.position + Vector3.up * 0.2f, _rotation);
            if(drawSpotted == null)
                drawSpotted = WDrawer.Inst.RegisterCircle(_model, _model.position + Vector3.up * 0.2f, _rotation);

            CircleInfo info = new CircleInfo()
            {
                center = drawWarning.Drawer.position,
                forward = _model.up,
                fillColor = _color,
                radius = _sensor.detectCharRange.warning,
                isSector = true,
                sectorArcLengthInDegrees = _sensor.detectCharDegreeAngle.warning,
                sectorInitialAngleInDegrees = _model.eulerAngles.y - 150 + _sensor.detectCharDegreeInit.warning,
            };
            
            drawWarning.Info = info;
            drawSpotted.Info = new CircleInfo()
            {
                center = drawWarning.Drawer.position,
                forward = _model.up,
                fillColor = _colorSpotted,
                radius = _sensor.detectCharRange.spotted,
                isSector = true,
                sectorArcLengthInDegrees = _sensor.detectCharDegreeAngle.spotted,
                sectorInitialAngleInDegrees = _model.eulerAngles.y - 150 + _sensor.detectCharDegreeInit.spotted,
            };
        }
    }

    public void UpdateSensorDrawer()
    {
        if(drawSensor == null)
            drawSensor = WDrawer.Inst.RegisterCircle(_model, _model.position + Vector3.up * 0.2f, _rotation);
        drawSensor.Info = new CircleInfo()
        {
            center = drawWarning.Drawer.position,
            forward = _model.up,
            fillColor = _colorSensor,
            radius = _sensor.sensorCharRadius.value,
            isSector = true,
            sectorArcLengthInDegrees = 360,
        };
    }

    public void Dispose()
    {
        if (drawWarning != null)
        {
            WDrawer.Inst.CancelCircle(drawWarning);
        }
    }
}
