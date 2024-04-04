using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using WGame.Ability.Editor.Custom;
using WGame.Editor;

namespace WGame.Ability.Editor
{
    internal class ActorEvent : WindowItemState
    {
        [SerializeField] private GUIContent text = new GUIContent();
        [SerializeField] private EventHandler leftHandle = null;
        [SerializeField] private EventHandler rightHandle = null;
        [SerializeField] private Tooltip _tooltip = null;
        [SerializeField] private TrackItem _parent = null;
        
        [SerializeField] public DataEvent eventProperty = null;
        
        [SerializeField] private int selectInterruptIndex = -1;
        
        [System.NonSerialized] private bool _hasExecute = false;
        

        [System.NonSerialized]
        private StringBuilder _tipBuf = new StringBuilder();

        [NonSerialized] private const float MIN_TOLERANCE = 0.000001f;
        [System.NonSerialized] private bool isFullDuration = false;
        [System.NonSerialized] private bool isLeft = false;
        [System.NonSerialized] private bool isRight = false;
        
        public TrackItem parent
        {
            get => _parent;
            set => _parent = value;
        }

        [SerializeField] private EventStyle _eventStyle = EventStyle.Signal;
        public EventStyle eventStyle
        {
            get
            {
                if (eventProperty != null)
                {
                    EventStyle style = EventStyle.Signal;
                    if (eventProperty.TriggerType == ETriggerType.Duration)
                    {
                        style = EventStyle.Duration;
                    }
                    return style;
                }
                return _eventStyle;
            }
            set
            {
                _eventStyle = value;
                if (value == EventStyle.Signal)
                {
                    duration = 0;
                    leftHandle = null;
                    rightHandle = null;
                    _tooltip = new Tooltip(Window.Setting.displayBackground, Window.Setting.tinyFont);

                    if (eventProperty != null)
                    {
                        eventProperty.TriggerType = ETriggerType.Signal;
                    }
                }
                else
                {
                    duration = Window.SnapInterval;
                    leftHandle = ScriptableObject.CreateInstance<EventHandler>();
                    leftHandle.Init(this, EventHandleType.Left);
                    rightHandle = ScriptableObject.CreateInstance<EventHandler>();
                    rightHandle.Init(this, EventHandleType.Right);
                    _tooltip = null;

                    if (eventProperty != null)
                    {
                        eventProperty.TriggerType = ETriggerType.Duration;
                    }
                }
            }
        }
        
        [SerializeField] private float _start = 0f;
        public float start
        {
            get
            {
                if (eventProperty != null)
                {
                    return Window.ToSecond(eventProperty.TriggerTime);
                }
                return _start;
            }
            set
            {
                _start = Mathf.Max(0, value);
                if (eventProperty != null)
                {
                    eventProperty.TriggerTime = Window.ToMillisecond(_start);
                }
            }
        }

        [SerializeField] private float _duration = 0f;
        public float duration
        {
            get
            {
                if (eventProperty != null)
                {
                    return Window.ToSecond(eventProperty.Duration);
                }
                return _duration;
            }
            set
            {
                if (eventStyle == EventStyle.Signal)
                {
                    _duration = 0f;
                }
                else
                {
                    _duration = Mathf.Max(0, value);
                }
                if (eventProperty != null)
                {
                    eventProperty.Duration = Window.ToMillisecond(_duration);
                }
            }
        }

        public float end
        {
            get
            {
                if (eventProperty != null)
                {
                    var s = Window.ToSecond(eventProperty.TriggerTime);
                    var d = Window.ToSecond(eventProperty.Duration);
                    return s + d;
                }
                return start + duration;
            }
        }

        public bool hasExecute
        {
            get => _hasExecute;
            set => _hasExecute = value;
        }

        public override string GetItemType()
        {
            return this.GetType().ToString();
        }
        
        
        public void Init(TrackItem parent, float posX, EventStyle style, string eventTag, bool manual)
        {
            this.parent = parent;
            this.parent.AddEvent(this);

            eventProperty = new DataEvent();
            eventProperty.TrackName = parent.itemName;
            //eventProperty.TriggerTime = window.ToMillisecond(start);
            
            if  (eventTag == Window.Setting.trackAnimationType)
            {
                eventProperty.EventType = EventDataType.PlayAnim;
            }
            else if (eventTag == Window.Setting.trackActionType)
            {
                eventProperty.EventType = EventDataType.DoAction;
            }
            else if (eventTag == Window.Setting.trackInterruptType)
            {
                eventProperty.EventType = EventDataType.Interrupt;
            }
            else if (eventTag == Window.Setting.trackEffectType)
            {
                eventProperty.EventType = EventDataType.PlayEffect;
            }
            else if (eventTag == Window.Setting.trackNoticeType)
            {
                eventProperty.EventType = EventDataType.NoticeMessage;
            }

            if (manual)
            {
                Window.Ability.EventList.Add(eventProperty);
            }

            start = Window.SnapTime3(Window.Pos2Time(posX));
            eventStyle = style;
            if (style == EventStyle.Duration)
            {
                duration = 0.2f;
            }
        }
        
