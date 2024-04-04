using WGame.UI.Main;using FairyGUI;
using UnityEngine;

namespace WGame.UI
{
	public class JoystickView: BaseView
	{
		public override string ViewName => "JoystickView";
		private readonly FUI_JoystickView ui = FUI_JoystickView.CreateInstance();
		protected override GObject uiObj => ui;

		private MainModel model = MainModel.Inst;

		private Vector2 touchPos;
		private Vector2 originPos;
		private float radius;
		private float radiusSqr;
		private float radiusFraction;
		private float deadRadiusSqr;

		private Vector2 lookPosBefore;
		// private float lookLerpSeed = 0.1f;

		protected override void CustomInit()
		{
			radius = ui.panel.size.x / 2;
			radiusFraction = 1 / radius;
			radiusSqr = radius * radius;
			deadRadiusSqr = (MainDefine.Inst.joystickDeadRadiusRate * radius);
			deadRadiusSqr *= deadRadiusSqr;
			originPos = ui.panel.position;
			touchPos = originPos;
			// this.childrenRenderOrder
		}
		protected override void AfterOpen()
		{
			// 确保在最底层
			GRoot.inst.SetChildIndex(this.contentPane.parent, 0);
			AddCheckLook();
			AddCheckMove();
			AddCheckButtons();
		}

		private void AddCheckLook()
		{
			ui.areaLook.onTouchBegin.Add(evt =>
			{
				if (evt.inputEvent.button != 0)
				{
					return;
				}
				evt.CaptureTouch();
				lookPosBefore = evt.inputEvent.position;
			});
			ui.areaLook.onTouchMove.Add(evt =>
			{
				var newPos = evt.inputEvent.position;
				var dir = newPos - lookPosBefore;
				lookPosBefore = newPos;
				dir.y = -dir.y;
				model.LookDir = dir * 1080/GRoot.inst.size.x * 0.5f;
			});
			ui.areaLook.onTouchEnd.Add(evt =>
			{
				MainModel.Inst.LookDir = Vector2.zero;
			});
		}

		private void AddCheckMove()
		{
			ui.area.onTouchBegin.Add(evt =>
			{
				if (evt.inputEvent.button != 0)
				{
					return;
				}

				touchPos = evt.inputEvent.position;
				touchPos = GRoot.inst.GlobalToLocal(touchPos);
				ui.panel.position = touchPos;
				ui.handle.position = touchPos;
				evt.CaptureTouch();
			});
			ui.area.onTouchMove.Add(evt =>
			{
				// dir小于等于单位向量
				var newTouchPos = GRoot.inst.GlobalToLocal(evt.inputEvent.position);
				var dir = newTouchPos - touchPos;
				var lenSqr = dir.sqrMagnitude;
				Vector2 newPos;
				if (lenSqr < deadRadiusSqr)
				{
					dir = Vector2.zero;
				}
				if (lenSqr > radiusSqr)
				{
					dir = dir.normalized;
					newPos = dir * radius;
				}
				else
				{
					newPos = dir;
					dir *= radiusFraction;
				}

				dir.y = -dir.y;
				MainModel.Inst.MoveDir = dir;
				ui.handle.position = newPos + touchPos;
			});
			ui.area.onTouchEnd.Add(evt =>
			{
				ui.handle.position = originPos;
				ui.panel.position = originPos;
				MainModel.Inst.MoveDir = Vector2.zero;
			});
		}

		private void AddCheckButtons()
		{
			ui.btnAttack.onTouchBegin.Add(() =>
			{
				model.IsTriggerAttack = true;
				model.isAttackPressing = true;
			});
			ui.btnAttack.onTouchEnd.Add(() =>
			{
				model.IsReleaseAttack = true;
				model.isAttackPressing = false;
			});
			
			ui.btnDefense.onTouchBegin.Add(() =>
			{
				model.isDefencePressing = true;
			});
			ui.btnDefense.onTouchEnd.Add(() =>
			{
				model.isDefencePressing = false;
			});
			
			ui.btnSpecial.onTouchBegin.Add(() =>
			{
				model.isSpecialPressing = true;
				model.IsTriggerSpecial = true;
			});
			// ui.btnSpecial.visible = false;
			ui.btnSpecial.onTouchEnd.Add(() =>
			{
				model.isSpecialPressing = false;
				model.IsHoldSpecial = false;
			});
			
			ui.btnStep.onTouchBegin.Add(() =>
			{
				model.IsTriggerStep = true;
			});
			
			ui.btnRun.onTouchBegin.Add(() =>
			{
				model.IsRunningState = !model.IsRunningState;
				// ui.btnRun.alpha = model.IsRunningState ? 0.6f : 1;
			});
			
			ui.btnFocus.onTouchBegin.Add(()=> model.IsTriggerFocus = true);
		}
		
		protected override void BeforeClose()
		{
						
		}
		protected override void OnDestroy()
		{
			
		}
	}
}
