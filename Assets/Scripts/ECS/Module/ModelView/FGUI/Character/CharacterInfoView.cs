using WGame.UI.Character;using FairyGUI;

namespace WGame.UI
{
	public class CharacterInfoView: BaseView
	{
		public override string ViewName => "CharacterInfoView";
		private readonly FUI_CharacterInfoView ui = FUI_CharacterInfoView.CreateInstance();
		protected override GObject uiObj => ui;

		private ListItemRenderer itemRenderer;

		protected override void CustomInit()
		{
			ui.attrList.itemRenderer = OnItemRender;
		}
		protected override void AfterOpen()
		{
			
		}

		private void OnItemRender(int index, GObject obj)
		{
			FUI_AttributeInfoItem item = obj as FUI_AttributeInfoItem;
		}
		
		protected override void BeforeClose()
		{
			
		}
		protected override void OnDestroy()
		{
			
		}
	}
}
