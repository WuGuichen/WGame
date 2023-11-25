using System.Collections.Generic;
using WGame.UI.Main;using FairyGUI;

namespace WGame.UI
{
	public class CommandView: BaseView
	{
		public override string ViewName => "CommandView";
		private readonly FUI_CommandView ui = FUI_CommandView.CreateInstance();
		protected override GObject uiObj => ui;

		private EventCallback1 onPresetItemClick;

		private int funcIdx;
		private int funcParamNum;

		protected override void CustomInit()
		{
			ui.btnClose.onClick.Add(CloseView);
			onPresetItemClick = OnPresetItemClick;
		}
		protected override void AfterOpen()
		{
			ui.list.itemRenderer = OnItemRender;
			ui.list.onClickItem.Add(OnCommandItemClick);
			ui.presetList.itemRenderer = OnPresetItemRender;
			ui.presetList.numItems = MainDefine.Inst.presetCommands.Length;
			ui.presetList.onClickItem.Add(onPresetItemClick);
			
			ui.btnSure.onClick.Add(() =>
			{
				if (funcIdx < 0)
					return;
				var t1 = ui.input1.text;
				var t2 = ui.input2.text;
			});
		}

		private void OnCommandItemClick(EventContext ctx)
		{
			// var list = ctx.sender as GList;
			// var idx = list.selectedIndex;
			// funcIdx = idx;
			// var funcName = ActionHelper.ActionListName[idx];
			// ui.txtSelect.text = funcName;
			// var p1 = ActionHelper.ActionListParam1[idx];
			// var p2 = ActionHelper.ActionListParam2[idx];
			// funcParamNum = 0;
			// if (p1 != null)
			// 	funcParamNum++;
			// if (p2 != null)
			// 	funcParamNum++;
			// ui.txtTip1.text = (p1 != null) ? p1.Split(".")[^1] : "空";
			// ui.txtTip2.text = (p2 != null) ? p2.Split(".")[^1] : "空";
		}
		
		private void OnPresetItemRender(int idx, GObject obj)
		{
			FUI_MainListItem item = obj as FUI_MainListItem;
			item.hello.text = MainDefine.Inst.presetCommands[idx];
			item.index = idx;
		}

		private void OnPresetItemClick(EventContext ctx)
		{
			var item = ctx.data as FUI_MainListItem;
			MainModel.Inst.OnPresetCommandClick(item.index);
		}

		private void OnItemRender(int index, GObject obj)
		{
			// obj.text = ActionHelper.ActionListName[index] 
			//                  + "(" + ActionHelper.ActionListParam1[index]+", " 
			//                  + ActionHelper.ActionListParam2[index] + ");";
		}
		protected override void BeforeClose()
		{
			
		}
		protected override void OnDestroy()
		{
			
		}
	}
}
