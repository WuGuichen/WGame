/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Main
{
	public partial class FUI_JoystickView: GComponent
	{
		public GGraph areaLook;
		public GGraph area;
		public GGraph panel;
		public GGraph handle;
		public GButton btnDefense;
		public GButton btnAttack;
		public GButton btnSpecial;
		public GButton btnStep;
		public GButton btnRun;
		public GButton btnFocus;
		public GGroup btns;
		public const string URL = "ui://x1rerq2kcrno4";

		public static FUI_JoystickView CreateInstance()
		{
			return (FUI_JoystickView)UIPackage.CreateObject("Main", "JoystickView");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			areaLook = (GGraph)GetChildAt(0);
			area = (GGraph)GetChildAt(1);
			panel = (GGraph)GetChildAt(2);
			handle = (GGraph)GetChildAt(3);
			btnDefense = (GButton)GetChildAt(4);
			btnAttack = (GButton)GetChildAt(5);
			btnSpecial = (GButton)GetChildAt(6);
			btnStep = (GButton)GetChildAt(7);
			btnRun = (GButton)GetChildAt(8);
			btnFocus = (GButton)GetChildAt(9);
			btns = (GGroup)GetChildAt(10);
		}
	}
}
