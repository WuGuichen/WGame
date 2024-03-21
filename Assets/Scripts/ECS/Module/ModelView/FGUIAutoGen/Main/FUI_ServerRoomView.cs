/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Main
{
	public partial class FUI_ServerRoomView: GComponent
	{
		public GList serverList;
		public WGame.UI.Common.FUI_BtnNormal btnReady;
		public WGame.UI.Common.FUI_BtnNormal btnStart;
		public GButton btnClose;
		public GTextInput inputIP;
		public const string URL = "ui://x1rerq2kpmrzb";

		public static FUI_ServerRoomView CreateInstance()
		{
			return (FUI_ServerRoomView)UIPackage.CreateObject("Main", "ServerRoomView");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			serverList = (GList)GetChildAt(2);
			btnReady = (WGame.UI.Common.FUI_BtnNormal)GetChildAt(3);
			btnStart = (WGame.UI.Common.FUI_BtnNormal)GetChildAt(4);
			btnClose = (GButton)GetChildAt(5);
			inputIP = (GTextInput)GetChildAt(7);
		}
	}
}
