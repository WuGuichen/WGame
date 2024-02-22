using System.Collections.Generic;
using DG.Tweening;
using FairyGUI;
using UnityEngine;
using System;
using Sirenix.Utilities;
using UnityTimer;
using WGame.Runtime;

namespace WGame.UI
{
	public abstract class BaseView : Window
	{
		public abstract string ViewName { get; }

		protected abstract GObject uiObj { get; }
		protected string[] hideViewList;
		protected List<string> hiddedViewList= new List<string>();

		public BaseView parentView;

		/// <summary>
		/// 展示窗口动画
		/// </summary>
		protected bool IsNeedShowAnim = false;

		/// <summary>
		/// 隐藏窗口动画
		/// </summary>
		protected bool IsNeedHideAnim = false;

		private Dictionary<int, WEventCallback0> eventList = new();
		private Dictionary<int, WEventCallback1> eventList1 = new();

		private List<Timer> timerList = new();
		private Dictionary<int,Timer> timerDict = new();

		private List<string> subViewList = new List<string>();

		public bool IsVisible
		{
			get { return visible; }
			set
			{
				if (visible != value)
				{
					visible = value;
					if (value)
					{
						OnVisible();
					}
					else
					{
						OnInvisible();
					}
				}
			}
		}

		protected override void OnInit()
		{
			contentPane = uiObj.asCom;
			container.cachedTransform.position = Vector3.zero;
			container.cachedTransform.localScale = Vector3.one;
			contentPane.SetSize(GRoot.inst.size.x, GRoot.inst.size.y);
			pivot = new Vector2(0.5f, 0.5f);
			base.OnInit();
			CustomInit();
		}

		protected virtual void CustomInit()
		{
			
		}

		protected virtual void OnInvisible()
		{
			
		}
		protected virtual void OnVisible()
		{
			
		}
		
		protected override void OnShown()
		{
			HideNeedHideView();
			OnRegisterEvent();
			AfterOpen();
		}

		protected virtual void OnRegisterEvent()
		{
			
		}

		private void HideNeedHideView()
		{
			hiddedViewList.Clear();
			if (!hideViewList.IsNullOrEmpty())
			{
				foreach (var view in hideViewList)
				{
					if(UIManager.HideView(view))
						hiddedViewList.Add(view);
				}
			}
		}

		protected virtual void AfterOpen()
		{
			
		}

		/// <summary>
        /// 显示页面动画,可重写
        /// </summary>
        protected override void DoShowAnimation()
        {
            if (IsNeedShowAnim)
            {
                if (!string.IsNullOrEmpty(UIConfig.globalModalWaiting))
                    GRoot.inst.ShowModalWait();
                scale = new Vector2(0.6f, 0.6f);
                DOTween.To(() => scale, a => scale = a, Vector2.one, 0.3f)
                    .SetEase(Ease.OutBounce).OnComplete(() =>
                    {
                        if (!string.IsNullOrEmpty(UIConfig.globalModalWaiting))
                        {
                            GRoot.inst.CloseModalWait();
                        }
                        OnShown();
                    })
                    .SetUpdate(true)
                    .SetTarget(this);
            }
            else
            {
                scale = Vector2.one;
                OnShown();
            }
        }
        
		/// <summary>
		/// 隐藏页面动画，可重写
		/// </summary>
		protected override void DoHideAnimation()
		{
			if (IsNeedHideAnim)
			{
				DOTween.To(() => scale, a => scale = a, Vector2.zero, 0.3f)
					.OnComplete(() => { base.DoHideAnimation(); });
			}
			else
			{
				HideImmediately();
			}
		}

		public override void Dispose()
		{
			OnDestroy();
			base.Dispose();
		}

		protected override void OnHide()
		{
			ReShowHideViews();
			RemoveAllEvent();
			RemoveAllTimer();
			CloseAllSubView();
			BeforeClose();
		}

		private void ReShowHideViews()
		{
			if (!hiddedViewList.IsNullOrEmpty())
			{
				foreach (var view in hiddedViewList)
				{
					UIManager.ShowView(view);	
				}
			}
		}

		private void CloseAllSubView()
		{
			if (parentView != null)
				parentView.RemoveSubView(this.ViewName);
			var tmp = new string[subViewList.Count];
			for (int i = 0; i < subViewList.Count; i++)
			{
				tmp[i] = subViewList[i];
			}

			for (int i = 0; i < tmp.Length; i++)
			{
				UIManager.CloseView(tmp[i]);
			}
			subViewList.Clear();
		}

		protected virtual void BeforeClose()
		{
			
		}

		protected virtual void OnDestroy()
		{
			
		}

		protected void AddEvent(int type, WEventCallback0 callback)
		{
			EventCenter.AddListener(type, callback);
			if (eventList.TryAdd(type, callback) == false)
			{
				Debug.LogWarning("单个界面仅支持添加一个同种类型事件");
			}
		}
		protected void AddEvent(int type, WEventCallback1 callback)
		{
			EventCenter.AddListener(type, callback);				
			if (eventList1.TryAdd(type, callback) == false)
			{
				Debug.LogWarning("单个界面仅支持添加一个同种类型事件");
			}
		}

		private void RemoveAllEvent()
		{
			foreach (var kv in eventList1)
			{
				EventCenter.RemoveListener(kv.Key, kv.Value);
			}
			foreach (var kv in eventList)
			{
				EventCenter.RemoveListener(kv.Key, kv.Value);
			}
			eventList.Clear();
			eventList1.Clear();
		}

		// 可同时存在多个的计时
		protected void AddTimer(float time, Action callback, Action<float> onUpdate = null, bool isLoop = false)
		{
			var timer = Timer.Register(time, callback, onUpdate, isLoop);
			timerList.Add(timer);
		}
		
		// 只有一个的计时
		protected void AddTimer(int id, float time, Action callback, Action<float> onUpdate = null, bool isLoop = false)
		{
			if (timerDict.TryGetValue(id, out var timer))
			{
				timer.Cancel();
			}
			timer = Timer.Register(time, callback, onUpdate, isLoop);
			timerDict.Add(id, timer);
		}

		private void RemoveAllTimer()
		{
			timerList.ForEach(timer => timer.Cancel());
			timerList.Clear();
			foreach (var kv in timerDict)
			{
				kv.Value.Cancel();	
			}
			timerDict.Clear();
		}

		protected void CloseView()
		{
			UIManager.CloseView(ViewName);
		}

		// ReSharper disable Unity.PerformanceAnalysis
		public void OpenSubView(string viewName)
		{
			UIManager.OpenView(viewName, this);
			UIManager.ShowView(viewName);
			var view = UIManager.GetView(viewName);
			if (view == null)
				return;
			if(!subViewList.Contains(viewName))
				subViewList.Add(viewName);
			for (int i = 0; i < subViewList.Count; i++)
			{
				if (subViewList[i] != viewName)
				{
					UIManager.HideView(subViewList[i]);
				}
			}
		}

		public void RemoveSubView(string viewName)
		{
			if(subViewList.Contains(viewName))
				subViewList.Remove(viewName);
		}
	}
}