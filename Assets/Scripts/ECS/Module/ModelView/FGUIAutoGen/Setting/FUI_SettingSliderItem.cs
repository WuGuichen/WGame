/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Setting
{
	public partial class FUI_SettingSliderItem: GComponent
	{
		public GTextField title;
		public WGame.UI.Common.FUI_Slider1 slider;
		public const string URL = "ui://6p265q2rny0wb";

		public static FUI_SettingSliderItem CreateInstance()
		{
			return (FUI_SettingSliderItem)UIPackage.CreateObject("Setting", "SettingSliderItem");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			title = (GTextField)GetChildAt(0);
			slider = (WGame.UI.Common.FUI_Slider1)GetChildAt(1);
		}
	}
}
