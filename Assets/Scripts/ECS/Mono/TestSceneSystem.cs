using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using WGame.Res;
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
    
	[SerializeField] private List<Transform> SpawnPoints;
	[SerializeField] private Transform sceneRoot;
	[SerializeField] private UnityEngine.Rendering.Volume volume;

	private Systems _systems;
    
	private Systems _rigidSystems;
	private Systems _lateFixedUpdateSystems;
	private Systems _lateUpdateSystems;
	private SensorDetectSystems _detectSystems;
	private GameEventSystems _gameEventSystems;
	private ProcessMotionSystems _processMotionSystems;
	private VMSystems _vmSystems;

	private Dictionary<int, GameEntity> inittedEntities;

    void PreInit()
    {
	    inittedEntities = new Dictionary<int, GameEntity>();
        Random.InitState((int)System.DateTime.Now.Ticks);
		PreInitModel();
	    RegisterEvents();
        _contexts = Contexts.sharedInstance;
        _settingContext = _contexts.setting;

        _settingContext.SetGameSetting(gameSetting);
        var _services = new Services(
            new InputServiceImplementation(),
            new CameraServiceImplementation(),
            YooassetManager.Inst,
            new FactoryServiceImplementation(_contexts),
            new PlayModeTimeServiceImplementation()
            );
        _registrationSystems = new ServiceRegistrationSystems(_contexts, _services);
        _gameSystems = new GameSystems(_contexts);
        _registrationSystems.Initialize();
        _gameSystems.Initialize();

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
		_systems.Add(new AnimSpeedSystem(_contexts));
		
		_systems.Add(new CharacterOnGroundSystem(_contexts));
		_systems.Add(new UpdateInputSignalSystem(_contexts));
		_systems.Add(new UpdateAttackInputSystem(_contexts));
		_systems.Add(new UpdateStepInputSystem(_contexts));
		_systems.Add(new UpdateFocusInputSystem(_contexts));
		_systems.Add(new CharacterFocusSystem(_contexts));
		_systems.Add(new ThrustCharacterSystem(_contexts));
		_systems.Add(new AIAgentUpdateSystem(_contexts));
		_systems.Add(new RefreshCharacterUISystem(_contexts));
		
		_rigidSystems.Add(new MoveCharacterSystem(_contexts));
		_rigidSystems.Add(new RotatePlayerSystem(_contexts));
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

	    TickManager.Inst.UpdateTick(_contexts.meta.timeService.instance.deltaTime);
	    _vmSystems.Execute();
	    _gameSystems.Execute();
	    _detectSystems.Execute();
	    _gameEventSystems.Execute();
	    _systems.Execute();
	    _processMotionSystems.Execute();
	    _systems.Cleanup();
	    _processMotionSystems.Cleanup();
	    _gameEventSystems.Cleanup();

	    if (Input.GetKeyDown(KeyCode.LeftAlt))
	    {
			EventCenter.Trigger(EventDefine.SetCursorState, WEventContext.Get(Cursor.visible ? 0 : 1));
	    }
    }

    private void FixedUpdate()
    {
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
		_lateUpdateSystems.Cleanup();
		
		WTriggerMgr.Inst.OnEndUpdate();
		UnityEngine.Profiling.Profiler.BeginSample("OptimizeBVH");
		EntityUtils.GameBVH.Optimize();
		UnityEngine.Profiling.Profiler.EndSample();
    }

    private void OnDrawGizmos()
    {
		EntityUtils.GameBVH.DrawAllBounds();
    }

    private void OnGUI()
    {
	    // if (GUILayout.Button("添加敌人 press P"))
	    // {
		   //  _contexts.meta.factoryService.instance.CreateEnemy(_contexts.game.CreateEntity(), new Vector3(1,1,2), 2);
	    // }
	    // GUILayout.Label("添加武器 press E");
    }

    void PreInitModel()
    {
	    MainModel.Inst.InitInstance();
	    SettingModel.Inst.SetVolume(volume);
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
		    var entity = _contexts.game.CreateEntity();
		    inittedEntities[int.Parse(child.name)] = entity;
		    if (child.activeSelf)
		    {
			    _contexts.meta.factoryService.instance.GenCharacter(child.gameObject);
		    }
	    }
    }

    void OnGameAssetsManagerInited()
    {
	    WLangMgr.Inst.LordInitCode(TerminalModel.Inst.Interp);
	    // Debug.Log("Init AssetsManager Success!");
		_contexts.meta.factoryService.instance.InitSceneObjectRoot(sceneRoot);
		// gameObject.GetComponent<UIManager>().OpenView(ViewDB.MainView);
		// gameObject.GetComponent<UIManager>().OpenView("MainView");
		// UIManager.OpenView(ViewDB.GameMainView);
		// Timer.Register(2f, () =>
		// {
		// 	UIManager.CloseView(ViewDB.GameMainView, true);
		// });
		// Timer.Register(4f, () =>
		// {
		// 	UIManager.OpenView(ViewDB.MainView);
		// });
		// Timer.Register(6f, () =>
		// {
		// 	UIManager.CloseView(ViewDB.MainView, true);
		// });
		// Timer.Register(8f, () =>
		// {
		// 	UIManager.OpenView(ViewDB.MainView);
		// });
    }
}
