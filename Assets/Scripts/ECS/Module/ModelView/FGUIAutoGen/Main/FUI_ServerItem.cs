/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Main
{
	public partial class FUI_ServerItem: GComponent
	{
		public GTextField txtName;
		public GTextField txtState;
		public const string URL = "ui://x1rerq2kpmrzc";

		public static FUI_ServerItem CreateInstance()
		{
			return (FUI_ServerItem)UIPackage.CreateObject("Main", "ServerItem");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			txtName = (GTextField)GetChildAt(1);
			txtState = (GTextField)GetChildAt(2);
		}
	}
}
