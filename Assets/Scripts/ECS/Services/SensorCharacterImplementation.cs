using System.Collections.Generic;
using Shapes;
using Unity.Collections;
using UnityEngine;

public class SensorCharacterImplementation : ISensorService
{
    private WDrawer.CircleDrawInfo drawWarning;
    private WDrawer.CircleDrawInfo drawSpotted;
    private WDrawer.CircleDrawInfo drawSensor;
    private WDrawer.CircleDrawInfo drawDetect;
    private readonly Transform _model;
    private Color _color = Color.cyan*0.6f;
    private Color _colorSpotted = Color.blue*0.6f;
    private Color _colorDetectted = Color.red*0.8f;
    private Color _colorDetecting = Color.yellow*0.8f;
    private Color _colorSensor = Color.white*0.6f;
    private Quaternion _rotation = Quaternion.Euler(90, 0, 0);

    private readonly SensorEntity _sensor;
    private readonly GameEntity _character;

    private readonly List<HitInfo> detectList = new();

    private readonly Dictionary<int, float> hatePointDict = new();
    private int maxHateEntityId = 0;
    private float maxHateEntityPoint = float.MinValue;

    #region 距离信息
    private float radiusSpotted = 0;
    private float radiusSpottedSqr = 0;
    private float RadiusSpotted
    {
        get
        {
            if (radiusSpotted != _sensor.detectSpottedRadius.value)
            {
                radiusSpotted = _sensor.detectSpottedRadius.value;
                radiusSpottedSqr = radiusSpotted * radiusSpotted;
            }
            return radiusSpotted;
        }
    }
    private float RadiusSpottedSqr
    {
        get
        {
            if (radiusSpotted != _sensor.detectSpottedRadius.value)
            {
                radiusSpotted = _sensor.detectSpottedRadius.value;
                radiusSpottedSqr = radiusSpotted * radiusSpotted;
            }
            return radiusSpottedSqr;
        }
    }
    private float radiusWarning = 0;
    private float radiusWarningSqr = 0;
    private float RadiusRadiusWarning
    {
        get
        {
            if (radiusWarning != _sensor.detectWarningRadius.value)
            {
                radiusWarning = _sensor.detectWarningRadius.value;
                radiusWarningSqr = radiusWarning * radiusWarning;
            }

            return radiusWarning;
        }
    }
    private float RadiusWarningSqr
    {
        get
        {
            if (radiusWarning != _sensor.detectWarningRadius.value)
            {
                radiusWarning = _sensor.detectWarningRadius.value;
                radiusWarningSqr = radiusWarning * radiusWarning;
            }
            return radiusWarningSqr;
        }
    }

    private int degreeDetect =0;
    private float angleDetectHalf = 0;

    public float AngleDetectHalf
    {
        get
        {
            if (degreeDetect != _sensor.detectCharacterDegree.value)
            {
                degreeDetect = _sensor.detectCharacterDegree.value;
                angleDetectHalf = (degreeDetect >> 1)*Mathf.Deg2Rad;
            }
            return angleDetectHalf;
        }        
    }
    public int DegreeDetect
    {
        get
        {
            if (degreeDetect != _sensor.detectCharacterDegree.value)
            {
                degreeDetect = _sensor.detectCharacterDegree.value;
                angleDetectHalf = (degreeDetect >> 1)*Mathf.Deg2Rad;
            }

            return degreeDetect;
        }
    }
    
    #endregion

    // private readonly NativeHeap<int, Max> hatePointHeap = new();

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

