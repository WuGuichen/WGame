/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Character
{
	public partial class FUI_HeadPad: GComponent
	{
		public Transition show;
		public WGame.UI.Character.FUI_HpBar hpBar;
		public const string URL = "ui://m3xq96sc96px2";

		public static FUI_HeadPad CreateInstance()
		{
			return (FUI_HeadPad)UIPackage.CreateObject("Character", "HeadPad");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			show = GetTransitionAt(0);
			hpBar = (WGame.UI.Character.FUI_HpBar)GetChildAt(0);
		}
	}
}
