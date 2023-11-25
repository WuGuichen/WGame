using WGame.UI.Terminal;using FairyGUI;
using UnityTimer;

namespace WGame.UI
{
	public class TerminalView: BaseView
	{
		public override string ViewName => "TerminalView";
		private readonly FUI_TerminalView ui = FUI_TerminalView.CreateInstance();
		protected override GObject uiObj => ui;

		private TerminalModel model;

		private FUI_TerminalMsgItem msgObj;

		private Timer focusTimer;

		protected override void CustomInit()
		{
			ui.btnClose.onClick.Set(() =>
			{
				CloseView();
			});
			model = TerminalModel.Inst;
			ui.listMessage.itemRenderer = OnMsgItemRender;
			ui.listMessage.numItems = 1;
			UpdateTerminalMessage();
			
			ui.inputLine.onSubmit.Set(OnInputLineSubmit);	
		}

		protected override void OnRegisterEvent()
		{
			AddEvent(EventDefine.OnTerminalMessageUpdate, UpdateTerminalMessage);
		}

		private void OnInputLineSubmit()
		{
			string text = ui.inputLine.text;
			ui.inputLine.text = "";
			WTerminal.Input(text);
			focusTimer = Timer.Register(0.00001f, () =>
			{
				ui.inputLine.RequestFocus();
				focusTimer = null;
			});
			switch (text)
			{
				case "exit":
					CloseView();
					break;
				case "clear":
					WTerminal.Clear();
					break;
				default:
					model.DoString(text);
					break;
			}
		}

		private void OnMsgItemRender(int idx ,GObject obj)
		{
			msgObj = obj as FUI_TerminalMsgItem;
		}

		private void UpdateTerminalMessage()
		{
			if (msgObj != null)
			{
				msgObj.title.text = WTerminal.Message;
				float offset = msgObj.title.height - ui.listMessage.height;
				if(offset > 0)
					ui.listMessage.container.SetXY(0, -offset);
			}
		}
		protected override void AfterOpen()
		{
			WTerminal.isInTerminal = true;
			ui.inputLine.RequestFocus();
		}
		protected override void BeforeClose()
		{
			WTerminal.isInTerminal = false;
			focusTimer?.Cancel();
			focusTimer = null;
		}
		protected override void OnDestroy()
		{
			ui.listMessage.numItems = 0;
		}
	}
}
