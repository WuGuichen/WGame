public class EventDefine
{
    public const int OnGameResourcesLoaded = 1;
    public const int OnGameAssetsManagerInitted = 2;
    public const int OnGameCodeInitted = 3;
    public const int OnGameStart = 4;
    public const int OnCharacterInit = 5;
    public const int OnGameSystemsInitted = 6;
    
    public const int OnGameExit = 7;
    public const int OnControlCharacterChanged = 8;
    
    public const int OnJoystickStateChanged = 10;

    public const int OnFPSUpdate = 11;
    public const int OnGameUpdate = 12;

    public const int OnBackToMainView = 13;

    public const int OnTerminalMessageUpdate = 21;
    
    public const int OnInteractTagRefresh = 31;

    public const int OnBTreeHotUpdate = 41;
    public const int OnFSMHotUpdate = 42;
    
    public const int SetCursorState = 52;

    // 界面加载完成
    public const int OnSceneLoaded = 60;
    // 按键绑定状态变化
    public const int OnRebindingInputStateChange = 61;
    
    public const int OnEnterGameMainView = 62;

    public const int OnClientChanged = 63;
    public const int OnPlayerRoomInfoRefresh = 64;
    public const int OnSelfClientDisconnected = 65;
    public const int OnSelfClientConnected = 66;
    public const int OnServerStartGame = 67;
    public const int OnServerEndGame = 68;
    
    public const int OnWeakPointUpdate = 80;
    public const int OnFocusPointUpdate = 81;
}
