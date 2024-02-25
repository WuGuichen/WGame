using Entitas;
using Oddworm.Framework;
using UnityEngine;
using Weapon;
using WGame.Res;
using Random = UnityEngine.Random;
using WGame.UI;
using WGame.Runtime;
using WGame.Trigger;

// [DefaultExecutionOrder(-120)]
public class TestSceneSystem : MonoBehaviour
{
    public GameSetting gameSetting;
    private Contexts _contexts;
    private SettingContext _settingContext;
    
    private ServiceRegistrationSystems _registrationSystems;
    private GameSystems _gameSystems;
    
	[SerializeField] private Transform sceneRoot;
	[SerializeField] private UnityEngine.Rendering.Volume volume;
	[SerializeField] private Transform goapTrans;

	private Systems _systems;
    
	private Systems _rigidSystems;
	private Systems _lateFixedUpdateSystems;
	private Systems _lateUpdateSystems;
	private SensorDetectSystems _detectSystems;
	private GameEventSystems _gameEventSystems;
	private ProcessMotionSystems _processMotionSystems;
	private VMSystems _vmSystems;
	private ITimeService _timeService;

    void PreInit()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        GameSceneMgr.Inst.InitEnvironment();
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
        _gameSystems = new GameSystems(_contexts);
        _registrationSystems.Initialize();
        _gameSystems.Initialize();

        _timeService = _contexts.meta.timeService.instance;
        if (YooassetManager.Inst == null)
			gameObject.AddComponent<YooassetManager>();
    }
    
    private void Awake()
    {
        PreInit();
		_systems = new Feature("Game Systems");
		_rigidSystems = new Feature("Rigidbody Systems");
		_lateFixedUpdateSystems = new Feature("LateFixedUpdate Systems");
		_lateUpdateSystems = new Feature("LateUpdate Systems");
		_detectSystems = new SensorDetectSystems(_contexts);
		_gameEventSystems = new GameEventSystems(_contexts);
		_processMotionSystems = new ProcessMotionSystems(_contexts);
		_vmSystems = new VMSystems(_contexts);
		
		// 注意系统顺序
		_systems.Add(new UpdateCharacterDataSystem(_contexts));
		_systems.Add(new AnimSpeedSystem(_contexts));
		
		_systems.Add(new CharacterOnGroundSystem(_contexts));
		_systems.Add(new UpdateDeviceInputSignalSystem(_contexts));
		// _systems.Add(new UpdateAttackInputSystem(_contexts));
		// _systems.Add(new UpdateStepInputSystem(_contexts));
		_systems.Add(new UpdateFocusInputSystem(_contexts));
		// _systems.Add(new CharacterFocusSystem(_contexts));
		_systems.Add(new FocusEntitySystem(_contexts));
		_systems.Add(new AIAgentUpdateSystem(_contexts));
		_systems.Add(new RefreshCharacterUISystem(_contexts));
		
		_rigidSystems.Add(new MoveCharacterSystem(_contexts));
		_rigidSystems.Add(new RotateCharacterSystem(_contexts));
		_rigidSystems.Add(new UpdateMoveDirectionSystem(_contexts));

		_lateFixedUpdateSystems.Add(new CameraFollowTargetSystem(_contexts));
		_lateFixedUpdateSystems.Add(new CameraRotateSystem(_contexts));
		
		_lateUpdateSystems.Add(new DeadCharacterSystem(_contexts));
    }

    // Start is called before the first frame update
    void Start()
    {
		_vmSystems.Initialize();
		_systems.Initialize();
		_processMotionSystems.Initialize();
		_rigidSystems.Initialize();
		_lateFixedUpdateSystems.Initialize();
		_lateUpdateSystems.Initialize();
		_gameEventSystems.Initialize();

		lastUpdateFPSTime = Time.realtimeSinceStartup;
    }

    private float lastUpdateFPSTime;
    private float updateFPSSpanTime = 0.1f;
    private float FPS;
    private int frames = 0;

    // Update is called once per frame
    void Update()
    {
	    UpdateBeforeSystems();
	    _vmSystems.Execute();
	    _gameSystems.Execute();
	    _detectSystems.Execute();
	    _gameEventSystems.Execute();
	    _systems.Execute();
		_contexts.meta.factoryService.instance.GOAPRunner?.OnUpdate();
	    _processMotionSystems.Execute();
	    _systems.Cleanup();
	    _processMotionSystems.Cleanup();
	    _gameEventSystems.Cleanup();

	    if (Input.GetKeyDown(KeyCode.LeftAlt))
	    {
			EventCenter.Trigger(EventDefine.SetCursorState, WEventContext.Get(Cursor.visible ? 0 : 1));
	    }
	    if(Input.GetKeyDown(KeyCode.L))
	    {
			EntityUtils.GetCameraEntity().notice.service.Notice(MessageDB.Getter.GetBehitted(new ContactInfo(){part = EntityPartType.Evasion}));
	    }
    }

    // ReSharper disable Unity.PerformanceAnalysis
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
        EventCenter.RemoveListener(EventDefine.OnGameResourcesLoaded, OnGameStart);
        EntityUtils.Dispose();
    }

    private void LateUpdate()
    {
		_lateUpdateSystems.Execute();	
		_contexts.meta.factoryService.instance.GOAPRunner?.OnLateUpdate();
		_lateUpdateSystems.Cleanup();
		
		WTriggerMgr.Inst.OnEndUpdate();
		EntityUtils.BvhRed.Optimize();
    }

    private void OnDrawGizmos()
    {
		// EntityUtils.BvhRed.DrawAllBounds();
    }

    void PreInitModel()
    {
	    MainModel.Inst.InitInstance();
	    SettingModel.Inst.SetVolume(volume);
	    WInputManager.Inst.InitInstance();
    }

    void RegisterEvents()
    {
	    EventCenter.AddListener(EventDefine.OnGameStart, OnGameStart);
	    EventCenter.AddListener(EventDefine.OnGameAssetsManagerInitted, OnGameAssetsManagerInited);
	    EventCenter.AddListener(EventDefine.SetCursorState, (_contexts) =>
	    {
		    var value = _contexts.pInt > 0;
		    Cursor.visible = value;
		    Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
	    });
    }
    
    void OnGameStart()
    {
	    var characterRoot = GameSceneMgr.Inst.editCharacterRoot;
	    for (int i = 0; i < characterRoot.childCount; i++)
	    {
		    var child = characterRoot.GetChild(i).gameObject;
		    if (child.activeSelf)
		    {
			    _contexts.meta.factoryService.instance.GenCharacter(child.gameObject);
		    }
	    }
    }

    void OnGameAssetsManagerInited()
    {
	    WLangMgr.Inst.LordInitCode(TerminalModel.Inst.Interp);
		_contexts.meta.factoryService.instance.InitSceneObjectRoot(sceneRoot);
		_contexts.meta.factoryService.instance.InitGOAPRoot(goapTrans);
    }
}
