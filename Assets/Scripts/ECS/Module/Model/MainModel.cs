// 数据缓存，数据逻辑操作

using UnityEngine;
using UnityTimer;
using WGame.Trigger;
using WtEventType = WGame.Trigger.WtEventType;
using WGame.Runtime;

namespace WGame.UI
{
	public class MainModel : Runtime.Singleton<MainModel>
	{
		private WtEventType eventType;
		private WTrigger trigger;
		
		private bool isFocus;
		public bool isGameStart = false;

		public bool IsBootByBootstrap { get; set; } = false;

		public bool IsFocus
		{
			get => isFocus;
			set
			{
				if (isFocus != value)
				{
					isFocus = value;
					EventCenter.Trigger(EventDefine.OnFocusPointUpdate);
				}
			}
		}
		private Vector3 focusPosition;

		public Vector3 FocusPosition
		{
			get => focusPosition;
			set
			{
				focusPosition = value;
				EventCenter.Trigger(EventDefine.OnFocusPointUpdate);
			}
		}
		
		public MainModel()
		{
			EventCenter.AddListener(EventDefine.OnEnterGameMainView, OnEnterMainView);
			EventCenter.AddListener(EventDefine.OnGameStart, OnGameStart);
			WTriggerMgr.Inst.Init();
			isGameStart = false;
		}

		private void OnGameStart()
		{
			isGameStart = true;
			EventCenter.Trigger(EventDefine.SetCursorState, WEventContext.Get(0));
		}

		// 游戏开始，数据的设置需要在这里进行
		private void OnEnterMainView()
		{
#if UNITY_ANDROID
			IsUseJoystick = true;
#elif UNITY_STANDALONE_WIN
			IsUseJoystick = false;
#endif
			UIManager.OpenView(VDB.GameMainView);
		}

		#region Joystick

		private bool isUseJoystick = false;

		public bool IsUseJoystick
		{
			get => isUseJoystick;
			set
			{
				if (value != isUseJoystick)
				{
					isUseJoystick = value;
					RefreshJoystickViewState();
					EventCenter.Trigger(EventDefine.OnJoystickStateChanged);
				}
			}
		}

		private Vector2 moveDir;

		public Vector2 MoveDir
		{
			get => moveDir;
			set
			{
				if (value != moveDir)
				{
					moveDir = value;
				}
			}
		}
		
		private Vector2 lookDir;

		public bool isLooking = false;
		public Vector2 LookDir
		{
			get => lookDir;
			set
			{
				if (value != lookDir)
				{
					lookDir = value;
				}

				isLooking = true;
			}
		}

		private const float HOLD_TIME_LIMIT = 0.2f;

		private bool isTriggerAttack = false;
		public bool IsTriggerAttack
		{
			get => isTriggerAttack;
			set
			{
				isTriggerAttack = value;
			}
		}

		private float attackTimer = HOLD_TIME_LIMIT;
		public bool isAttackPressing = false;
		private bool isHoldAttack = false;
		public bool IsHoldAttack
		{
			get => isHoldAttack;
			set
			{
				isHoldAttack = value;
			}
		}

		public bool IsReleaseAttack
		{
			set
			{
				if (value)
				{
					isHoldAttack = false;
					attackTimer = HOLD_TIME_LIMIT;
				}
			}
		}

		public bool isDefencePressing = false;

		private float jumpTimer = HOLD_TIME_LIMIT;
		private bool isTriggerSpecial = false;
		public bool IsTriggerSpecial
		{
			get => isTriggerSpecial;
			set
			{
				isTriggerSpecial = value;
			}
		}

		public bool IsReleaseJump
		{
			set
			{
				jumpTimer = HOLD_TIME_LIMIT;
			}
		}
		
		private bool isHoldSpecial = false;
		public bool IsHoldSpecial
		{
			get => isHoldSpecial;
			set
			{
				isHoldSpecial = value;
			}
		}

		public bool isSpecialPressing = false;
		
		private bool isTriggerStep = false;

		public bool IsTriggerStep
		{
			get => isTriggerStep;
			set
			{
				isTriggerStep = value;
			}
		}

		private bool isRunningState = false;

		public bool IsRunningState
		{
			get => isRunningState;
			set => isRunningState = value;
		}

		private bool isTriggerFocus = false;

