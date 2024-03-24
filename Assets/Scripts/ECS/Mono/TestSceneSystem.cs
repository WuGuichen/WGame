using UnityEngine;
using WGame.Ability;
using Random = UnityEngine.Random;
using WGame.UI;
using WGame.Runtime;
using WGame.Trigger;

public class TestSceneSystem : MonoBehaviour
{
    public GameSetting gameSetting;
    private Contexts _contexts;
    private SettingContext _settingContext;
    
    private ServiceRegistrationSystems _registrationSystems;
    private GameSystems _gameSystems;
    
	private OtherSystems _otherSystems;
	private FixedUpdateSystems _rigidSystems;
	private LateFixedUpdateSystems _lateFixedUpdateSystems;
	private LateUpdateSystems _lateUpdateSystems;
	private SensorDetectSystems _detectSystems;
	private GameEventSystems _gameEventSystems;
	private ProcessMotionSystems _processMotionSystems;
	private VMSystems _vmSystems;
	private ITimeService _timeService;

	private bool isDirectlyLoad = false;

    void PreInit()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
		PreInitModel();
	    RegisterEvents();
        _contexts = Contexts.sharedInstance;
        _settingContext = _contexts.setting;

        _settingContext.SetGameSetting(gameSetting);
        var _inputAgent = new WInputAgentMyController();
        SettingModel.Inst.InputAgent = _inputAgent;
        var _services = new Services(
            _inputAgent,
            new CameraServiceImplementation(),
            YooassetManager.Inst,
            new FactoryServiceImplementation(_contexts),
            new PlayModeTimeServiceImplementation()
            );
        _registrationSystems = new ServiceRegistrationSystems(_contexts, _services);
        _registrationSystems.Initialize();
        
        _gameSystems = new GameSystems(_contexts);
        _gameSystems.Initialize();

        _timeService = _contexts.meta.timeService.instance;
    }
    
    private void Awake()
    {
        PreInit();
        _otherSystems = new OtherSystems(_contexts);
        _lateFixedUpdateSystems = new LateFixedUpdateSystems(_contexts);
		_lateUpdateSystems = new LateUpdateSystems(_contexts);
		_detectSystems = new SensorDetectSystems(_contexts);
		_gameEventSystems = new GameEventSystems(_contexts);
		_processMotionSystems = new ProcessMotionSystems(_contexts);
		_vmSystems = new VMSystems(_contexts);
		_rigidSystems = new FixedUpdateSystems(_contexts);
    }

    void Start()
    {
	    WNetMgr.Inst.InitInstance();
	    
		_vmSystems.Initialize();
		_otherSystems.Initialize();
		_processMotionSystems.Initialize();
		_rigidSystems.Initialize();
		_lateFixedUpdateSystems.Initialize();
		_lateUpdateSystems.Initialize();
		_gameEventSystems.Initialize();

		lastUpdateFPSTime = Time.realtimeSinceStartup;

		if (YooassetManager.Inst.IsInitted)
		{
			OnGameSystemsInitted();
		}
    }

    private float lastUpdateFPSTime;
    private float updateFPSSpanTime = 0.1f;
    private float FPS;
    private int frames = 0;

    void Update()
    {
	    UpdateBeforeSystems();
	    _vmSystems.Execute();
	    _gameSystems.Execute();
	    _detectSystems.Execute();
	    _gameEventSystems.Execute();
	    _otherSystems.Execute();
		_contexts.meta.factoryService.instance.GOAPRunner?.OnUpdate();
	    _processMotionSystems.Execute();
	    _otherSystems.Cleanup();
	    _processMotionSystems.Cleanup();
	    _gameEventSystems.Cleanup();

	    if (Input.GetKeyDown(KeyCode.LeftAlt))
	    {
			EventCenter.Trigger(EventDefine.SetCursorState, Cursor.visible ? 0 : 1);
	    }
    }

    private void UpdateBeforeSystems()
    {
	    _timeService.UpdateDeltaTime(Time.deltaTime);
	    _timeService.UpdateRealTimeSinceStart(Time.realtimeSinceStartup);
	    if (MainModel.Inst.frames >= 0)
	    {
		    frames++;
		    if (Time.realtimeSinceStartup - lastUpdateFPSTime >= updateFPSSpanTime)
		    {
			    FPS = frames / (Time.realtimeSinceStartup - lastUpdateFPSTime);
			    lastUpdateFPSTime = Time.realtimeSinceStartup;
			    MainModel.Inst.frames = FPS;
			    frames = 0;
		    }
	    }

	    EventCenter.Trigger(EventDefine.OnGameUpdate);

	    TickManager.Inst.UpdateTick(_timeService.DeltaTime);
    }

    private void FixedUpdate()
    {
	    _timeService.UpdateFixedDeltaTime(Time.fixedDeltaTime);
	    _rigidSystems.Execute();
	    _lateFixedUpdateSystems.Execute();
	    _rigidSystems.Cleanup();
	    _lateFixedUpdateSystems.Cleanup();
    }
    
    private void OnApplicationQuit()
    {
        EventCenter.Trigger(EventDefine.OnGameExit);
        _vmSystems.TearDown();
        _gameSystems.TearDown();
        _registrationSystems.TearDown();
        _settingContext.RemoveGameSetting();
        WLangMgr.Inst.OnDispose();
        WNetMgr.Inst.OnDispose();
        EventCenter.RemoveListener(EventDefine.OnGameResourcesLoaded, OnGameStart);
        EntityUtils.Dispose();
        ActionHelper.Dispose();
    }

    private void LateUpdate()
    {
		_lateUpdateSystems.Execute();	
		_contexts.meta.factoryService.instance.GOAPRunner?.OnLateUpdate();
		_lateUpdateSystems.Cleanup();
		
		WTriggerMgr.Inst.OnEndUpdate();
		UnityEngine.Profiling.Profiler.BeginSample("BvhOptimize");
		EntityUtils.BvhRed.Optimize();
		UnityEngine.Profiling.Profiler.EndSample();
    }

    void PreInitModel()
    {
	    MainModel.Inst.InitInstance();
	    SettingModel.Inst.SetVolume();
	    WInputManager.Inst.InitInstance();
    }

    void RegisterEvents()
    {
	    EventCenter.AddListener(EventDefine.OnGameStart, OnGameStart);
	    EventCenter.AddListener(EventDefine.OnGameAssetsManagerInitted, OnGameSystemsInitted);
	    EventCenter.AddListener(EventDefine.SetCursorState, (_contexts) =>
	    {
		    var value = _contexts.AsInt() > 0;
		    Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
		    Cursor.visible = Cursor.lockState != CursorLockMode.Locked;
	    });
    }
    
    void OnGameStart()
    {
	    EntityUtils.InitCharacterRoot();
    }

    void OnGameSystemsInitted()
    {
	    WLangMgr.Inst.LordInitCode(TerminalModel.Inst.Interp);
		_contexts.meta.factoryService.instance.InitSceneObjectRoot();
		_contexts.meta.factoryService.instance.InitGOAPRoot();
		WAbilityMgr.Inst.Initialize(new AbilityAssetLoader());
		EventCenter.Trigger(EventDefine.OnGameSystemsInitted);
    }
}
