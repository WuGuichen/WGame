/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Character
{
	public partial class FUI_CharacterInfoView: GComponent
	{
		public GButton btnClose;
		public GList attrList;
		public const string URL = "ui://m3xq96scki6w0";

		public static FUI_CharacterInfoView CreateInstance()
		{
			return (FUI_CharacterInfoView)UIPackage.CreateObject("Character", "CharacterInfoView");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			btnClose = (GButton)GetChildAt(0);
			attrList = (GList)GetChildAt(1);
		}
	}
}
