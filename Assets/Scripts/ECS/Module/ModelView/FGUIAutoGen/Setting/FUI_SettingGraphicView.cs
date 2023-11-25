/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Setting
{
	public partial class FUI_SettingGraphicView: GComponent
	{
		public GList list;
		public GTextField title;
		public const string URL = "ui://6p265q2rpmv09";

		public static FUI_SettingGraphicView CreateInstance()
		{
			return (FUI_SettingGraphicView)UIPackage.CreateObject("Setting", "SettingGraphicView");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			list = (GList)GetChildAt(1);
			title = (GTextField)GetChildAt(2);
		}
	}
}
