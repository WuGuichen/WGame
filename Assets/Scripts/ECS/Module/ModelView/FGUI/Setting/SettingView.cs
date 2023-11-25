using WGame.UI.Setting;using FairyGUI;

namespace WGame.UI
{
	public class SettingView: BaseView
	{
		public override string ViewName => "SettingView";
		private readonly FUI_SettingView ui = FUI_SettingView.CreateInstance();
		protected override GObject uiObj => ui;

		private EventCallback0 onTabItemClick;

		protected override void CustomInit()
		{
			ui.btnClose.onClick.Add(CloseView);
			hideViewList = new string[] { VDB.GameMainView, VDB.JoystickView };
			onTabItemClick = OnRefreshTabSelect;
			ui.c1.onChanged.Add(onTabItemClick);
		}

		protected override void AfterOpen()
		{
			ui.tabList.itemRenderer = OnTabItemRender;
			ui.tabList.numItems = SettingDefine.Inst.settingTabs.Length;
			ui.c1.onChanged.Call();
			ui.tabList.selectedIndex = ui.tabList.selectionController.selectedIndex;
		}

		private void OnTabItemRender(int idx, GObject obj)
		{
			var item = obj as FUI_SettingTabItem;
			item.title = SettingDefine.Inst.settingTabs[idx];
		}

		private void OnRefreshTabSelect()
		{
			int idx = ui.c1.selectedIndex;
			SettingModel.Inst.OnSettingTabItemClick(this, idx);
		}

		private void OnPanelItemRender(int idx, GObject obj)
		{
			var item = obj as FUI_SettingPanel;
		}
		protected override void BeforeClose()
		{
		}
		protected override void OnDestroy()
		{
			
		}
	}
}
