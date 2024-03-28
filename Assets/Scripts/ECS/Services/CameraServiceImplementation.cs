using UnityEngine;

public class CameraServiceImplementation : ICameraService
{
    private Transform _root;
    private Transform _pivot;
    private Transform _camera;

    // X轴旋转
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

    // Y轴旋转
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

    private Vector3 _camOriginPos;

    // Z轴旋转
    public Transform Camera
    {
        get
        {
            if (_camera == null)
            {
                if (Pivot != null)
                {
                    _camera = Pivot.GetChild(0);
                    _targetMovePos = _camera.localPosition;
                    _camOriginPos = _targetMovePos;
                }
            }

            return _camera;
        }
    }

    private float _shakeX;
    private float _shakeY;
    private float _shakeZ;
    private float _shakeTime = 0f;
    private float _shakeTotalTime = -1f;
    private float _shakeIntensity = 0f;
    private WEaseType _shakeEaseType;

    private Vector3 _targetMovePos;
    private Vector3 _moveDeltaPos;
    private WEaseType _posEaseType;
    private float _movePosTotalTime;
    private float _movePosDuration;
    private float _moveHoldTime;
    private const float POS_MIN_Z = -12f;
    private const float POS_MAX_Z = -0.84f;
    private bool _posChanging = false;
    private bool _hasChangeCameraXY = false;
    
    private Vector3 _baseRotAngle;
    private Vector3 _targetRotAngle;
    private float _rotDeltaAngleX;
    private float _rotDeltaAngleY;
    private Vector3 _curRotAngle;
    private WEaseType _rotEaseType;
    private float _rotSpeed;
    private float _rotTime;
    private float _rotHoldTime;
    private bool _rotChanging = false;
    public bool IsAutoControl => _posChanging || _rotChanging;
    public Vector3 CachedFwd { get; private set; }

    public void Shake(float time, float intensity = 1, WEaseType easeType = WEaseType.Linear)
    {
        _shakeTime = 0f;
        _shakeTotalTime = time;
        _shakeIntensity = intensity;
        _shakeEaseType = easeType;
    }

    public void Move(Vector3 deltaPos, WEaseType easeType = WEaseType.Linear, float duration = 0.0f, float holdTime = 0.2f)
    {
        if (Mathf.Abs(deltaPos.x) > 0.01f || Mathf.Abs(deltaPos.y) > 0.01f)
        {
            _hasChangeCameraXY = true;
        }
        _moveHoldTime = holdTime;
        if (duration > 0.1f)
        {
            _movePosTotalTime = duration;
            _movePosDuration = 0;
            _posEaseType = easeType;
            _posChanging = true;
            _moveDeltaPos = deltaPos;
            _targetMovePos = Camera.localPosition;
        }
        else
        {
            _targetMovePos += deltaPos;
            if (_targetMovePos.z > POS_MAX_Z)
            {
                _targetMovePos.z = POS_MAX_Z;
            }
            else if(_targetMovePos.z < POS_MIN_Z)
            {
                _targetMovePos.z = POS_MIN_Z;
            }
            var tarPos = _targetMovePos;
            Camera.localPosition = tarPos;
        }
    }

    public void StopMove()
    {
        _posChanging = false;
    }

    public void StopRotate()
    {
        _rotChanging = false;
    }

    public void Rotate(Vector3 deltaRot, WEaseType easeType = WEaseType.Linear, float speed = 0.2f, float holdTime = 2f)
    {
        _targetRotAngle = deltaRot;
        var angle = GetCurrentDeltaPos();
        _rotDeltaAngleX = angle.x;
        _rotDeltaAngleY = angle.y;
        if (_rotDeltaAngleX > 180)
        {
            _rotDeltaAngleX -= 360;
        }
        else if (_rotDeltaAngleX < -180)
        {
            _rotDeltaAngleX += 360;
        }
        if (_rotDeltaAngleY > 180)
        {
            _rotDeltaAngleY -= 360;
        }
        else if (_rotDeltaAngleY < -180)
        {
            _rotDeltaAngleY += 360;
        }
        _curRotAngle = Vector3.zero;
        _rotEaseType = easeType;
        _rotSpeed = speed;
        _rotChanging = true;
        _rotTime = 0f;
        _rotHoldTime = holdTime;
    }

    public void Process(float deltaTime)
    {
        if (_shakeTime < _shakeTotalTime)
        {
            var rate = WEaseManager.Evaluate(_shakeEaseType, _shakeTime, _shakeTotalTime);
            var deltaPos = Random.insideUnitSphere * _shakeIntensity * (1f-rate);
            Root.transform.position += deltaPos;
            _shakeTime += deltaTime;
        }

        if (!(_posChanging || _rotChanging))
        {
            CachedFwd = Camera.forward;
        }
        
        if (_rotChanging)
        {
            if (_rotTime < 1f)
            {
                var rate = WEaseManager.Evaluate(_rotEaseType, _rotTime, 1f);

                Vector3 rotation = Vector3.zero;
                rotation.y = _baseRotAngle.y + rate * _rotDeltaAngleY;
                Root.rotation = Quaternion.Euler(rotation);
                rotation.y = 0f;
                rotation.x = _baseRotAngle.x + rate * _rotDeltaAngleX;
                Pivot.localRotation = Quaternion.Euler(rotation);
            }
            else
            {
                if (_rotHoldTime > 0f)
                {
                    _rotHoldTime -= deltaTime;
                }
                else
                {
                    _rotChanging = false;
                }
            }

            _rotTime += _rotSpeed * deltaTime;
        }

        if (_posChanging)
        {
            if (_movePosDuration > _movePosTotalTime)
            {
                if (_moveHoldTime > 0)
                {
                    _moveHoldTime -= deltaTime;
                }
                else
                {
                    _posChanging = false;
                    _targetMovePos = Camera.localPosition;
                    _targetMovePos.x = 0;
                    _targetMovePos.y = 0;
                }
            }
            else
            {
                var rate = WEaseManager.Evaluate(_posEaseType, _movePosDuration, _movePosTotalTime);
                Camera.localPosition = _targetMovePos + rate * _moveDeltaPos;
            }

            _movePosDuration += deltaTime;
        }
        else
        {
            if (_hasChangeCameraXY)
            {
                SmoothResetCameraXY(deltaTime);
            }
        }
    }

    private void SmoothResetCameraXY(float deltaTime)
    {
        var pos = Camera.localPosition;
        var posX = pos.x;
        var posY = pos.y;
        if (Mathf.Abs(posX) > 0.01f)
        {
            posX -= (posX > 0 ? deltaTime : -deltaTime);
        }
        else if (Mathf.Abs(posY) > 0.01f)
        {
            posY -= (posY > 0 ? deltaTime : -deltaTime);
        }
        else
        {
            posX = 0;
            posY = 0;
            _targetMovePos = new Vector3(0, 0, pos.z);
            _hasChangeCameraXY = false;
        }

        Camera.localPosition = new Vector3(posX, posY, pos.z);
    }

    private Vector3 GetCurrentDeltaPos()
    {
        var entity = EntityUtils.GetCameraEntity();
        var fwdAngle = entity.gameViewService.service.Model.eulerAngles;
        var targetAngle = _targetRotAngle + new Vector3(0,fwdAngle.y,0);
        _baseRotAngle = new Vector3(Pivot.localEulerAngles.x ,Root.eulerAngles.y, 0);
        return targetAngle - _baseRotAngle;
    }
}
