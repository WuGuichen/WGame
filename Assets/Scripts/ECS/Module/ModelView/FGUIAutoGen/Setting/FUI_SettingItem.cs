/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Setting
{
	public partial class FUI_SettingItem: GComponent
	{
		public GTextField title;
		public WGame.UI.Setting.FUI_SettingComboBox value;
		public const string URL = "ui://6p265q2rpmv03";

		public static FUI_SettingItem CreateInstance()
		{
			return (FUI_SettingItem)UIPackage.CreateObject("Setting", "SettingItem");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			title = (GTextField)GetChildAt(0);
			value = (WGame.UI.Setting.FUI_SettingComboBox)GetChildAt(1);
		}
	}
}
