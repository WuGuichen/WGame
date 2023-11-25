/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Character
{
	public partial class FUI_AttributeInfoItem: GComponent
	{
		public GTextField name;
		public GTextField value;
		public const string URL = "ui://m3xq96scki6w1";

		public static FUI_AttributeInfoItem CreateInstance()
		{
			return (FUI_AttributeInfoItem)UIPackage.CreateObject("Character", "AttributeInfoItem");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			name = (GTextField)GetChildAt(1);
			value = (GTextField)GetChildAt(2);
		}
	}
}
