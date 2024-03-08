/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Main
{
	public partial class FUI_GameMainView: GComponent
	{
		public GList list;
		public GTextField FPS;
		public WGame.UI.Main.FUI_ProgressBar1 hpBar;
		public WGame.UI.Main.FUI_ProgressBar1 mpBar;
		public GGraph focusPoint;
		public GButton interactTag;
		public GList topList;
		public GTextField messageBox;
		public GGraph messageBtn;
		public const string URL = "ui://x1rerq2ky2kd2";

		public static FUI_GameMainView CreateInstance()
		{
			return (FUI_GameMainView)UIPackage.CreateObject("Main", "GameMainView");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			list = (GList)GetChildAt(0);
			FPS = (GTextField)GetChildAt(1);
			hpBar = (WGame.UI.Main.FUI_ProgressBar1)GetChildAt(2);
			mpBar = (WGame.UI.Main.FUI_ProgressBar1)GetChildAt(3);
			focusPoint = (GGraph)GetChildAt(4);
			interactTag = (GButton)GetChildAt(5);
			topList = (GList)GetChildAt(6);
			messageBox = (GTextField)GetChildAt(7);
			messageBtn = (GGraph)GetChildAt(8);
		}
	}
}