		public bool IsTriggerFocus
		{
			get => isTriggerFocus;
			set => isTriggerFocus = value;
		}

		public void CleanUpInputs()
		{
			if(isLooking)
				isLooking = false;
			if(isTriggerSpecial)
				isTriggerSpecial = false;
			if(isTriggerAttack)
				isTriggerAttack = false;
			if(isTriggerSpecial)
				isTriggerSpecial = false;
			if(isTriggerStep)
				isTriggerStep = false;
			// if(isDefencing)
			// isDefencing = false;
			// isHoldJump = false;
			if (isTriggerFocus)
				isTriggerFocus = false;
		}

		public void TickInputUpdate(float deltaTime)
		{
			if (isSpecialPressing)
			{
				if (IsHoldSpecial == false && jumpTimer < 0)
				{
					IsHoldSpecial = true;
				}
				jumpTimer -= deltaTime;
			}

			if (isAttackPressing)
			{
				if (IsHoldAttack == false && attackTimer < 0)
				{
					IsHoldAttack = true;
				}

				attackTimer -= deltaTime;
			}
		}
		
		public void RefreshJoystickViewState()
		{
			if (IsUseJoystick)
			{
				if (UIManager.IsViewOpen(VDB.JoystickView))
					return;
				UIManager.OpenView(VDB.JoystickView);
			}
			else
			{
				if (!UIManager.IsViewOpen(VDB.JoystickView))
					return;
				UIManager.CloseView(VDB.JoystickView);
			}
		}
		
		#endregion

		public void OnGameMainViewTopItemClick(int index)
		{

			switch (index)
			{
				case 0:
					EntityUtils.SelectRandomCharacter();
					break;
				case 1:
					EntityUtils.GenCharacter(2);
					break;
				case 2:
					EntityUtils.GenCharacter(1);
					break;
				case 3:
					EntityUtils.GenRandomWeapon();
					break;
				case 4:
					var entity = EntityUtils.GetCameraEntity();
					EntityUtils.DropEntityWeapon(entity);
					break;
				default:
					break;
			}
		}
		public void OnGameMainViewItemClick(int index)
		{
			switch (index)
			{
				case 0:
					if (isGameStart)
						break;
					EventCenter.Trigger(EventDefine.OnGameStart);
						ActionHelper.DoSetCharacterCameraByID(10000001);
					break;
				case 1:
					// UIManager.OpenView(VDB.CommandView);
					UIManager.OpenView(VDB.TerminalView);
					break;
				case 2:
					UIManager.OpenView(VDB.SettingView);
					break;
				case 3:
					ActionHelper.DoExitGame();
					break;
				default:
					break;
			}	
		}

		public void OnPresetCommandClick(int idx)
		{
			switch (idx)
			{
				case 0:
					// var weapon = Contexts.sharedInstance.weapon.CreateEntity();
					// Contexts.sharedInstance.meta.factoryService.instance.CreateWeapon(weapon, 40000);
					// ActionHelper.DoEquipWeapon(Contexts.sharedInstance.game.GetEntityWithEntityID(10000001), weapon);
					break;
				case 1:
					ActionHelper.DoReachPoint(Contexts.sharedInstance.game.GetEntityWithEntityID(10000002)
						, Contexts.sharedInstance.game.GetEntityWithEntityID(10000001).position.value);
					break;
				default:
					break;
			}
		}
		
		public bool IsShowTag { get; private set; }

		public Interactable curInteractTag { get; private set; }

		public void ResetInteractTag(Interactable inter, float sqrDist)
		{
			// 取消交互
			if (sqrDist < 0)
			{
				IsShowTag = false;
				EventCenter.Trigger(EventDefine.OnInteractTagRefresh);
			}
			else
			{
				curInteractTag = inter;
				IsShowTag = true;
				EventCenter.Trigger(EventDefine.OnInteractTagRefresh);
			}
		}

		public void OnInteractTagClick()
		{
			curInteractTag?.Interact();
			IsShowTag = false;
			EventCenter.Trigger(EventDefine.OnInteractTagRefresh);
		}

		private float _frames;
		public float frames
		{
			get => _frames;
			set
			{
				_frames = value;
				EventCenter.Trigger(EventDefine.OnFPSUpdate);
			}
		}
	}
}
