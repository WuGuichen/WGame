/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Setting
{
	public partial class FUI_SettingSubTabItem: GButton
	{
		public GGraph shape;
		public const string URL = "ui://6p265q2rj3xda";

		public static FUI_SettingSubTabItem CreateInstance()
		{
			return (FUI_SettingSubTabItem)UIPackage.CreateObject("Setting", "SettingSubTabItem");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			shape = (GGraph)GetChildAt(0);
		}
	}
}
