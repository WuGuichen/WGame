// 数据缓存，数据逻辑操作

using UnityEngine;
using WGame.Trigger;
using WtEventType = WGame.Trigger.WtEventType;
using WGame.Runtime;
using WGame.UI.Main;

namespace WGame.UI
{
	public class MainModel : Runtime.Singleton<MainModel>
	{
		private WtEventType eventType;
		private WTrigger trigger;

		private bool isFocus;
		public bool isGameStart = false;

		public bool HasWeakPoint { get; set; } = false;
		
		private Vector3 weakPosition;

		public Vector3 WeakPosition
		{
			get => weakPosition;
			set
			{
				weakPosition = value;
				EventCenter.Trigger(EventDefine.OnWeakPointUpdate);
			}
		}
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
			EventCenter.Trigger(EventDefine.SetCursorState, 0);
		}

		// 游戏开始，数据的设置需要在这里进行
		private void OnEnterMainView()
		{
#if UNITY_ANDROID
			IsUseJoystick = true;
#elif UNITY_STANDALONE_WIN
			IsUseJoystick = false;
#endif
#if UNITY_EDITOR
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
			set { isTriggerAttack = value; }
		}

		private float attackTimer = HOLD_TIME_LIMIT;
		public bool isAttackPressing = false;
		private bool isHoldAttack = false;

		public bool IsHoldAttack
		{
			get => isHoldAttack;
			set { isHoldAttack = value; }
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
			set { isTriggerSpecial = value; }
		}

		public bool IsReleaseJump
		{
			set { jumpTimer = HOLD_TIME_LIMIT; }
		}

		private bool isHoldSpecial = false;

		public bool IsHoldSpecial
		{
			get => isHoldSpecial;
			set { isHoldSpecial = value; }
		}

		public bool isSpecialPressing = false;

		private bool isTriggerStep = false;

		public bool IsTriggerStep
		{
			get => isTriggerStep;
			set { isTriggerStep = value; }
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
			if (isLooking)
				isLooking = false;
			if (isTriggerSpecial)
				isTriggerSpecial = false;
			if (isTriggerAttack)
				isTriggerAttack = false;
			if (isTriggerSpecial)
				isTriggerSpecial = false;
			if (isTriggerStep)
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

		public void OnGameMainViewItemClick(string index)
		{
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
		
		private Vector3 _originCamPos;
		private Quaternion _originCamRot;

		private void InitGameStartInfo()
		{
			var trans = Contexts.sharedInstance.meta.mainCameraService.service.Root;
			_originCamPos = trans.position;
			_originCamRot = trans.rotation;
		}
		
		public void StartGame(bool isServer = false)
		{
			InitGameStartInfo();
			if (isGameStart)
			{
				return;
			}

			if (isServer)
			{
				if (!WNetMgr.Inst.IsAllPlayerReady())
				{
					WLogger.Print("还有人未准备");
					return;
				}

				// 通知服务器
				WNetMgr.Inst.StartGame();
			}
			else
			{
				if (WNetMgr.Inst.IsServer)
				{
					SetMainViewBtnState(MainDefine.Btn_StartOffline, "中止游戏");
				}
				else
				{
					SetMainViewBtnState(MainDefine.Btn_StartOffline, "返回主菜单");
				}
				EventCenter.Trigger(EventDefine.OnGameStart);
			}
			UIManager.CloseView(VDB.ServerRoomView);
		}

		public void BackToMainView(bool isServer = false)
		{
			if (!isGameStart)
			{
				return;
			}

			if (isServer)
			{
				// 通知服务器
				WNetMgr.Inst.BackToMainView();
			}
			else
			{
				var trans = Contexts.sharedInstance.meta.mainCameraService.service.Root;
				trans.position = _originCamPos;
				trans.rotation = _originCamRot;
				EventCenter.Trigger(EventDefine.OnBackToMainView);
				isGameStart = false;
			}
		}

		#region 主界面按钮

		private SparseSet<FUI_MainListItem> _curMainBtnDict = new();
		private SparseSet<MainBtnInfo> _mainBtnInfoDict = new();

		public void InitMainBtnDict()
		{
			var list = MainDefine.AllMainBtnList;
			for (var i = 0; i < list.Count; i++)
			{
				var info = list[i];
				_mainBtnInfoDict.Add(info.ID, info);
			}
		}

		public void RegisterMainViewBtn(FUI_MainListItem item)
		{
			if (_curMainBtnDict.IsContain(item.Info.ID))
			{
				return;
			}
			_curMainBtnDict.Add(item.Info.ID, item);
		}

		public void UnregisterMainViewBtn(FUI_MainListItem item)
		{
			if (_curMainBtnDict.IsContain(item.Info.ID))
			{
				_curMainBtnDict.Remove(item.Info.ID);
			}
		}

		public void SetMainViewBtnState(int id, string text)
		{
			if (_mainBtnInfoDict.TryGet(id, out var info))
			{
				info.Name = text;
			}
			else
			{
				WLogger.Error("非法按钮ID: " + id);
				return;
			}
			
			if (_curMainBtnDict.TryGet(id, out var item))
			{
				item.RefreshInfo();
			}
		}
		public void SetMainViewBtnState(int id, bool isShow)
		{
			if (_mainBtnInfoDict.TryGet(id, out var info))
			{
				info.IsShow = isShow;
			}
			else
			{
				WLogger.Error("非法按钮ID: " + id);
				return;
			}
			if (_curMainBtnDict.TryGet(id, out var item))
			{
				item.RefreshInfo();
			}
		}
		
		public MainBtnInfo GetMainBtnInfo(int id)
		{
			if(_mainBtnInfoDict.TryGet(id, out var info))
			{
				return info;
			}

			return MainBtnInfo.Empty;
		}

		public void OnClickMainBtn(MainBtnInfo info)
		{
			OnClickMainBtn(info.ID);	
		}

		public void OnClickMainBtn(int id)
		{
			switch (id)
			{
				case MainDefine.Btn_StartOffline:
					if (isGameStart)
					{
						BackToMainView(WNetMgr.Inst.IsServer);
					}
					else
					{
						EntityUtils.GenCharacter(1, true);
						StartGame();
					}
					break;
				case MainDefine.Btn_StartServer:
					UIManager.OpenView(VDB.ServerRoomView);
					break;
				case MainDefine.Btn_StartHost:
					if (WNetMgr.Inst.IsHost == false)
					{
						if (WNetMgr.Inst.StartHost())
						{
							UIManager.OpenView(VDB.ServerRoomView);
						}
					}
					else
					{
						UIManager.OpenView(VDB.ServerRoomView);
					}
					break;
				case MainDefine.Btn_ShutDownServer:
					WNetMgr.Inst.ShutDown();
					break;
				case MainDefine.Btn_CmdList:
					UIManager.OpenView(VDB.TerminalView);
					break;
				case MainDefine.Btn_Setting:
					UIManager.OpenView(VDB.SettingView);
					break;
				case MainDefine.Btn_Quit:
					WNetMgr.Inst.ShutDown();
					WLogger.Info("结束游戏");
					ActionHelper.DoExitGame();
					break;
			}	
		}

		#endregion
	}
}
