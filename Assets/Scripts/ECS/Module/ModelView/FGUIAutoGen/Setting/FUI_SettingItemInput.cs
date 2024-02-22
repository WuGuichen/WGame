/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Setting
{
	public partial class FUI_SettingItemInput: GComponent
	{
		public GTextField title;
		public GButton btnChange;
		public WGame.UI.Common.FUI_BtnResetRect btnReset;
		public const string URL = "ui://6p265q2rlwycd";

		public static FUI_SettingItemInput CreateInstance()
		{
			return (FUI_SettingItemInput)UIPackage.CreateObject("Setting", "SettingItemInput");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			title = (GTextField)GetChildAt(0);
			btnChange = (GButton)GetChildAt(1);
			btnReset = (WGame.UI.Common.FUI_BtnResetRect)GetChildAt(2);
		}
	}
}
