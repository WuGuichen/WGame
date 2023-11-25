using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Motion
{
    class CodeCommandItem : TimeAreaItem
    {
        protected override string ToolTip
        {
            get
            {
                for (int i = 0; i < ByteCodeType.Count; i++)
                {
                    if ((ByteCodeType.Triggers[i] & commandType) > 0)
                    {
                        return ByteCodeType.TriggerNames[i];
                    }
                }

                return "未定义";
            }
        }

        public bool isShowTip = false;

        private readonly Action<CodeCommandItem> _onRightClick;
        private readonly Action<CodeCommandItem, double> _onDragTrigger;
        private readonly Action _onClickUp;
        private readonly Action _onMouseOverlay;

        // 毫秒(千分比)
        public int commandTime;
        public int commandParam;
        private int _commandType;

        public int commandType 
        {
            get => _commandType;
            set
            {
                _commandType = value;
                Color color = Color.white;
                for (int i = 0; i < 32; i++)
                {
                    if (((1 << i) & _commandType) > 0)
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

        public CodeCommandItem(GUIStyle style, WindowState state,Action<double> onDrag, Action onMouseOverlay, Action onClickUp,Action<CodeCommandItem, double> onDragTrigger, bool alwaysShowTooltip = false, Action<CodeCommandItem> onRightClick = null, float triggerTime = 0f) : base(style, state, onDrag, alwaysShowTooltip)
        {
            _onRightClick = onRightClick;
            _onDragTrigger = onDragTrigger;
            _onClickUp = onClickUp;
            _onClickUp += OnClickUp;
            _onMouseOverlay = OnMouseOverLay;
            _onMouseOverlay += onMouseOverlay;
            this.commandTime = (int)(triggerTime*1000);
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