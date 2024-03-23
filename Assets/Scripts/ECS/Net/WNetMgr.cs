using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using WGame.Runtime;

public class WNetMgr : Singleton<WNetMgr>
{
	public const int NetPort = 7777;
	private Dictionary<int, GameEntity> _netEntities = new();
	private IFactoryService _factory;
	
    private Dictionary<ulong, PlayerRoomInfo> playerRoomInfoDict = new();

    private List<PlayerRoomInfo> _allPlayerInfo = new();

    private bool isInitCtrls = false;
    
    public WNetAgent Agent { get; private set; }
    public Dictionary<ulong, WNetAgent> OtherAgents { get; private set; } = new();

    public void SetAgent(WNetAgent agent)
    {
	    Agent = agent;
    }

    public void AddOtherAgent(WNetAgent agent)
    {
	    OtherAgents.Add(agent.OwnerClientId, agent);
    }
    
    public void RemoveOtherAgent(WNetAgent agent)
    {
	    OtherAgents.Remove(agent.OwnerClientId);
    }

    public bool TryGetPlayerRoomInfo(ulong id, out PlayerRoomInfo info)
    {
	    return playerRoomInfoDict.TryGetValue(id, out info);
    }

    private void RefreshAllPlayerInfo()
    {
	    _allPlayerInfo.Clear();
	    var itr = playerRoomInfoDict.Values.GetEnumerator();
	    while (itr.MoveNext())
	    {
		    if (itr.Current.id == LocalClientId)
		    {
			    MyPlayerInfo = itr.Current;
		    }
		    _allPlayerInfo.Add(itr.Current);
	    }
    }

    public List<PlayerRoomInfo> AllPlayerInfo => _allPlayerInfo;
    public PlayerRoomInfo MyPlayerInfo { get; private set; }

    public ulong LocalClientId => NetworkManager.Singleton.LocalClientId;
    public bool IsHost => NetworkManager.Singleton.IsHost;
    public bool IsServer => NetworkManager.Singleton.IsListening && NetworkManager.Singleton.IsServer;
    public bool IsClient => NetworkManager.Singleton.IsClient;
    public bool IsConnected => NetworkManager.Singleton.IsConnectedClient;
    public bool IsConnecting => NetworkManager.Singleton.IsClient && !IsConnected;
    
	public void InitInstance()
	{
		NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
		NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisConnected;
		NetworkManager.Singleton.OnClientStarted += OnServerStarted;

		_factory = Contexts.sharedInstance.meta.factoryService.instance;
	}

	private string connectMsg => Transport.ConnectionData.Address + ":" + Transport.ConnectionData.Port;

	public bool StartClient()
	{
		if (IsClient)
		{
			WLogger.Info("已经加入游戏");
			return true;
		}
		
		if (NetworkManager.Singleton.StartClient())
		{
			WLogger.Info("客户端启动成功！" + connectMsg);
			EventCenter.Trigger(EventDefine.OnClientChanged);
			return true;
		}
		else
		{
			WLogger.Error("客户端启动失败！" + connectMsg);
			return false;
		}
	}

	public bool StartHost()
	{
		if (NetworkManager.Singleton.StartHost())
		{
			WLogger.Info("服务器和客户端启动成功！" + connectMsg);
			return true;
		}
		else
		{
			WLogger.Error("服务器和客户端启动失败！" + connectMsg);
			return false;
		}
	}

	public void StartServer()
	{
		if (NetworkManager.Singleton.StartServer())
		{
			WLogger.Info("服务器启动成功！" + connectMsg);
		}
		else
		{
			WLogger.Error("服务器启动失败！" + connectMsg);
		}
	}

	public void ShutDown()
	{
        RemoveAllPlayer();
		NetworkManager.Singleton?.Shutdown();
	}

	public void RemoveAllPlayer()
	{
		playerRoomInfoDict.Clear();
		RefreshAllPlayerInfo();
		EventCenter.Trigger(EventDefine.OnClientChanged);
	}

