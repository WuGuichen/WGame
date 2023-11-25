/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Main
{
	public partial class FUI_MainListItem: GButton
	{
		public GTextField hello;
		public const string URL = "ui://x1rerq2kcy7p3";

		public static FUI_MainListItem CreateInstance()
		{
			return (FUI_MainListItem)UIPackage.CreateObject("Main", "MainListItem");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			hello = (GTextField)GetChildAt(1);
		}
	}
}
