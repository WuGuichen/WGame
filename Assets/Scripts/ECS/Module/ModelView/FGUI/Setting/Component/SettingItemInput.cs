using UnityEngine.InputSystem;

namespace WGame.UI.Setting
{
	public partial class FUI_SettingItemInput
	{
		private int index;
		public void SetData(int idx, InputAction inputAction)
		{
			index = idx;
			title.text = inputAction.name;
			btnChange.text = inputAction.GetBindingDisplayString(0, InputBinding.DisplayStringOptions.DontIncludeInteractions);
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
					SetData(index ,action);
					SettingModel.Inst.IsRebindingInput = false;
				});
			});
			btnReset.onClick.Set(ctx =>
			{
				SettingModel.Inst.InputAgent.ResetBindData(action);
				SetData(index, action);
			});
		}
	}
}
