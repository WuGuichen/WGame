/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Common
{
	public partial class FUI_Slider1: GSlider
	{
		public GTextField title;
		public const string URL = "ui://4bm98jsmm876e";

		public static FUI_Slider1 CreateInstance()
		{
			return (FUI_Slider1)UIPackage.CreateObject("Common", "Slider1");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			title = (GTextField)GetChildAt(3);
		}
	}
}
