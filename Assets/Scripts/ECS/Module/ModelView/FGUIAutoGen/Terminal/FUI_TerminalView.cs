/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Terminal
{
	public partial class FUI_TerminalView: GComponent
	{
		public GList listMessage;
		public GTextInput inputLine;
		public GTextInput inputBoard;
		public WGame.UI.Terminal.FUI_btnNormal1 btnSend;
		public GButton btnClose;
		public const string URL = "ui://xfar8tnuq56t0";

		public static FUI_TerminalView CreateInstance()
		{
			return (FUI_TerminalView)UIPackage.CreateObject("Terminal", "TerminalView");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			listMessage = (GList)GetChildAt(1);
			inputLine = (GTextInput)GetChildAt(3);
			inputBoard = (GTextInput)GetChildAt(5);
			btnSend = (WGame.UI.Terminal.FUI_btnNormal1)GetChildAt(6);
			btnClose = (GButton)GetChildAt(7);
		}
	}
}
