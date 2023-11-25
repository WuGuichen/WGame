using WGame.UI.Setting;using FairyGUI;
using WGame.UI.Main;
using FUI_SettingSliderItem = WGame.UI.Setting.FUI_SettingSliderItem;

namespace WGame.UI
{
	public class SettingGraphicView: BaseView
	{
		public override string ViewName => "SettingGraphicView";
		private readonly FUI_SettingGraphicView ui = FUI_SettingGraphicView.CreateInstance();
		protected override GObject uiObj => ui;

		private SettingDefine define = SettingDefine.Inst;

		private EventCallback1 onSettingValueChanged;

		protected override void CustomInit()
		{
			ui.title.text = "图像设置";
			onSettingValueChanged = OnSettingValueChanged;
		}
		protected override void AfterOpen()
		{
			ui.list.itemRenderer = OnItemRender;
			ui.list.itemProvider = ItemProvider;
			ui.list.numItems = define.grapicConfigs.GetLength(0);
		}

		private void OnItemRender(int idx, GObject obj)
		{
			var type = define.grapicConfigs[idx, 1];
			if (type == SettingDefine.ComboBox)
			{
				FUI_SettingItem item = obj as FUI_SettingItem;
				item.title.text = define.grapicConfigs[idx, 0];
				if (item.title.text == "帧率")
				{
					item.value.items = define.fpsListStr;
					for (int i = 0; i < define.fpsList.Length; i++)
					{
						if (define.fpsList[i] == SettingModel.Inst.FPS)
						{
							item.value.selectedIndex = i;
							break;
						}
					}

					item.value.onChanged.Add(onSettingValueChanged);
				}
			}
			else if (type == SettingDefine.Toggle)
			{
				FUI_SettingToggleItem item = obj as FUI_SettingToggleItem;
				item.SetData(idx, define.grapicConfigs[idx, 0], SettingModel.Inst.IsShowFPS);
				item.onClick.Add(item.OnToggleClick);
			}
			else if (type == SettingDefine.Slider)
			{
				FUI_SettingSliderItem item = obj as FUI_SettingSliderItem;
				item.SetData(idx, define.grapicConfigs[idx, 0], SettingModel.Inst);
			}
		}

		private string ItemProvider(int idx)
		{
			if (idx > define.grapicConfigs.GetLength(0) - 1)
			{
				return FUI_SettingItem.URL;
			}
			var type = define.grapicConfigs[idx, 1];
			if (type == SettingDefine.ComboBox)
			{
				return FUI_SettingItem.URL;
			}
			else if (type == SettingDefine.Toggle)
			{
				return FUI_SettingToggleItem.URL;
			}
			else if (type == SettingDefine.Slider)
			{
				return FUI_SettingSliderItem.URL;
			}
			return FUI_SettingItem.URL;
		}

		private void OnSettingValueChanged(EventContext ctx)
		{
			var comboBox = ctx.sender as FUI_SettingComboBox;
			int idx = comboBox.selectedIndex;
			SettingModel.Inst.FPS = define.fpsList[idx];
			WLogger.Info(define.fpsListStr[idx]);
		}
		protected override void BeforeClose()
		{
			
		}
		protected override void OnDestroy()
		{
			
		}
	}
}
