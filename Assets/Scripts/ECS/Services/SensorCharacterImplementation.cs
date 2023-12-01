using Shapes;
using UnityEngine;

public class SensorCharacterImplementation : ISensorService
{
    private WDrawer.CircleDrawInfo drawInfo;
    private readonly Transform _model;
    private Color _color = Color.cyan*0.6f;
    private Quaternion _rotation = Quaternion.Euler(90, 0, 0);

    public SensorCharacterImplementation(Transform model)
    {
        _model = model;
    }
    
    public void UpdateSensorDrawer()
    {
        if (drawInfo == null)
        {
            drawInfo = WDrawer.Inst.RegisterCircle(_model, _model.position+Vector3.up*0.2f, _rotation);
        }
        
        CircleInfo info = new CircleInfo()
        {
            center = drawInfo.Drawer.position,
            forward = _model.up,
            fillColor = _color,
            radius = 4f,
            isSector = true,
            sectorArcLengthInDegrees = 120,
            sectorInitialAngleInDegrees = _model.eulerAngles.y - 150,
        };
        drawInfo.Info = info;
    }

    public void Dispose()
    {
        if (drawInfo != null)
        {
            WDrawer.Inst.CancelCircle(drawInfo);
        }
    }
}
