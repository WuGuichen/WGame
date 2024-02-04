using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class SandboxSystem : MonoBehaviour
{
	[SerializeField] private List<Transform> SpawnPoints;
	[SerializeField] private Transform sceneRoot;
	[SerializeField] private Transform goapTrans;

	private Systems _systems;
	private Contexts _contexts;

	private Systems _rigidSystems;
	private Systems _lateFixedUpdateSystems;
	private SensorDetectSystems _detectSystems;
	private GameEventSystems _gameEventSystems;
	private ProcessMotionSystems _processMotionSystems;

	private void Awake()
	{
        _contexts = Contexts.sharedInstance;
        
		_systems = new Feature("Game Systems");
		_rigidSystems = new Feature("Rigidbody Systems");
		_lateFixedUpdateSystems = new Feature("LateFixedUpdate Systems");
		_detectSystems = new SensorDetectSystems(_contexts);
		_gameEventSystems = new GameEventSystems(_contexts);
		// _processCharacterStateSystems = new ProcessCharacterStateSystems(_contexts);
		_processMotionSystems = new ProcessMotionSystems(_contexts);
		
		// 注意系统顺序
		_systems.Add(new AnimSpeedSystem(_contexts));
		
		_systems.Add(new CharacterOnGroundSystem(_contexts));
		_systems.Add(new UpdateDeviceInputSignalSystem(_contexts));
		// _systems.Add(new UpdateAttackInputSystem(_contexts));
		// _systems.Add(new UpdateStepInputSystem(_contexts));
		// _systems.Add(new CharacterFocusSystem(_contexts));
		_systems.Add(new FocusEntitySystem(_contexts));
		
		_rigidSystems.Add(new MoveCharacterSystem(_contexts));
		_rigidSystems.Add(new RotatePlayerSystem(_contexts));
		_rigidSystems.Add(new UpdateMoveDirectionSystem(_contexts));

		_lateFixedUpdateSystems.Add(new CameraFollowTargetSystem(_contexts));
		_lateFixedUpdateSystems.Add(new CameraRotateSystem(_contexts));
		
		_contexts.meta.factoryService.instance.InitSceneObjectRoot(sceneRoot);
		_contexts.meta.factoryService.instance.InitGOAPRoot(goapTrans);
	}

	private void Start()
	{
		_systems.Initialize();
		_processMotionSystems.Initialize();
		_rigidSystems.Initialize();
		_lateFixedUpdateSystems.Initialize();
		_gameEventSystems.Initialize();
	}

	private void Update()
	{
		_detectSystems.Execute();
		_gameEventSystems.Execute();
		_systems.Execute();
		_processMotionSystems.Execute();
		_systems.Cleanup();
		_processMotionSystems.Cleanup();
		_gameEventSystems.Cleanup();
	}

	private void FixedUpdate()
	{
		_rigidSystems.Execute();
		_lateFixedUpdateSystems.Execute();
		_rigidSystems.Cleanup();
		_lateFixedUpdateSystems.Cleanup();
	}

	private void OnDrawGizmos()
	{
		// if (_contexts == null)
		// 	return;
		// if (_contexts.game.localPlayerEntity != null && _contexts.game.localPlayerEntity.hasGameViewService)
		// {
		// 	Gizmos.DrawWireCube(_contexts.game.localPlayerEntity.gameViewService.service.Position, new Vector3(0.2f,0.2f,0.2f));
		// 	var entity = _contexts.game.localPlayerEntity;
		// 	if(entity.isDropItemSensor)
		// 		Gizmos.DrawWireSphere(entity.gameViewService.service.Position + entity.gameViewService.service.Model.forward, 2f);
		// }
	}
	
}
