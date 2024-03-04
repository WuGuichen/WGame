using FairyGUI;
using WGame.Runtime;

namespace WGame.UI.Setting
{
	public partial class FUI_SettingToggleItem
	{
		public int index;
		private EventCallback0 onToggleClick;
		private string data;

		public void SetData(int idx, string setName, bool isToggled)
		{
			onToggleClick = OnToggleClick;
			index = idx;
			title.text = setName;
			data = setName;
			toggle.onClick.Add(onToggleClick);
			toggle.selected = isToggled;
		}

		public void OnToggleClick()
		{
			toggle.selected = !toggle.selected;
			if (index == 1)
			{
				SettingModel.Inst.IsShowFPS = toggle.selected;
			}
			else if (index == 2)
			{
				SettingModel.Inst.IsShowMessage = toggle.selected;
				EventCenter.Trigger(EventDefine.OnTerminalMessageUpdate);
			}
		}
	}
}
