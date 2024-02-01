using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class DetectorCharacterImplementation : IDetectorService
{
    private readonly Transform _model;
    private Color[] _colorList;
    private readonly DetectorDrawer detectorDrawer;
    private readonly HatePointInfo hateInfo;

    private readonly SensorEntity _sensor;
    private readonly GameEntity _character;
    private readonly IVMService _vmService;

    private readonly List<HitInfo> detectList = new();

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
    private float RadiusWarning
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

    public DetectorCharacterImplementation(Transform model, SensorEntity sensor)
    {
        _sensor = sensor;
        _character = sensor.linkCharacter.Character;
        _model = model;
        detectorDrawer = new DetectorDrawer();
        hateInfo = DetectMgr.Inst.RegisterHatePoint(_character.instanceID.ID);
        _vmService = _character.linkVM.VM.vMService.service;
        var vm = _vmService;
        var info = hateInfo;
        hateInfo.RegisterOnHateRankChanged((() =>
        {
            vm.Set("E_MAX_HATE_RANK", info.MaxHateEntityRank);
            vm.Set("E_MAX_HATE_ENTITY", info.MaxHateEntityId);
        }), () =>
        {
            vm.Set("E_MAX_HATE_POINT", info.MaxHateEntityPoint);
        });

        _colorList = new Color[]
        {
            Color.white*0.4f,
            Color.yellow*0.4f,
            Color.magenta*0.4f,
            Color.red*0.4f,
        };
    }

    public void UpdateSensorDrawer()
    {
        if (_character.isDeadState || _character.isCamera)
        {
            detectorDrawer.EndDraw(DetectorDrawer.Detect);
            detectorDrawer.EndDraw(DetectorDrawer.Spotted);
            detectorDrawer.EndDraw(DetectorDrawer.Warning);
            detectorDrawer.EndDraw(DetectorDrawer.Sensor);
            return;
        }
        // if (hateInfo.MaxHateEntityRank >= 0)
        // {
            var tmp = new CircleInfo()
            {
                center = _model.position + new Vector3(0, 0.2f, 0),
                forward = _model.up,
                isSector = true,
                sectorInitialAngleInDegrees = -90,
            };

            tmp.radius = 1f;
            if(hateInfo.MaxHateEntityRank >= 0)
                tmp.fillColor = _colorList[hateInfo.MaxHateEntityRank];
            if (hateInfo.MaxHateEntityPoint <= 360)
            {
                tmp.sectorArcLengthInDegrees = hateInfo.MaxHateEntityPoint;
            }
            else
            {
                tmp.sectorArcLengthInDegrees = 360;
            }

            detectorDrawer.Draw(DetectorDrawer.Spotted, _model, RadiusSpotted, DegreeDetect);
            detectorDrawer.Draw(DetectorDrawer.Warning, _model, RadiusWarning, DegreeDetect);
            detectorDrawer.EndDraw(DetectorDrawer.Sensor);
            if (hateInfo.MaxHateEntityPoint > 0)
                detectorDrawer.Draw(DetectorDrawer.Detect, _model, tmp);
            else
                detectorDrawer.EndDraw(DetectorDrawer.Detect);
        // }
        // else
        // {
        //     // detectorDrawer.Draw(DetectorDrawer.Sensor, _model, 2);
        //     detectorDrawer.EndDraw(DetectorDrawer.Sensor);
        //     detectorDrawer.EndDraw(DetectorDrawer.Detect);
        //     detectorDrawer.EndDraw(DetectorDrawer.Spotted);
        //     detectorDrawer.EndDraw(DetectorDrawer.Warning);
        // }
    }
    
    private void RefreshMaxHateTarget(float deltaTime)
    {
        if (hateInfo.HasHateTarget)
        {
            var hateEntity = EntityUtils.GetGameEntity(hateInfo.MaxHateEntityId);
            if (CheckTargetIsAlive(hateEntity))
            {
                var dist = DetectMgr.Inst.GetDistance(hateEntity, _character);
                var dir = hateEntity.position.value - _model.position;
                var normalDir = dir / dist;
                var angle = normalDir.GetAngle(_model.forward);
                // 距离仇恨值增加
                AddDistanceHatePoint(hateInfo.MaxHateEntityId, dist * dist, angle, deltaTime);
            }
        }
    }

    public void UpdateDetect(float deltaTime)
    {
        RefreshMaxHateTarget(deltaTime);
        if (detectList.Count > 0)
        {
            for (int i = 0; i < detectList.Count; i++)
            {
                var point = detectList[i];
                var target = EntityUtils.GetGameEntity(point.EntityId);
                if(point.EntityId == hateInfo.MaxHateEntityId)
                    continue;
                if (CheckTargetIsAlive(target))
                {
                    var dir = point.Position - _model.position;
                    var normalDir = dir / point.Dist;
                    var angle = normalDir.GetAngle(_model.forward);
                    // 距离仇恨值增加
                    AddDistanceHatePoint(point.EntityId, point.SqrDist, angle, deltaTime);
                }
            }
            detectList.Clear();
        }

        hateInfo.CheckIsHasHateTarget();
    }

    private bool CheckTargetIsAlive(GameEntity target, bool clearHate = true)
    {
        if (target.isDeadState)
        {
            if(clearHate)
                hateInfo.Set(target.instanceID.ID, 0, 0);
            return false;
        }

        return true;
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
                hateInfo.Add(id, 200 * deltaTime, HatePointType.Spotted);
                return;
            }
            if (sqrDist <= RadiusWarningSqr)
            {
                hateInfo.Add(id, 50 * deltaTime, HatePointType.Warning);
                return;
            }
        }
        hateInfo.Add(id, -150 * deltaTime, HatePointType.OutSign);
    }

    public void Dispose()
    {
        detectorDrawer.Dispose();
        DetectMgr.Inst.CancelHatePoint(_character.instanceID.ID);
    }

    public GameEntity Entity => _character;
    public SensorEntity Sensor => _sensor;
    public HatePointInfo HatePointInfo => hateInfo;
}
