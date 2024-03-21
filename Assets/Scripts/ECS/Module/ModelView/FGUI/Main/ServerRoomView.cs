using System.Collections.Generic;
using WGame.UI.Main;
using FairyGUI;

namespace WGame.UI
{
	public class ServerRoomView: BaseView
	{
		public override string ViewName => "ServerRoomView";
		private readonly FUI_ServerRoomView ui = FUI_ServerRoomView.CreateInstance();
		protected override GObject uiObj => ui;
		private Dictionary<ulong, FUI_ServerItem> _itemDict =new();

		protected override void CustomInit()
		{
			ui.btnClose.onClick.Add(CloseView);			
			ui.btnReady.onClick.Add(OnClickReady);
			ui.btnStart.onClick.Add(OnClickStart);
			ui.serverList.itemRenderer = ServerItemRenderer;
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
			WNetMgr.Inst.SetConnectData(IP, 7777);
			if (WNetMgr.Inst.StartClient())
			{
				var ready = !WNetMgr.Inst.IsReady;
				ui.btnReady.text = ready ? "取消" : "准备";
				// 刷新自己的状态
				WNetMgr.Inst.RefreshPlayRoomInfo(WNetMgr.Inst.LocalClientId, ready);
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
			WLogger.Print("开始游戏");
			CloseView();
		}
		
		private void RefreshServerList()
		{
			_itemDict.Clear();
			ui.serverList.numItems = WNetMgr.Inst.AllPlayerInfo.Count;
		}
		
		protected override void AfterOpen()
		{
			RefreshServerList();
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
			
		}
		protected override void OnDestroy()
		{
			_itemDict.Clear();
		}
	}
}
