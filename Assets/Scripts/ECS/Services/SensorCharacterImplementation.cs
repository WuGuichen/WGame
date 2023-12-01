using Shapes;
using UnityEngine;

public class SensorCharacterImplementation : ISensorService
{
    private WDrawer.CircleDrawInfo drawInfo;
    private readonly Transform _model;

    public SensorCharacterImplementation(Transform model)
    {
        _model = model;
    }
    
    public void UpdateSensorDrawer()
    {
        CircleInfo info = new CircleInfo()
        {
            center = _model.position,
            forward = _model.forward,
        };
        if (drawInfo == null)
        {
            WDrawer.Inst.RegisterCircle(_model, info);
        }
        else
        {
            drawInfo.Info = info;
        }
    }

    public void Dispose()
    {
        if (drawInfo != null)
        {
            WDrawer.Inst.CancelCircle(drawInfo);
        }
    }
}
