using System;
using System.Collections.Generic;
using System.Text;
using WGame.UI.Main;using FairyGUI;
using UnityEngine;
using WGame.Ability;
using WGame.Attribute;

namespace WGame.UI
{
	public class GameMainView: BaseView
	{
		public override string ViewName => "GameMainView";
		private readonly FUI_GameMainView ui = FUI_GameMainView.CreateInstance();
		protected override GObject uiObj => ui;

		private EventCallback1 callback0;
		private MainModel model;

		private BuffManager buffManager;
		private StringBuilder _builder = new();

		private class BuffInfo
		{
			public FUI_TxtInfo UIInfo { get; set; }
			public LinkedList<BuffStatus> Buff { get; set; } = new();
			public float LifePercentTime => Buff.First.Value.LifePercentTime;
		}
		
		private Dictionary<string, BuffInfo> _buffDict = new();

		protected override void CustomInit()
		{
			MainModel.Inst.InitMainBtnDict();
			ui.list.itemRenderer = RenderItemList;
			ui.topList.itemRenderer = RenderTopItemList;
			
			ui.txtVersion.text = "点击Alt切换鼠标";
			callback0 = OnItemClick;
			model = MainModel.Inst;
			ui.interactTag.visible = false;
			ui.topList.visible = false;
			ui.weakPoint.visible = false;
			ui.messageBox.text = WTerminal.LastMessage;
			ui.messageBtn.onClick.Add(() =>
			{
				UIManager.OpenView(VDB.TerminalView);
			});
			OnTerminalUpdate();
		}

		// ReSharper disable Unity.PerformanceAnalysis
		protected override void OnRegisterEvent()
		{
			AddEvent(EventDefine.OnFPSUpdate, RefreshFPS);
			AddEvent(EventDefine.OnControlCharacterChanged, RefreshCharacterInfo);
			AddEvent(EventDefine.OnFocusPointUpdate, RefreshFocusPoint);
			AddEvent(EventDefine.OnWeakPointUpdate, OnWeakPointUpdate);
			AddEvent(EventDefine.OnInteractTagRefresh, RefreshInteractTag);
			AddEvent(EventDefine.OnGameStart, OnGameStart);
			AddEvent(EventDefine.OnBackToMainView, OnBackToMainView);
			AddEvent(EventDefine.OnTerminalMessageUpdate, OnTerminalUpdate);
		}

		private void OnTerminalUpdate()
		{
			if (SettingModel.Inst.IsShowMessage)
			{
				ui.messageBox.text = WTerminal.LastMessage;
				ui.messageBtn.touchable = true;
			}
			else
			{
				ui.messageBox.text = String.Empty;
				ui.messageBtn.touchable = false;
			}
		}
		
		private void OnBackToMainView()
		{
			model.SetMainViewBtnState(MainDefine.Btn_StartOffline, true);
			model.SetMainViewBtnState(MainDefine.Btn_StartOffline, "离线模式开始");
			ui.topList.visible = false;
		}

		void OnGameStart()
		{
			ui.topList.visible = true;
		}

		protected override void AfterOpen()
		{
			ui.list.numItems = MainDefine.AllMainBtnList.Count;
			ui.list.onClickItem.Add(callback0);
			ui.interactTag.onClick.Add(model.OnInteractTagClick);
			ui.topList.numItems = MainDefine.Inst.mainTopBtnListNames.Length;
			ui.topList.onClickItem.Add(OnTopItemClick);
			RefreshFPS();
			RefreshFocusPoint();
			WGame.Runtime.YooassetManager.Inst.LoadTextAssetSync("item_tbitem");
			TickManager.Inst.AddTick(OnUpdate);
		}

		private void OnUpdate()
		{
			if (model.IsShowTag)
			{
				ui.interactTag.position = GRoot.inst.WorldToLocal(model.curInteractTag.TagPos);
			}
		}
		
		private void RefreshFocusPoint()
		{
			ui.focusPoint.visible = model.IsFocus;
			var pos = model.FocusPosition;
			pos = GRoot.inst.WorldToLocal(pos);
			ui.focusPoint.position = pos;
		}
		
		private void OnWeakPointUpdate()
		{
			ui.weakPoint.visible = model.HasWeakPoint;
			var pos = model.WeakPosition;
			pos = GRoot.inst.WorldToLocal(pos);
			ui.weakPoint.position = pos;
		}

		private void RefreshFPS()
		{
			if (MainModel.Inst.frames > 0)
				ui.FPS.text = MainModel.Inst.frames.ToString("f1");
			else
				ui.FPS.text = "";
		}

		void RenderItemList(int idx, GObject obj)
		{
			FUI_MainListItem item = obj as FUI_MainListItem;
			item.SetData(idx);
		}

		void RenderTopItemList(int idx, GObject obj)
		{
			var item = obj as FUI_MainListItem;
			item.hello.text = MainDefine.Inst.mainTopBtnListNames[idx];
			item.index = idx;
		}
		
		// void RenderBuffItemList(int idx, GObject obj)
		// {
		// 	var item = obj as FUI_TxtInfo;
		// }

		void OnItemClick(EventContext ctx)
		{
			var c = ctx.data as FUI_MainListItem;
			MainModel.Inst.OnClickMainBtn(c.Info);
		}
		
		void OnTopItemClick(EventContext ctx)
		{
			var c = ctx.data as FUI_MainListItem;
			MainModel.Inst.OnGameMainViewTopItemClick(c.index);
		}
		
		protected override void BeforeClose()
		{
			TickManager.Inst.RemoveTick(OnUpdate);
		}
		protected override void OnDestroy()
		{
		}

