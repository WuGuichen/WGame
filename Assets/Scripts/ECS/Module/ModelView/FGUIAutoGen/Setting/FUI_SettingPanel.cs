/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Setting
{
	public partial class FUI_SettingPanel: GComponent
	{
		public GList list;
		public GTextField title;
		public const string URL = "ui://6p265q2rpmv02";

		public static FUI_SettingPanel CreateInstance()
		{
			return (FUI_SettingPanel)UIPackage.CreateObject("Setting", "SettingPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			list = (GList)GetChildAt(1);
			title = (GTextField)GetChildAt(2);
		}
	}
}
