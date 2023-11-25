using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using WGame.Res;
using Random = UnityEngine.Random;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string firstSceneName;
    public GameSetting gameSetting;

    private Contexts _contexts;
    private SettingContext _settingContext;

    private ServiceRegistrationSystems _registrationSystems;
    private GameSystems _gameSystems;
    
    public static bool isQuitting { get; private set; }

    private void Awake()
    {
        Application.quitting += delegate { isQuitting = true; };
        DoAwake();
    }

    void DoAwake()
    {
        DontDestroyOnLoad(this);
        _contexts = Contexts.sharedInstance;
        _settingContext = _contexts.setting;
        
        Random.InitState((int)System.DateTime.Now.Ticks);
        
        LoadSettings();

        var _services = new Services(
            new InputServiceImplementation(),
            new CameraServiceImplementation(),
            YooassetManager.Inst,
            new FactoryServiceImplementation(_contexts),
            new PlayModeTimeServiceImplementation()
            );
        _registrationSystems = new ServiceRegistrationSystems(_contexts, _services);
        _gameSystems = new GameSystems(_contexts);
    }

    private void Start()
    {
        _registrationSystems.Initialize();
        _gameSystems.Initialize();
        SceneManager.LoadSceneAsync(firstSceneName, LoadSceneMode.Additive);
    }

    private void Update()
    {
        _gameSystems.Execute();
    }

    void LoadSettings()
    {
        _settingContext.SetGameSetting(gameSetting);
    }

    private void OnApplicationQuit()
    {
        _gameSystems.TearDown();
        _registrationSystems.TearDown();
    }
}
