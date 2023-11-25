/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Setting
{
	public partial class FUI_SettingTabItem: GButton
	{
		public GGraph shape;
		public const string URL = "ui://6p265q2rpmv01";

		public static FUI_SettingTabItem CreateInstance()
		{
			return (FUI_SettingTabItem)UIPackage.CreateObject("Setting", "SettingTabItem");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			shape = (GGraph)GetChildAt(0);
		}
	}
}
