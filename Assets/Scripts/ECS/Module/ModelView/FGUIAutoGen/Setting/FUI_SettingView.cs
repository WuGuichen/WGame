/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Setting
{
	public partial class FUI_SettingView: GComponent
	{
		public Controller c1;
		public GList tabList;
		public GButton btnClose;
		public const string URL = "ui://6p265q2rpmv00";

		public static FUI_SettingView CreateInstance()
		{
			return (FUI_SettingView)UIPackage.CreateObject("Setting", "SettingView");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			c1 = GetControllerAt(0);
			tabList = (GList)GetChildAt(0);
			btnClose = (GButton)GetChildAt(1);
		}
	}
}
