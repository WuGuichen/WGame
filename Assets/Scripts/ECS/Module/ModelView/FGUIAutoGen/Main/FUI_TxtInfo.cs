/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Main
{
	public partial class FUI_TxtInfo: GComponent
	{
		public GGraph bg;
		public GTextField txtMain;
		public GTextField txtSub;
		public const string URL = "ui://x1rerq2kthtgd";

		public static FUI_TxtInfo CreateInstance()
		{
			return (FUI_TxtInfo)UIPackage.CreateObject("Main", "TxtInfo");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			bg = (GGraph)GetChildAt(0);
			txtMain = (GTextField)GetChildAt(1);
			txtSub = (GTextField)GetChildAt(2);
		}
	}
}
