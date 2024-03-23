using Unity.Netcode;
using UnityEngine;

public class WNetAgent : NetworkBehaviour
{
    private NetworkVariable<Vector3> _syncPos = new();
    private NetworkVariable<Quaternion> _syncRot = new();
    private NetworkVariable<bool> _syncIsCamera = new();
    private NetworkVariable<float> _syncAnimRight = new();
    private NetworkVariable<float> _syncAnimUp = new();
    private IFactoryService _factory;
    private GameEntity _entity;

    private const float threshold = 0.1f;

    public Vector3 SyncPos => _syncPos.Value;
    public Quaternion SyncRot => _syncRot.Value;
    public float AnimRight => _syncAnimRight.Value;
    public float AnimUp => _syncAnimUp.Value;
    
    public bool IsCamera
    {
        get => !IsOwner && _syncIsCamera.Value;
        set
        {
            if (IsServer)
            {
                _syncIsCamera.Value = value;
            }
            else
            {
                SetIsCameraServerRpc(value);             
            }
        }
    }
    
    public void SetAnimParam(float right, float up)
    {
        if (IsServer)
        {
            if (Mathf.Abs(_syncAnimRight.Value - right) > threshold)
            {
                _syncAnimRight.Value = right;
            }
            if (Mathf.Abs(_syncAnimUp.Value - up) > threshold)
            {
                _syncAnimUp.Value = up;
            }
        }
        else
        {
            SetAnimParamServerRpc(right, up);         
        }
    }

    [ServerRpc]
    private void SetAnimParamServerRpc(float right, float up)
    {
        if (Mathf.Abs(_syncAnimRight.Value - right) > threshold)
        {
            _syncAnimRight.Value = right;
        }

        if (Mathf.Abs(_syncAnimUp.Value - up) > threshold)
        {
            _syncAnimUp.Value = up;
        }
    }

    [ServerRpc]
    private void SetIsCameraServerRpc(bool value)
    {
        _syncIsCamera.Value = value;
    }

    public void SetGameEntity(ref GameEntity entity)
    {
        _entity = entity;
        if (IsServer)
        {
            UpdatePosition(Vector3.zero, Quaternion.identity);
        }
    }

    private IFactoryService Factory
    {
        get
        {
            if (_factory == null)
            {
                _factory = Contexts.sharedInstance.meta.factoryService.instance;
            }
            return _factory;
        }
    }

    public void UpdatePosition(Vector3 pos, Quaternion rot)
    {
        if (IsServer)
        {
            _syncPos.Value = pos;
            _syncRot.Value = rot;
        }
        else
        {
            UpdatePositionServerRpc(pos, rot);
        }
    }
    
    [ServerRpc]
    private void UpdatePositionServerRpc(Vector3 pos, Quaternion rot)
    {
        _syncPos.Value = pos;
        _syncRot.Value = rot;
    }
    

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            WNetMgr.Inst.SetAgent(this);
            NetworkManager.OnClientDisconnectCallback += OnDisconnected;
        }
        else
        {
            WNetMgr.Inst.AddOtherAgent(this);
        }
    }

    private void OnDisconnected(ulong obj)
    {
        
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            NetworkManager.OnClientDisconnectCallback -= OnDisconnected;
        }
        else
        {
            WNetMgr.Inst.RemoveOtherAgent(this);
        }
        
        _entity = null;
        _factory = null;
    }

    public void GenCharacter(PlayerRoomInfo info)
    {
        WLogger.Print("Gen Server" + info.id + "type:" + info.charId);
        GenServerCharacterServerRpc(info);
    }

    [ServerRpc(RequireOwnership = false)]
    private void GenServerCharacterServerRpc(PlayerRoomInfo info)
    {
        GenServerCharacterClientRpc(info);
    }

    [ClientRpc]
    private void GenServerCharacterClientRpc(PlayerRoomInfo info)
    {
        if (!IsOwner)
        {
            WLogger.Print("生成：" + info.id + "type" + info.charId + "isOwner" + (info.id==WNetMgr.Inst.LocalClientId));
            Factory.GenServerCharacter(info, out var gameEntity);
        }
    }
}