	private void OnClientConnected(ulong id)
	{
		WLogger.Info("Client connected id: " + id + ", " + Transport.ConnectionData.Address);

		// var charId = 1;
		// var pos = EntityUtils.GetCameraPos() + EntityUtils.GetCameraFwdDir()*2;
		// pos.y = 0.5f;
		// _factory.GenCharacter(charId, pos, Quaternion.identity, out var entity,
		// 	gameEntity =>
		// 	{
		// 		var obj = gameEntity.gameViewService.service.Model.parent.gameObject;
		// 		var netObj = obj.AddComponent<WNetObject>();
		// 		gameEntity.AddNetObject(netObj);
		// 		var trans = obj.AddComponent<NetworkTransform>();
		// 		_netEntities.Add(gameEntity.instanceID.ID, gameEntity);
		// 	});
		if (id == LocalClientId)
		{
			EventCenter.Trigger(EventDefine.OnSelfClientConnected);
		}
		EventCenter.Trigger(EventDefine.OnClientChanged);
	}
	
	private UnityTransport Transport => NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;

	public void SetConnectData(string ipv4, ushort port, string listenAddress = null)
	{
		Transport.SetConnectionData(ipv4, port, listenAddress);	
	}

	public void AddPlayer(List<PlayerRoomInfo> infos)
	{
		for (var i = 0; i < infos.Count; i++)
		{
			var info = infos[i];
			playerRoomInfoDict[info.id] = info;
		}
		RefreshAllPlayerInfo();
		EventCenter.Trigger(EventDefine.OnClientChanged);
	}
	
	public void AddPlayer(PlayerRoomInfo info)
	{
		playerRoomInfoDict[info.id] = info;
		RefreshAllPlayerInfo();
		EventCenter.Trigger(EventDefine.OnClientChanged);
	}

	public void RemovePlayer(ulong id)
	{
		if (playerRoomInfoDict.ContainsKey(id))
		{
			playerRoomInfoDict.Remove(id);
			RefreshAllPlayerInfo();
			EventCenter.Trigger(EventDefine.OnClientChanged);
		}
	}
	
	private void OnClientDisConnected(ulong id)
	{
		WLogger.Info("Client disconnected id: " + id);
		if (id == LocalClientId)
		{
			EventCenter.Trigger(EventDefine.OnSelfClientDisconnected);
		}
	}

	private void OnServerStarted()
	{
		WLogger.Info("server started!");
		if (isInitCtrls)
		{
			return;
		}
		isInitCtrls = true;

	}

	public int GetClientNum()
	{
		return playerRoomInfoDict.Count;
	}

	public bool IsReady
	{
		get
		{
			if (playerRoomInfoDict.TryGetValue(LocalClientId, out var info))
			{
				return info.isReady;
			}

			return false;
		}
	}

	public void RefreshPlayRoomInfo(ulong id, int charId)
	{
		if(playerRoomInfoDict.TryGetValue(id, out var info))
		{
			info.charId = charId;
			playerRoomInfoDict[id] = info;
			RefreshAllPlayerInfo();
			EventCenter.Trigger(EventDefine.OnPlayerRoomInfoRefresh, id);
		}
	}
	public void RefreshPlayRoomInfo(ulong id, bool isReady)
	{
		if(playerRoomInfoDict.TryGetValue(id, out var info))
		{
			info.isReady = isReady;
			playerRoomInfoDict[id] = info;
			RefreshAllPlayerInfo();
			EventCenter.Trigger(EventDefine.OnPlayerRoomInfoRefresh, id);
		}
	}

	public bool IsAllPlayerReady()
	{
		// for (var i = 0; i < AllPlayerInfo.Count; i++)
		// {
		// 	var info = AllPlayerInfo[i];
		// 	if (!info.isReady)
		// 	{
		// 		return false;
		// 	}
		// }

		return true;
	}

	public void StartGame()
	{
		EventCenter.Trigger(EventDefine.OnServerStartGame);
	}

	public void BackToMainView()
	{
		EventCenter.Trigger(EventDefine.OnServerEndGame);
	}

	public void OnDispose()
	{
		ShutDown();
	}
}
