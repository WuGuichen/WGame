/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace WGame.UI.Main
{
	public partial class FUI_CommandView: GComponent
	{
		public GButton btnClose;
		public GList list;
		public GTextField desc;
		public GList presetList;
		public GTextInput input1;
		public WGame.UI.Common.FUI_BtnNormal btnSure;
		public GTextInput input2;
		public GTextField txtSelect;
		public GTextField txtTip1;
		public GTextField txtTip2;
		public const string URL = "ui://x1rerq2kd97j5";

		public static FUI_CommandView CreateInstance()
		{
			return (FUI_CommandView)UIPackage.CreateObject("Main", "CommandView");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			btnClose = (GButton)GetChildAt(0);
			list = (GList)GetChildAt(1);
			desc = (GTextField)GetChildAt(2);
			presetList = (GList)GetChildAt(3);
			input1 = (GTextInput)GetChildAt(5);
			btnSure = (WGame.UI.Common.FUI_BtnNormal)GetChildAt(6);
			input2 = (GTextInput)GetChildAt(8);
			txtSelect = (GTextField)GetChildAt(9);
			txtTip1 = (GTextField)GetChildAt(10);
			txtTip2 = (GTextField)GetChildAt(11);
		}
	}
}
