using WGame.UI.Setting;using FairyGUI;
using UnityEngine.InputSystem;

namespace WGame.UI
{
	public class SettingInputView: BaseView
	{
		public override string ViewName => "SettingInputView";
		private readonly FUI_SettingInputView ui = FUI_SettingInputView.CreateInstance();
		protected override GObject uiObj => ui;

		private EventCallback0 onTabChanged;
		private InputAction[] _inputActions;

		protected override void OnRegisterEvent()
		{
			AddEvent(EventDefine.OnRebindingInputStateChange, OnRebindingStateChanged);
		}

		protected override void CustomInit()
		{
			_inputActions = SettingModel.Inst.InputAgent.InputActions[0];
			ui.title.text = "输入设置";
			onTabChanged = OnTabChanged;
			ui.c1.onChanged.Add(onTabChanged);
		}
		protected override void AfterOpen()
		{
			SettingModel.Inst.IsRebindingInput = false;
			ui.list.itemRenderer = OnItemRender;
			ui.list.numItems = _inputActions.Length;
			ui.tabList.itemRenderer = OnTabItemRender;
			ui.tabList.numItems = SettingDefine.Inst.inputType.Length;
			ui.c1.onChanged.Call();
			ui.tabList.selectedIndex = ui.tabList.selectionController.selectedIndex;
			OnRebindingStateChanged();
		}

		private void OnItemRender(int idx, GObject obj)
		{
			var item = obj as FUI_SettingItemInput;
			item.SetData(idx, _inputActions[idx]);
			// item.touchable = true;
		}

		private void OnTabItemRender(int idx, GObject obj)
		{
			var item = obj as FUI_SettingSubTabItem;
			item.title = SettingDefine.Inst.inputType[idx];
		}

		private void OnTabChanged()
		{
			SettingModel.Inst.InputAgent.CancelRebinding();
			SettingModel.Inst.IsRebindingInput = false;
		}
		protected override void BeforeClose()
		{
			CleanUpRebinding();
		}

		protected override void OnInvisible()
		{
			CleanUpRebinding();
		}

		protected override void OnDestroy()
		{
			
		}

		private void CleanUpRebinding()
		{
			SettingModel.Inst.InputAgent.CancelRebinding();
			SettingModel.Inst.IsRebindingInput = false;
		}

		private void OnRebindingStateChanged()
		{
			if (SettingModel.Inst.IsRebindingInput)
			{
				ui.maskCommon.displayObject.visible = true;
			}
			else
			{
				ui.maskCommon.displayObject.visible = false;
			}
		}
	}
}
