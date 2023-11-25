/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Setting
{
	public partial class FUI_SettingToggleItem: GComponent
	{
		public GTextField title;
		public WGame.UI.Common.FUI_Toggle1 toggle;
		public const string URL = "ui://6p265q2rm876c";

		public static FUI_SettingToggleItem CreateInstance()
		{
			return (FUI_SettingToggleItem)UIPackage.CreateObject("Setting", "SettingToggleItem");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			title = (GTextField)GetChildAt(0);
			toggle = (WGame.UI.Common.FUI_Toggle1)GetChildAt(1);
		}
	}
}
