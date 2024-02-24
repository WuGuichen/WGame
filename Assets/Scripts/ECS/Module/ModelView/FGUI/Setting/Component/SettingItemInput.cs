using UnityEngine.InputSystem;
using WGame.Runtime;

namespace WGame.UI.Setting
{
	public partial class FUI_SettingItemInput
	{
		private int index;
		public void SetData(int idx, InputAction inputAction, int tabIndex = 0)
		{
			index = idx;
			if (index < SettingDefine.Inst.inputBtns.Length)
			{
				title.text = SettingDefine.Inst.inputBtns[index];
			}
			else
			{
				title.text = inputAction.name;
			}
			btnChange.text = inputAction.GetBindingDisplayString(tabIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);
			var action = inputAction;
			btnChange.onClick.Set(ctx =>
			{
				if (SettingModel.Inst.IsRebindingInput)
				{
					return;
				}
				SettingModel.Inst.IsRebindingInput = true;
				btnChange.text = "等待新按键...";
				SettingModel.Inst.InputAgent.RebindInputSetting(action, () =>
				{
					SetData(index ,action, tabIndex);
					SettingModel.Inst.IsRebindingInput = false;
				}, false, tabIndex);
			});
			btnReset.onClick.Set(ctx =>
			{
				SettingModel.Inst.InputAgent.ResetBindData(action, tabIndex);
				SetData(index, action, tabIndex);
				EventCenter.Trigger(EventDefine.OnRebindingInputStateChange);
			});
		}
	}
}
