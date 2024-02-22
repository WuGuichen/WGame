/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Setting
{
	public partial class FUI_SettingInputView: GComponent
	{
		public Controller c1;
		public GList list;
		public GTextField title;
		public GList tabList;
		public GLabel maskCommon;
		public const string URL = "ui://6p265q2rpmv07";

		public static FUI_SettingInputView CreateInstance()
		{
			return (FUI_SettingInputView)UIPackage.CreateObject("Setting", "SettingInputView");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			c1 = GetControllerAt(0);
			list = (GList)GetChildAt(1);
			title = (GTextField)GetChildAt(2);
			tabList = (GList)GetChildAt(3);
			maskCommon = (GLabel)GetChildAt(4);
		}
	}
}
