using UnityEngine;

public class CameraServiceImplementation : ICameraService
{
    private Transform _root;
    private Transform _pivot;
    private Transform _camera;

    public Transform Root
    {
        get
        {
            if (_root == null)
            {
                var go = GameObject.Find("CameraRoot");
                if (go != null)
                    _root = go.transform;
            }

            return _root;
        }
    }

    public Transform Pivot
    {
        get
        {
            if (_pivot == null)
            {
                if (Root != null)
                {
                    _pivot = Root.GetChild(0);
                }
            }

            return _pivot;
        }
    }

    public Transform Camera
    {
        get
        {
            if (_camera == null)
            {
                if (Pivot != null)
                {
                    _camera = Pivot.GetChild(0);
                }
            }

            return _camera;
        }
    }
}
