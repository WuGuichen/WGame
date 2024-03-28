using Shapes;
using UnityEngine;

public class DetectorDrawer
{
    private WDrawer.CircleDrawInfo drawWarning;
    private WDrawer.CircleDrawInfo drawSpotted;
    private WDrawer.CircleDrawInfo drawSensor;
    private WDrawer.CircleDrawInfo drawDetect;

    public const int Warning = 0;
    public const int Spotted = 1;
    public const int Sensor = 2;
    public const int Detect = 3;
    private readonly WDrawer.CircleDrawInfo[] drawInfos = new WDrawer.CircleDrawInfo[4];

    private static readonly Color _colorWarning = Color.cyan*0.4f;
    private static Color _colorSpotted = Color.blue*0.4f;
    private static Color _colorDetectted = Color.red*0.4f;
    private static Color _colorDetecting = Color.yellow*0.2f;
    private static Color _colorSensor = Color.white*0.2f;
    
    private static Quaternion _rotation = Quaternion.Euler(90, 0, 0);

    private CircleInfo GetInfo(int type, Transform parent, float radius, int degree, Color color)
    {
        return new CircleInfo()
        {
            center = drawInfos[type].Drawer.position,
            forward = parent.up,
            radius = radius,
            fillColor = color,
            isSector = true,
            sectorArcLengthInDegrees = degree,
            sectorInitialAngleInDegrees = parent.eulerAngles.y - 90 - (degree >> 1),
        };
    }
    private CircleInfo GetInfo(int type, Transform parent, float radius, int degree)
    {
        var color = type switch
        {
            Warning => _colorWarning,
            Spotted => _colorSpotted,
            Sensor => _colorSensor,
            Detect => _colorDetecting,
            _ => _colorWarning
        };
        return GetInfo(type, parent, radius, degree, color);
    }

    private void RegisterDrawer(int type, Transform parent)
    {
        if (drawInfos[type] == null)
        {
            drawInfos[type] = WDrawer.Inst.RegisterCircle(parent, parent.position + Vector3.up * 0.2f, _rotation);
        }
    }

    private static void CancelDrawer(ref WDrawer.CircleDrawInfo drawer)
    {
        if (drawer != null)
        {
            WDrawer.Inst.CancelCircle(drawer);
            drawer = null;
        }
    }
    
    public void EndDraw(int type)
    {
        CancelDrawer(ref drawInfos[type]);
    }

    public void Draw(int type, Transform parent, float radius, int degree)
    {
        RegisterDrawer(type, parent);
        drawInfos[type].Info = GetInfo(type, parent, radius, degree);
    }

    public void Draw(int type, Transform parent, float radius)
    {
        RegisterDrawer(type, parent);
        drawInfos[type].Info = new CircleInfo()
        {
            center = drawInfos[type].Drawer.position,
            forward = parent.up,
            fillColor = _colorSensor,
            radius = radius,
        };
    }

    public void Draw(int type, Transform parent, CircleInfo info)
    {
        RegisterDrawer(type, parent);
        drawInfos[type].Info = info;
    }

    public void Dispose()
    {
        for (int i = 0; i < drawInfos.Length; i++)
        {
            if (drawInfos[i] != null)
            {
                WDrawer.Inst.CancelCircle(drawInfos[i]);
                drawInfos[i] = null;
            }
        }
    }
}