        public void Destroy()
        {
            if (Window.Ability != null)
            {
                Window.Ability.EventList.Remove(eventProperty);
            }
        }
        
        public void BuildRect()
        {
            var x = Window.Time2Pos(start) + Window.rectTimeArea.x;
            var width = Mathf.Max(Window.FrameWidth, 8);

            if (duration > 0)
            {
                width = Window.Time2Pos(end) - Window.Time2Pos(start);
            }
            rect = new Rect(x, parent.manipulatorRect.y, width, parent.height);
        }
        
        public void Move(Event evt, float offset)
        {
            start = Window.SnapTime(Window.Pos2Time(evt.mousePosition.x) - offset);
        }
        
        public void OnSelected()
        {
            if (eventProperty != null && eventProperty.EventData != null)
            {
                switch (eventProperty.EventType)
                {
                }
            }
        }
        
        public override void Draw()
        {
            if (start < Window.ViewTimeMin || start > Window.ViewTimeMax)
                return;

            BuildRect();

            var selected = Window.HasSelect(this);
            var color = eventStyle == EventStyle.Signal ? Window.Setting.colorEventSignal : Window.Setting.colorEventDuration;
            color = eventProperty.IsEnable ? color : Window.Setting.colorUnEnabled;
            using (new GUIColorScope(color))
            {
                GUI.Box(rect, "", Window.Setting.customEventKey);
            }

            _tipBuf.Clear();
            bool isShowTime = true;
            if (eventProperty != null && eventProperty.EventData != null)
            {
                if (eventProperty.EventData is EventPlayAnim epa)
                {
                    if (!string.IsNullOrEmpty(epa.AnimName))
                    {
                        var clip = GameAssetsMgr.Inst.LoadAnimClip(epa.AnimName);
                        var offset = Window.ToSecond(epa.PlayOffsetEnd + epa.PlayOffsetStart);
                        duration = clip.length - offset;
                        if (duration < 0)
                            duration = 0;
                        _tipBuf.Append(clip.name);
                        _tipBuf.Append(",");
                        _tipBuf.Append((epa.LayerType switch { 0 => "Base", 1 => "UpperBody", 2 => "LowerBody" }));
                        _tipBuf.Append(",");
                        var transRect = rect;
                        transRect.width = Window.Time2Pos(Window.ToSecond(epa.TransDuration));
                        using (new GUIColorScope(Window.Setting.colorDuration))
                        {
                            GUI.Box(transRect, "", Window.Setting.customEventKey);
                            GUI.color = Color.black;
                            GUI.Label(transRect, epa.TransDuration.ToString());
                        }
                    }
                }
                else if (eventProperty.EventData is EventInterrupt ei)
                {
                    StringToIDDefine.VisualizeMotionType(ref _tipBuf, ei.BreakType);
                }
                else if (eventProperty.EventData is EventSetTimeArea esta)
                {
                    isShowTime = false;
                    StringToIDDefine.VisualizeTimeAreaType(ref _tipBuf, esta.AreaType);
                }
                else if (eventProperty.EventData is EventSetMoveParam esmp)
                {
                    var define = StringToIDDefine.DefineDict[4];
                    _tipBuf.Append(define.StringArray[esmp.ParamType]);
                    _tipBuf.Append("[");
                    _tipBuf.Append(esmp.ParamValue);
                    _tipBuf.Append("]");
                }
                else if (eventProperty.EventData is EventSetState ess)
                {
                    StringToIDDefine.VisualizeStateType(ref _tipBuf, ess.StateMask, false);
                }
                else if (eventProperty.EventData is EventLockTick elt)
                {
                    _tipBuf.Append("进度停止在此");
                    isShowTime = false;
                }
                else if (eventProperty.EventData is EventInputTriggerToMotion eitt)
                {
                    isShowTime = false;
                    _tipBuf.Append("等待");
                    StringToIDDefine.VisualizeInputType(ref _tipBuf, eitt.InputType, false);

                    _tipBuf.Append(eitt.InputValue ? "按下" : "抬起");
                    _tipBuf.Append(", 切换");

                    StringToIDDefine.VisualizeMotionType(ref _tipBuf, eitt.StateType, true);
                }
                else if (eventProperty.EventData is EventFocusKeepDist efkd)
                {
                    _tipBuf.Append("目标小于");
                    _tipBuf.Append(efkd.OffsetDist);
                    _tipBuf.Append("cm则停止移动");
                    isShowTime = false;
                }
                else if (eventProperty.EventData is EventSetOwnerAttribute esoa)
                {
                    StringToIDDefine.VisualizeAttrType(ref _tipBuf, esoa.AttrID);
                    _tipBuf.Append(esoa.Value);
                    isShowTime = false;
                }
                else if (eventProperty.EventData is EventStateToMotion estm)
                {
                    isShowTime = false;
                    _tipBuf.Append(estm.CheckType ? "如果" : "不");
                    StringToIDDefine.VisualizeStateType(ref _tipBuf, estm.WaitState);
                    _tipBuf.Append("切换");
                    StringToIDDefine.VisualizeMotionType(ref _tipBuf, estm.MotionType);
                }
            }

            if (selected)
            {
                using (new GUIColorScope(Window.Setting.colorPropertySelected))
                {
                    var selectRect = rect;
                    selectRect.height = eventStyle == EventStyle.Duration ? 6f : rect.height;
                    selectRect.y += rect.height - selectRect.height;
                    GUI.Box(selectRect, "", Window.Setting.customEventKey);
                }
            }


            if (isShowTime)
            {
                switch (eventStyle)
                {
                    case EventStyle.Signal:
                        _tipBuf.Append(Window.FormatTime(start));
                        break;
                    case EventStyle.Duration:
                        _tipBuf.Append(Window.FormatTime(duration));
                        break;
                }
            }

            text.text = _tipBuf.ToString();
            
            using (new GUIColorScope(Window.Setting.colorBlack))
            {
                var rc = rect;
                var size = EditorStyles.label.CalcSize(text);
                var offset = (rc.width - size.x) / 2;
                offset = Mathf.Max(0, offset);
                rc.x += offset;
                if (_tooltip == null)
                {
                    GUI.Label(rc, text, EditorStyles.label);
                }
            }

            if (_tooltip != null)
            {
                _tooltip.text = text.text;
                var rc = rect;
                rc.y -= 6f;
                var size = EditorStyles.label.CalcSize(text);
                rc.width = size.x;
                var offset = size.x/2;
                offset = Mathf.Max(0, offset);
                // rc.x =;
                _tooltip.bounds = rc;
            }

            if (eventStyle == EventStyle.Duration)
            {
                leftHandle.Draw();
                rightHandle.Draw();
            }
            else
            {
                _tooltip.Draw();
            }
        }
        
