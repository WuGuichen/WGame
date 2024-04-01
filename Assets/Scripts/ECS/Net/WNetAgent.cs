using System;
using Unity.Netcode;
using UnityEngine;
using WGame.Attribute;

public class WNetAgent : NetworkBehaviour
{
    private readonly NetworkVariable<Vector3> _syncPos = new();
    private readonly NetworkVariable<Quaternion> _syncRot = new();
    private readonly NetworkVariable<bool> _syncIsCamera = new();
    private readonly NetworkVariable<float> _syncAnimRight = new();
    private readonly NetworkVariable<float> _syncAnimUp = new();
    private readonly NetworkVariable<int> _syncMotionID = new();
    private delegate void OnOtherClientAttrChange(int attrId, int value);

    private NetworkVariable<int> _attrHp = new();

    private IFactoryService _factory;
    private GameEntity _entity;

    private const float threshold = 0.1f;
    
    public bool IsServerAgent => OwnerClientId == 0;

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

    public void SwitchMotion(int newID)
    {
        if (IsServer)
        {
            _syncMotionID.Value = newID;
        }
        else
        {
            SwitchMotionServerRpc(newID);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SwitchMotionServerRpc(int newID)
    {
        WLogger.Print("ServerChangeMotionID:" + newID);
        _syncMotionID.Value = newID;
        if (IsServer)
        {
            OnMotionIdChange(0, newID);
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
        else
        {
        }

        if (!IsOwner)
        {
            // 不是自己，则监听属性变化
            if (_entity.hasAttribute)
            {
                for (int i = 0; i < WAttrType.Count; i++)
                {
                    _entity.attribute.value.RegisterEvent(i, OnAttrChange);
                }
            }
        }
    }

    private void OnAttrValueChange(int attrId, int newValue)
    {
        WLogger.Print("Set: " + attrId + ": " + newValue);
        _entity.attribute.value.Set(attrId, newValue);
    }

    private void RefreshAttr(int id, int value)
    {
        switch (id)
        {
            case WAttrType.CurHP:
                _attrHp.Value = value;
                break;
        }

        if (IsServer)
        {
            _entity.attribute.value.Set(id, value);
        }
    }

    private void OnAttrChange(WaEventContext context)
    {
        // 其他角色的属性变化
        if (IsServer)
        {
            RefreshAttr(context.attrID, context.value);
        }
        else
        {
            OnAttrChangeServerRpc(context.attrID ,context.value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnAttrChangeServerRpc(int id, int value)
    {
        WLogger.Print("Change: " + OwnerClientId + " :" +value) ;
        RefreshAttr(id, value);
    }

    private void OnMotionIdChange(int previousvalue, int newvalue)
    {
        if (_entity.hasLinkMotion)
        {
            _entity.linkMotion.Motion.motionService.service.SwitchMotion(newvalue, false);
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
        if (IsServer)
        {
            _attrHp.Value = 0;
        }
        else
        {
            _syncMotionID.OnValueChanged += OnMotionIdChange;
            _syncPos.OnValueChanged += OnPositionChanged;
            
            _attrHp.OnValueChanged += (value, newValue) =>
            {
                _entity.attribute.value.Set(WAttrType.CurHP, newValue);
            };
        }
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

    private void OnPositionChanged(Vector3 previousvalue, Vector3 newvalue)
    {
        
    }

    private void OnDisconnected(ulong obj)
    {
        
    }

    public override void OnNetworkDespawn()
    {
        Dispose();
        if (IsOwner)
        {
            NetworkManager.OnClientDisconnectCallback -= OnDisconnected;
        }
        else
        {
            WNetMgr.Inst.RemoveOtherAgent(this);
        }

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

    public void Dispose()
    {
        if (_entity != null && _entity.isEnabled)
        {
            if (!IsOwner)
            {
                if (_entity.hasAttribute)
                {
                    for (int i = 0; i < WAttrType.Count; i++)
                    {
                        _entity.attribute.value.CancelEvent(i, OnAttrChange);
                    }
                }
            }

            _entity.RemoveNetAgent();
            _entity = null;
            _factory = null;
        }
    }
}
