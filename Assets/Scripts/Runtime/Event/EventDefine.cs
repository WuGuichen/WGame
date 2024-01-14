public class EventDefine
{
    public const int OnGameResourcesLoaded = 1;
    public const int OnGameAssetsManagerInitted = 2;
    public const int OnGameCodeInitted = 3;
    public const int OnGameStart = 4;
    public const int OnCharacterInit = 5;
    
    public const int OnGameExit = 7;
    public const int OnControlCharacterChanged = 8;
    public const int OnFocusPointUpdate = 9;
    
    public const int OnJoystickStateChanged = 10;

    public const int OnFPSUpdate = 11;
    public const int OnGameUpdate = 12;

    public const int OnTerminalMessageUpdate = 21;
    
    public const int OnInteractTagRefresh = 31;

    public const int OnBTreeHotUpdate = 41;
    public const int OnFSMHotUpdate = 42;
    
    public const int SetCursorState = 52;

    // 界面加载完成
    public const int OnSceneLoaded = 60;
}