                if (drawDetect != null)
                {
                    WDrawer.Inst.CancelCircle(drawDetect);
                    drawDetect = null;
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
            if (maxHateEntityPoint > 0)
            {
                if (drawDetect == null)
                    drawDetect = WDrawer.Inst.RegisterCircle(_model, _model.position + Vector3.up * 0.2f, _rotation);
                
            var tmp = new CircleInfo()
            {
                center = drawDetect.Drawer.position,
                forward = _model.up,
                isSector = true,
                sectorInitialAngleInDegrees = -90,
            };
            tmp.radius = 1f;
            if (maxHateEntityPoint <= 360)
            {
                tmp.sectorArcLengthInDegrees = maxHateEntityPoint;
                tmp.fillColor = _colorDetecting;
            }
            else
            {
                tmp.sectorArcLengthInDegrees = 360;
                tmp.fillColor = _colorDetectted;
            }
            drawDetect.Info = tmp;
            }
            else
            {
                if (drawDetect != null)
                {
                    WDrawer.Inst.CancelCircle(drawDetect);
                    drawDetect = null;
                }
            }


            isShowDetect = true;
            CircleInfo info = new CircleInfo()
            {
                center = drawWarning.Drawer.position,
                forward = _model.up,
                fillColor = _color,
                radius = RadiusRadiusWarning,
                isSector = true,
                sectorArcLengthInDegrees = DegreeDetect,
                sectorInitialAngleInDegrees = _model.eulerAngles.y - 90 - (DegreeDetect >> 1),
            };

            drawWarning.Info = info;
            drawSpotted.Info = new CircleInfo()
            {
                center = drawSpotted.Drawer.position,
                forward = _model.up,
                fillColor = _colorSpotted,
                radius = RadiusSpotted,
                isSector = true,
                sectorArcLengthInDegrees = DegreeDetect,
                sectorInitialAngleInDegrees = _model.eulerAngles.y - 90 - (DegreeDetect >> 1),
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

    public void UpdateDetect(float deltaTime)
    {
        if (detectList.Count > 0)
        {
            for (int i = 0; i < detectList.Count; i++)
            {
                var point = detectList[i];
                var dir = point.Position - _model.position;
                var normalDir = dir.normalized;
                var angle = normalDir.GetAngle(_model.forward);
                // 距离仇恨值增加
                AddDistanceHatePoint(point.EntityId, point.SqrDist, angle, deltaTime);
            }
            detectList.Clear();
        }
    }

    public void AddDetectTarget(HitInfo hitInfo)
    {
        detectList.Add(hitInfo);
    }

    private void AddDistanceHatePoint(int id, float sqrDist, float angle, float deltaTime)
    {
        if (angle <= AngleDetectHalf)
        {
            if (sqrDist <= RadiusSpottedSqr)
            {
                AddHatePoint(id, 200 * deltaTime);
                return;
            }
            if (sqrDist <= RadiusWarningSqr)
            {
                AddHatePoint(id, 50 * deltaTime);
                return;
            }
        }
        AddHatePoint(id, -150 * deltaTime);
    }

    public void UpdateSensor()
    {
        
    }

    public void AddHatePoint(int id, float value)
    {
        if (hatePointDict.TryGetValue(id, out var point))
        {
            value = point + value;
        }

        if (value < 0)
            value = 0;
        else if (value > 1000)
            value = 1000;
        hatePointDict[id] = value;
        RefreshMaxHateTarget(id, value);
    }

    public void SetHatePoint(int id, float value)
    {
        hatePointDict[id] = value;
        RefreshMaxHateTarget(id, value);
    }

    private void RefreshMaxHateTarget(int id, float value)
    {
        if (id == maxHateEntityId)
        {
            maxHateEntityPoint = value;
        }
        else
        {
            if (value > maxHateEntityPoint)
            {
                maxHateEntityPoint = value;
                maxHateEntityId = id;
            }
        }
    }

    public void Dispose()
    {
        if (drawWarning != null)
        {
            WDrawer.Inst.CancelCircle(drawWarning);
        }
        if (drawSpotted != null)
        {
            WDrawer.Inst.CancelCircle(drawSpotted);
        }
        if (drawSensor != null)
        {
            WDrawer.Inst.CancelCircle(drawSensor);
        }
    }

    public GameEntity Entity => _character;
    public SensorEntity Sensor => _sensor;
}
