using System;
using UnityEngine;

namespace Motion
{
    class TriggerItem : TimeAreaItem
    {
        protected override string ToolTip
        {
            get
            {
                for (int i = 0; i < AnimTriggerType.Count; i++)
                {
                    if ((AnimTriggerType.Triggers[i] & triggerType) > 0)
                    {
                        return AnimTriggerType.TriggerNames[i];
                    }
                }

                return "未定义";
            }
        }

        public bool isShowTip = false;

        private readonly Action<TriggerItem> _onRightClick;
        private readonly Action<TriggerItem, double> _onDragTrigger;
        private readonly Action _onClickUp;
        private readonly Action _onMouseOverlay;

        // 毫秒(千分比)
        public int triggerTime;
        // 百分比
        public int triggerParam;
        private int _triggerType;

        public int triggerType
        {
            get => _triggerType;
            set
            {
                _triggerType = value;
                // var color = _triggerType switch
                // {
                //     AnimTriggerType.RootMotion => Color.yellow,
                //     _ => Color.white
                // };
                Color color = Color.white;
                for (int i = 0; i < 32; i++)
                {
                    if (((1 << i) & _triggerType) > 0)
                    {
                        color = i switch
                        {
                            < 3 => new Color(1, 0.3f * (i+1), 0),
                            < 6 => new Color(1 - 0.3f * (i - 2), 1, 0),
                            < 8 => new Color(0, 0.5f - 0.25f*(i-5), 1),
                            < 12 => new Color(0.25f*(i-7), 0, 1f),
                            < 16 => new Color(1f, 0.25f*(i-11), 1f),
                            _ => Color.white
                        };
                    }
                }

                headColor = color;
                lineColor = color;
            }
        }

        public TriggerItem(GUIStyle style, WindowState state,Action<double> onDrag, Action onMouseOverlay, Action onClickUp,Action<TriggerItem, double> onDragTrigger, bool alwaysShowTooltip = false, Action<TriggerItem> onRightClick = null, float triggerTime = 0f) : base(style, state, onDrag, alwaysShowTooltip)
        {
            _onRightClick = onRightClick;
            _onDragTrigger = onDragTrigger;
            _onClickUp = onClickUp;
            _onClickUp += OnClickUp;
            _onMouseOverlay = OnMouseOverLay;
            _onMouseOverlay += onMouseOverlay;
            this.triggerTime = (int)(triggerTime*1000);
        }

        private void OnClickUp()
        {
            isShowTip = false;
        }

        private void OnMouseOverLay()
        {
            isShowTip = true;
        }
        
        protected override void InitItem(Action<double> onDrag)
        {
            var trigger = new EventTriggerManipulator(
                (evt, st) =>
                {
                    // var res = st.window.TimeContentWithOffset.Contains(evt.mousePosition) &&
                    //           Bounds.Contains(evt.mousePosition);
                    // if (res)
                        
                    firstDrag = true;
                    return st.window.TimeContentWithOffset.Contains(evt.mousePosition) &&
                           Bounds.Contains(evt.mousePosition);
                },
                (d) =>
                {
                    if (onDrag != null)
                    {
                        onDrag(d);
                    }

                    _onDragTrigger?.Invoke(this, d);
                },
                () =>
                {
                    showTooltip = false;
                    firstDrag = false;
                    _onClickUp?.Invoke();
                }, (evt, st) =>
                {
                    var res = st.window.TimeContentWithOffset.Contains(evt.mousePosition) &&
                              Bounds.Contains(evt.mousePosition);
                    if (res)
                    {
                        _onRightClick?.Invoke(this);
                    }
                    return res;
                }
                , () =>
                {
                    _onMouseOverlay?.Invoke();
                }
            );
            AddManipulator(trigger);
        }
    }
}