using WGame.UI.Setting;using FairyGUI;

namespace WGame.UI
{
	public class SettingInputView: BaseView
	{
		public override string ViewName => "SettingInputView";
		private readonly FUI_SettingInputView ui = FUI_SettingInputView.CreateInstance();
		protected override GObject uiObj => ui;

		private EventCallback0 onTabChanged;

		protected override void CustomInit()
		{
			ui.title.text = "输入设置";
			onTabChanged = OnTabChanged;
			ui.c1.onChanged.Add(onTabChanged);
		}
		protected override void AfterOpen()
		{
			ui.list.itemRenderer = OnItemRender;
			ui.list.numItems = SettingDefine.Inst.inputBtns.Length;
			ui.tabList.itemRenderer = OnTabItemRender;
			ui.tabList.numItems = SettingDefine.Inst.inputType.Length;
			ui.c1.onChanged.Call();
			ui.tabList.selectedIndex = ui.tabList.selectionController.selectedIndex;
		}

		private void OnItemRender(int idx, GObject obj)
		{
			var item = obj as FUI_SettingItem;
			item.title.text = SettingDefine.Inst.inputBtns[idx];
			// item.touchable = true;
		}

		private void OnTabItemRender(int idx, GObject obj)
		{
			var item = obj as FUI_SettingSubTabItem;
			item.title = SettingDefine.Inst.inputType[idx];
		}

		private void OnTabChanged()
		{
		}
		protected override void BeforeClose()
		{
			
		}
		protected override void OnDestroy()
		{
			
		}
	}
}