		private void OnHPChanged(WaEventContext context)
		{
			if (context.attrID == WAttrType.CurHP)
			{
				ui.hpBar.value = context.value;
			}
			else
			{
				ui.hpBar.max = context.value;
			}
		}

		private void OnMPChanged(WaEventContext context)
		{
			if (context.attrID == WAttrType.CurMP)
			{
				ui.mpBar.value = context.value;
			}
			else
			{
				ui.mpBar.max = context.value;
			}
		}

		private void RefreshCharacterInfo()
		{
			var entity = EntityUtils.GetGameEntity(CharacterModel.Inst.beforeControlledCharacterID);
			if (entity != null && entity.hasAttribute)
			{
				var attribute = entity.attribute.value;
				attribute.CancelEvent(WAttrType.CurHP, OnHPChanged);
				attribute.CancelEvent(WAttrType.MaxHP, OnHPChanged);
				attribute.CancelEvent(WAttrType.MaxMP, OnMPChanged);
				attribute.CancelEvent(WAttrType.CurMP, OnMPChanged);
				if (buffManager != null)
				{
					buffManager.onBuffUpdate -= OnBuffUpdate;
					buffManager.onBuffAdded -= OnBuffAdded;
					buffManager.onBuffRemoved -= OnBuffRemoved;
				}

				ClearBuffDict();
			}
			entity = EntityUtils.GetGameEntity(CharacterModel.Inst.currentControlledCharacterID);
			if (entity != null && entity.hasAttribute)
			{
				var attribute = entity.attribute.value;
				attribute.RegisterEvent(WAttrType.CurHP, OnHPChanged);
				attribute.RegisterEvent(WAttrType.MaxHP, OnHPChanged);
				attribute.RegisterEvent(WAttrType.MaxMP, OnMPChanged);
				attribute.RegisterEvent(WAttrType.CurMP, OnMPChanged);

				ui.hpBar.visible = true;
				ui.mpBar.visible = true;
				ui.hpBar.max = entity.attribute.value.Get(WAttrType.MaxHP, true);
				ui.hpBar.value = entity.attribute.value.Get(WAttrType.CurHP, true);
				ui.mpBar.max = entity.attribute.value.Get(WAttrType.MaxMP, true);
				ui.mpBar.value = entity.attribute.value.Get(WAttrType.CurMP, true);

				buffManager = entity.linkAbility.Ability.abilityService.service.BuffManager;
				buffManager.onBuffUpdate += OnBuffUpdate;
				buffManager.onBuffAdded += OnBuffAdded;
				buffManager.onBuffRemoved += OnBuffRemoved;
				
				var _buffList = buffManager.BuffList;
				RefreshBuffDict(_buffList);
			}
			else
			{
				ui.hpBar.visible = false;
				ui.mpBar.visible = false;
				ClearBuffDict();
				buffManager.onBuffUpdate -= OnBuffUpdate;
				buffManager.onBuffAdded -= OnBuffAdded;
				buffManager.onBuffRemoved -= OnBuffRemoved;
				buffManager = null;
			}
		}

		private void OnBuffUpdate(BuffStatus buff)
		{
			if (buff.EntityID == buffManager.Owner.EntityID)
			{
				var info = _buffDict[buff.Name];
				if (info.Buff.First.Value == buff)
				{
					info.UIInfo.bg.scaleX = 1 - info.LifePercentTime;
				}
			}
		}
		
		private void OnBuffAdded(BuffStatus buff)
		{
			if (_buffDict.TryGetValue(buff.Name, out var info))
			{
				info.Buff.AddLast(buff);
			}
			else
			{
				info = new BuffInfo()
				{
					UIInfo = ui.buffList.AddItemFromPool() as FUI_TxtInfo
				};
				info.Buff.AddLast(buff);
				_buffDict.Add(buff.Name, info);
				info.UIInfo.txtMain.text = buff.Name;
			}

			RefreshBuffNum(ref info);
		}

		private void RefreshBuffNum(ref BuffInfo info)
		{
			_builder.Append("[");
			_builder.Append(info.Buff.Count);
			_builder.Append("]");
			info.UIInfo.txtSub.text = _builder.ToString();
			_builder.Clear();
		}

		private void ClearBuffDict()
		{
			using (var itr = _buffDict.GetEnumerator())
			{
				while (itr.MoveNext())
				{
					ui.buffList.RemoveChildToPool(itr.Current.Value.UIInfo);
				}
			}
			_buffDict.Clear();
		}
		private void RefreshBuffDict(LinkedList<BuffStatus> buffList)
		{
			using (var itr = buffList.GetEnumerator())
			{
				while (itr.MoveNext())
				{
					var status = itr.Current;
					if (!_buffDict.TryGetValue(status.Name, out var info))
					{
						info = new BuffInfo()
						{
							UIInfo = ui.buffList.AddItemFromPool() as FUI_TxtInfo
						};
						_buffDict.Add(status.Name, info);
					}
					info.Buff.AddLast(itr.Current);
					RefreshBuffNum(ref info);
				}
			}
		}
		
		private void OnBuffRemoved(BuffStatus buff)
		{
			if (_buffDict.TryGetValue(buff.Name, out var info))
			{
				info.Buff.Remove(buff);
				if (info.Buff.Count <= 0)
				{
					ui.buffList.RemoveChildToPool(info.UIInfo);	
					_buffDict.Remove(buff.Name);
				}
				RefreshBuffNum(ref info);
			}
			else
			{
				WLogger.Error("有问题");
			}
		}

		private void RefreshInteractTag()
		{
			if (model.IsShowTag)
			{
				ui.interactTag.visible = true;
			}
			else
			{
				ui.interactTag.visible = false;
				ui.interactTag.position = GRoot.inst.GlobalToLocal(new Vector2(0, 999999));
			}
		}
	}
}
