/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Character
{
	public partial class FUI_MessagePad: GComponent
	{
		public GTextField message;
		public const string URL = "ui://m3xq96scp15k5";

		public static FUI_MessagePad CreateInstance()
		{
			return (FUI_MessagePad)UIPackage.CreateObject("Character", "MessagePad");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			message = (GTextField)GetChildAt(1);
		}
	}
}
