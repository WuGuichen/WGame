/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Setting
{
	public partial class FUI_SettingComboBox_popup: GComponent
	{
		public GList list;
		public const string URL = "ui://6p265q2rpmv05";

		public static FUI_SettingComboBox_popup CreateInstance()
		{
			return (FUI_SettingComboBox_popup)UIPackage.CreateObject("Setting", "SettingComboBox_popup");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			list = (GList)GetChildAt(1);
		}
	}
}