        public void BuildEventHandles(ref List<EventHandler> list)
        {
            if (eventStyle == EventStyle.Duration)
            {
                list.Add(leftHandle);
                list.Add(rightHandle);
            }
        }
        
        public override void DrawInspector()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Space(5);
                Window.rightScrollPos = GUILayout.BeginScrollView(Window.rightScrollPos, false, true);
                {

                    var eventType = eventProperty.EventType;
                    if (eventProperty.EventData != null)
                    {
                        if (!(eventType == EventDataType.PlayAnim
                            || eventType == EventDataType.Interrupt))
                        {
                            using (new GUIColorScope(Window.Setting.colorInspectorLabel))
                            {
                                GUILayout.Label("Event");
                            }

                            GUILayout.Space(2);
                            Window.DrawData(eventProperty);
                        }

                        GUILayout.Space(5);
                        using (new GUIColorScope(Window.Setting.colorInspectorLabel))
                        {
                            GUILayout.Label("Event Data");
                        }

                        GUILayout.Space(2);
                        switch (eventType)
                        {
                            // case EventDataType.EET_Interrupt:
                            //     DrawInspectorInterrupt();
                            //     break;
                            // case EventDataType.EET_AttackDef:
                            //     DrawInspectorAttackDef();
                            //     break;
                            default:
                                Window.DrawData(eventProperty.EventData);
                                break;
                        }


                        GUILayout.BeginHorizontal();
                        GUILayout.Label("辅助");
                        if (eventType != EventDataType.PlayAnim)
                        {
                            isFullDuration = Mathf.Abs(duration - Window.Length) < MIN_TOLERANCE && start < MIN_TOLERANCE;
                            var tmpIsFull = GUILayout.Toggle(isFullDuration, "铺满");
                            if (isFullDuration != tmpIsFull)
                            {
                                if (isFullDuration)
                                {
                                    duration -= 0.2f;
                                }
                                else
                                {
                                    start = 0f;
                                    duration = Window.Length;
                                }

                                isFullDuration = tmpIsFull;
                            }
                        }
                        
                        isLeft = start <MIN_TOLERANCE;
                        var tmpLeft = GUILayout.Toggle(isLeft, "贴左侧");
                        if (isLeft != tmpLeft)
                        {
                            if (isLeft)
                            {
                                start += 0.1f;
                            }
                            else
                            {
                                start = 0f;
                            }
                        }

                        isRight = Math.Abs(start + duration - Window.Length) < MIN_TOLERANCE;
                        var tmpRight = GUILayout.Toggle(isRight, "贴右侧");
                        if (isRight != tmpRight)
                        {
                            if (isRight)
                            {
                                start -= 0.1f;
                            }
                            else
                            {
                                start = Window.Length - duration;
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }
        
        public void OnChangeTrackName(string trackName)
        {
            eventProperty.TrackName = parent.itemName;
        }
        
        public ActorEvent Clone()
        {
            var posX = Window.Time2Pos(start);
            var newAE = Window.CreateActorEvent(parent, posX, eventStyle, parent.GetItemType());
        
            newAE.ItemIndex = this.ItemIndex;
            this.eventProperty.CopyTo(newAE.eventProperty);
        
            return newAE;
        }
    }
}