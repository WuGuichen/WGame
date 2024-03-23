using System.Collections.Generic;
using BaseData.Character;
using Entitas.Unity;
using Motion;
using Pathfinding;
using Unity.Netcode;
using UnityEngine;
using WGame.Attribute;

public class WNetAgent : NetworkBehaviour
{
    private NetworkVariable<Vector3> _syncPos = new();
    private NetworkVariable<bool> _syncIsCamera = new();
    private IFactoryService _factory;
    private GameEntity _entity;

    public Vector3 SyncPos => _syncPos.Value;
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
    
    [ServerRpc(RequireOwnership = false)]
    private void SetIsCameraServerRpc(bool value)
    {
        _syncIsCamera.Value = value;
    }

    public void SetGameEntity(ref GameEntity entity)
    {
        _entity = entity;
        if (IsServer)
        {
            UpdatePosition(Vector3.zero);
        }
        else
        {
            _syncPos.OnValueChanged += OnPositionChanged;
        }
    }

    private void OnPositionChanged(Vector3 previousvalue, Vector3 newvalue)
    {
        _entity.gameViewService.service.Model.transform.position = newvalue;
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

    public void UpdatePosition(Vector3 pos)
    {
        if (_entity != null && _entity.hasInstanceID)
        {
            WLogger.Print(_entity.instanceID.ID);
        }
        if (IsServer)
        {
            _syncPos.Value = pos;
        }
        else
        {
            UpdatePositionServerRpc(pos);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void UpdatePositionServerRpc(Vector3 pos)
    {
        _syncPos.Value = pos;
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
        
        _syncPos.OnValueChanged -= OnPositionChanged;
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
