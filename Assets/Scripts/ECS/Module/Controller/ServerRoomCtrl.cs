using Unity.Netcode;
using WGame.Runtime;

public class ServerRoomCtrl : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback += OnOnClientDisconnectCallback;
        }

        RegisterEvent();

        var info = new PlayerRoomInfo(WNetMgr.Inst.LocalClientId, false);
        WNetMgr.Inst.AddPlayer(info);
        UpdateAllPlayerInfo();
    }

    public override void OnDestroy()
    {
        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback -= OnOnClientDisconnectCallback;
        }

        UnRegisterEvent();
        base.OnDestroy();
    }

    private void UnRegisterEvent()
    {
        if (IsOwner)
        {
            EventCenter.RemoveListener(EventDefine.OnPlayerRoomInfoRefresh, OnPlayerRoomInfoRefresh);
            EventCenter.RemoveListener(EventDefine.OnSelfClientDisconnected, OnSelfDisconnected);
        }
    }

    private void OnSelfDisconnected()
    {
        WNetMgr.Inst.RemoveAllPlayer();
    }

    private void RegisterEvent()
    {
        if (IsOwner)
        {
            EventCenter.AddListener(EventDefine.OnPlayerRoomInfoRefresh, OnPlayerRoomInfoRefresh);
            EventCenter.AddListener(EventDefine.OnSelfClientDisconnected, OnSelfDisconnected);
        }
    }

    private void OnPlayerRoomInfoRefresh(TAny context)
    {
        if (WNetMgr.Inst.TryGetPlayerRoomInfo(context.AsULong(), out var info))
        {
            RefreshPlayerRoomInfo(info.id);
        }
    }

    private void OnClientConnectedCallback(ulong obj)
    {
        WNetMgr.Inst.AddPlayer(new PlayerRoomInfo(obj, false));
        UpdateAllPlayerInfo();
    }

    private void OnOnClientDisconnectCallback(ulong obj)
    {
        WNetMgr.Inst.RemovePlayer(obj);
        UpdateAllPlayerInfo();
    }
    
    private void UpdateAllPlayerInfo()
    {
        var allInfos = WNetMgr.Inst.AllPlayerInfo;
        for (var i = 0; i < allInfos.Count; i++)
        {
            UpdatePlayerInfoClientRpc(allInfos[i]);
        }
    }

    [ClientRpc]
    private void UpdatePlayerInfoClientRpc(PlayerRoomInfo info)
    {
        if (!IsHost)
        {
            // 纯客户端添加player信息(除Host以外的客户端)
            WLogger.Print(WNetMgr.Inst.LocalClientId + " 非Host客户端更新Player：" + info.id + " is " + info.isReady) ;
            WNetMgr.Inst.AddPlayer(info);
        }
    }
    
    public void RefreshPlayerRoomInfo(ulong id)
    {
        if (IsServer)
        {
            // 服务器直接通知客户端
            UpdateAllPlayerInfo();
        }
        else
        {
            // 客户端则通知服务器自己的数据
            if (WNetMgr.Inst.TryGetPlayerRoomInfo(id, out var info))
            {
                WLogger.Print(WNetMgr.Inst.LocalClientId + " 通知服务器: " + id + " is " + info.isReady);
                UpdateAllPlayerInfoServerRpc(info);
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void UpdateAllPlayerInfoServerRpc(PlayerRoomInfo info)
    {
        WLogger.Print("更新客户端: " + info.id + " is " + info.isReady);
        // 服务器下发数据到客户端
        WNetMgr.Inst.AddPlayer(info);
        UpdateAllPlayerInfo();
    }
}
