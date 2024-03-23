using System.Collections.Generic;
using WGame.UI.Main;
using FairyGUI;
using UnityEngine;
using UnityTimer;
using WGame.Runtime;

namespace WGame.UI
{
	public class ServerRoomView: BaseView
	{
		public override string ViewName => "ServerRoomView";
		private readonly FUI_ServerRoomView ui = FUI_ServerRoomView.CreateInstance();
		protected override GObject uiObj => ui;
		private Dictionary<ulong, FUI_ServerItem> _itemDict =new();
		private bool _isNoServerList = false;
		
		private GoWrapper _characterWrapper;
		private int _curCharacterIDIndex = -1;
		private Vector2 _charTouchPosBefore;
		private bool _isLoadingCharacter = false;

		protected override void CustomInit()
		{
			ui.btnClose.onClick.Add(CloseView);			
			ui.btnReady.onClick.Add(OnClickReady);
			ui.btnStart.onClick.Add(OnClickStart);
			ui.serverList.itemRenderer = ServerItemRenderer;
			ui.modelHandler.onTouchBegin.Add(evt =>
			{
				if (evt.inputEvent.button != 0)
				{
					return;
				}
				evt.CaptureTouch();
				_charTouchPosBefore = evt.inputEvent.position;
			});
			ui.modelHandler.onTouchMove.Add(evt =>
			{
				var newPos = evt.inputEvent.position;
				var dir = newPos - _charTouchPosBefore;
				_charTouchPosBefore = newPos;
				OnDragModel(dir);
			});
			ui.btnArrowLeft.onClick.Add(evt =>
			{
				SetShowCharacterModel(_curCharacterIDIndex-1);	
			});
			ui.btnArrowRight.onClick.Add(evt =>
			{
				SetShowCharacterModel(_curCharacterIDIndex+1);	
			});
		}

		private void OnDragModel(Vector2 dir)
		{
			if (_characterWrapper.wrapTarget != null)
			{
				var trans = _characterWrapper.wrapTarget.transform;
				trans.Rotate(Vector3.up, -dir.x);
			}
		}

		private void ServerItemRenderer(int index, GObject obj)
		{
			var item = obj as FUI_ServerItem;
			var info = WNetMgr.Inst.AllPlayerInfo[index];
			item.SetData(info);
			_itemDict.Add(info.id, item);
		}

		private string IP => ui.inputIP.inputTextField.text;

		private void OnClickReady()
		{
			WNetMgr.Inst.SetConnectData(IP, WNetMgr.NetPort);
			if (WNetMgr.Inst.StartClient())
			{
				// 刷新自己的状态
				WNetMgr.Inst.RefreshPlayRoomInfo(WNetMgr.Inst.LocalClientId, !WNetMgr.Inst.IsReady);
			}
		}

		private void RefreshItem(ulong id)
		{
			if (_itemDict.TryGetValue(id, out var item))
			{
				item.Refresh();
			}
		}

		private void OnClickStart()
		{
			MainModel.Inst.StartGame(true);
		}
		
		private void RefreshServerList()
		{
			_itemDict.Clear();
			ui.serverList.numItems = WNetMgr.Inst.AllPlayerInfo.Count;
			_isNoServerList = WNetMgr.Inst.AllPlayerInfo.Count == 0;
			RefreshReadyState();
		}

		private void RefreshReadyState()
		{
			if (_isNoServerList)
			{
				ui.btnReady.text = "加入";
			}
			else
			{
				var ready = WNetMgr.Inst.IsReady;
				ui.btnReady.text = ready ? "取消" : "准备";
			}

			WLogger.Print("刷新，" + WNetMgr.Inst.IsConnecting);
			if (WNetMgr.Inst.IsConnecting)
			{
				ui.txtMessage.text = MainDefine.MessageStartClient + IP + "...";
			}
			else
			{
				ui.txtMessage.text = "";
			}
			// ui.inputIP.visible = !WNetMgr.Inst.IsConnected;
		}

		protected override void AfterOpen()
		{
			ui.btnStart.visible = WNetMgr.Inst.IsServer;
			RefreshServerList();
			SetShowCharacterModel(NetPlayerInfoModel.Inst.CurSelectCharacterIndex);
		}

		private void DestroyShowCharacterModel()
		{
			_curCharacterIDIndex = -1;
			if (_characterWrapper != null)
			{
				_characterWrapper.Dispose();
				_characterWrapper = null;
			}
		}

		private bool SelectNewCharacter(int idx, out BaseData.CharacterData charData)
		{
			if (!NetPlayerInfoModel.Inst.TryGetCharacterID(idx, out var charId))
			{
				charData = null;
				return false;
			}

			if (!NetPlayerInfoModel.Inst.TryGetCharacterData(charId, out charData))
			{
				return false;	
			}

			_curCharacterIDIndex = idx;
			NetPlayerInfoModel.Inst.SelectPlayer(idx);
			ui.txtCharName.text = charData.Name;
			RefreshArrow();
			WNetMgr.Inst.RefreshPlayRoomInfo(WNetMgr.Inst.LocalClientId, charId);
			return true;
		}

		private void SetShowCharacterModel(int idx)
		{
			if (_isLoadingCharacter)
			{
				return;
			}

			if (idx == _curCharacterIDIndex)
			{
				return;
			}

			if(!SelectNewCharacter(idx, out var data))
			{
				WLogger.Error("无效的角色Index:" + idx);
				return;
			}
			
			if (_characterWrapper == null)
			{
				_characterWrapper = new GoWrapper();
				var holder = ui.model;
				holder.SetNativeObject(_characterWrapper);
			}
			else
			{
				Object.Destroy(_characterWrapper.wrapTarget);
			}
			
			var objData = GameData.Tables.TbObjectData.Get(data.ObjectId);
			_isLoadingCharacter = true;
			YooassetManager.Inst.LoadGameObject(objData.Path, o =>
			{
				var trans = o.transform;
				var anim = trans.GetComponentInChildren<MotionAnimationProcessor>();
				YooassetManager.Inst.LoadAnimationClipAsync("Idle", clip =>
				{
					trans.localPosition = new Vector3(0, 0, 1000);
					trans.localEulerAngles = new Vector3(0, 180, 0);
					anim.OnInit();
					anim.RefreshAnimClip(LocalMotionType.Idle, clip);
					anim.ResetState(true);
					ApplyCharacterWarpper(trans);
				});
			});
		}

		private void ApplyCharacterWarpper(Transform trans)
		{
			Timer.Register(0.1f, () =>
			{
				if (_curCharacterIDIndex < 0)
				{
					Object.Destroy(trans);
					return;
				}
				_characterWrapper.wrapTarget = trans.gameObject;
				_isLoadingCharacter = false;
				trans.localScale = new Vector3(380, 380, 380);
			});
		}

		private void RefreshArrow()
		{
			ui.btnArrowLeft.visible = _curCharacterIDIndex != 0;
			ui.btnArrowRight.visible = _curCharacterIDIndex != MainDefine.Inst.selectablePlayerIDs.Length-1;
		}

		protected override void OnRegisterEvent()
		{
			AddEvent(EventDefine.OnClientChanged, RefreshServerList);
			AddEvent(EventDefine.OnPlayerRoomInfoRefresh, RefreshPlayerInfo);
		}

		private void RefreshPlayerInfo(TAny context)
		{
			RefreshItem(context.AsULong());
		}

		protected override void BeforeClose()
		{
			DestroyShowCharacterModel();
		}
		protected override void OnDestroy()
		{
			_itemDict.Clear();
		}
	}
}
