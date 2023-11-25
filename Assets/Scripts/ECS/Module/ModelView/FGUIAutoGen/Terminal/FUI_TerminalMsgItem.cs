/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Terminal
{
	public partial class FUI_TerminalMsgItem: GComponent
	{
		public GTextField title;
		public const string URL = "ui://xfar8tnuq56t1";

		public static FUI_TerminalMsgItem CreateInstance()
		{
			return (FUI_TerminalMsgItem)UIPackage.CreateObject("Terminal", "TerminalMsgItem");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			title = (GTextField)GetChildAt(0);
		}
	}
}
