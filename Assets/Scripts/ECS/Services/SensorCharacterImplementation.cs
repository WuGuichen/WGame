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
    private Color _colorSensor = Color.white*0.6f;
    private Quaternion _rotation = Quaternion.Euler(90, 0, 0);

    private readonly SensorEntity _sensor;
    private readonly GameEntity _character;

    private bool isShowDetect;
    private bool isShowSensor;

    public SensorCharacterImplementation(Transform model, SensorEntity sensor)
    {
        _sensor = sensor;
        _character = sensor.linkCharacter.Character;
        _model = model;
        isShowDetect = false;
    }

    public void UpdateDetectorDrawer()
    {
        if (_character.isCamera)
        {
            if (isShowDetect)
            {
                if (drawWarning != null)
                {
                    WDrawer.Inst.CancelCircle(drawWarning);
                    drawWarning = null;
                }

                if (drawSpotted != null)
                {
                    WDrawer.Inst.CancelCircle(drawSpotted);
                    drawSpotted = null;
                }
                isShowDetect = false;
            }
        }
        else
        {
            if (drawWarning == null)
                drawWarning = WDrawer.Inst.RegisterCircle(_model, _model.position + Vector3.up * 0.2f, _rotation);
            if (drawSpotted == null)
                drawSpotted = WDrawer.Inst.RegisterCircle(_model, _model.position + Vector3.up * 0.2f, _rotation);

            isShowDetect = true;
            CircleInfo info = new CircleInfo()
            {
                center = drawWarning.Drawer.position,
                forward = _model.up,
                fillColor = _color,
                radius = _sensor.detectWarningRadius.value,
                isSector = true,
                sectorArcLengthInDegrees = _sensor.detectCharacterDegree.value,
                sectorInitialAngleInDegrees = _model.eulerAngles.y - 90 - (_sensor.detectCharacterDegree.value >> 1),
            };

            drawWarning.Info = info;
            drawSpotted.Info = new CircleInfo()
            {
                center = drawSpotted.Drawer.position,
                forward = _model.up,
                fillColor = _colorSpotted,
                radius = _sensor.detectSpottedRadius.value,
                isSector = true,
                sectorArcLengthInDegrees = _sensor.detectCharacterDegree.value,
                sectorInitialAngleInDegrees = _model.eulerAngles.y - 90 - (_sensor.detectCharacterDegree.value >> 1),
            };
        }
    }

    public void UpdateSensorDrawer()
    {
        if (_character.isCamera)
        {
            if (drawSensor == null)
                drawSensor = WDrawer.Inst.RegisterCircle(_model, _model.position + Vector3.up * 0.2f, _rotation);
            isShowSensor = true;
            drawSensor.Info = new CircleInfo()
            {
                center = drawSensor.Drawer.position,
                forward = _model.up,
                fillColor = _colorSensor,
                radius = _sensor.sensorCharRadius.value,
            };
        }
        else
        {
            if (isShowSensor)
            {
                WDrawer.Inst.CancelCircle(drawSensor);
                drawSensor = null;
            }
        }
    }

    public void UpdateDetect()
    {
        
    }

    public void UpdateSensor()
    {
        
    }

    public void Dispose()
    {
        if (drawWarning != null)
        {
            WDrawer.Inst.CancelCircle(drawWarning);
        }
    }
}
