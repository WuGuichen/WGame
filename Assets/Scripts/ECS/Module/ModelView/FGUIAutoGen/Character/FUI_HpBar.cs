/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Character
{
	public partial class FUI_HpBar: GProgressBar
	{
		public GTextField txtValue;
		public const string URL = "ui://m3xq96scq8tt4";

		public static FUI_HpBar CreateInstance()
		{
			return (FUI_HpBar)UIPackage.CreateObject("Character", "HpBar");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			txtValue = (GTextField)GetChildAt(2);
		}
	}
}
