/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Main
{
	public partial class FUI_ServerRoomView: GComponent
	{
		public GButton btnClose;
		public GList serverList;
		public WGame.UI.Common.FUI_BtnNormal btnReady;
		public WGame.UI.Common.FUI_BtnNormal btnStart;
		public GTextInput inputIP;
		public GTextField txtMessage;
		public GGroup window;
		public GGraph modelHandler;
		public GGraph model;
		public WGame.UI.Common.FUI_BtnArrow btnArrowRight;
		public WGame.UI.Common.FUI_BtnArrow btnArrowLeft;
		public GTextField txtCharName;
		public GGroup character;
		public const string URL = "ui://x1rerq2kpmrzb";

		public static FUI_ServerRoomView CreateInstance()
		{
			return (FUI_ServerRoomView)UIPackage.CreateObject("Main", "ServerRoomView");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			btnClose = (GButton)GetChildAt(1);
			serverList = (GList)GetChildAt(3);
			btnReady = (WGame.UI.Common.FUI_BtnNormal)GetChildAt(4);
			btnStart = (WGame.UI.Common.FUI_BtnNormal)GetChildAt(5);
			inputIP = (GTextInput)GetChildAt(7);
			txtMessage = (GTextField)GetChildAt(8);
			window = (GGroup)GetChildAt(9);
			modelHandler = (GGraph)GetChildAt(10);
			model = (GGraph)GetChildAt(11);
			btnArrowRight = (WGame.UI.Common.FUI_BtnArrow)GetChildAt(12);
			btnArrowLeft = (WGame.UI.Common.FUI_BtnArrow)GetChildAt(13);
			txtCharName = (GTextField)GetChildAt(14);
			character = (GGroup)GetChildAt(15);
		}
	}
}
