using System.Collections.Generic;
using System.Text;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using WGame.Editor;

namespace WGame.Ability.Editor
{
    internal class ActorEvent : WindowItemState
    {
        [SerializeField] private GUIContent text = new GUIContent();
        [SerializeField] private EventHandler leftHandle = null;
        [SerializeField] private EventHandler rightHandle = null;
        [SerializeField] private TrackItem _parent = null;
        
        [SerializeField] public DataEvent eventProperty = null;
        
        [SerializeField] private int selectInterruptIndex = -1;
        
        [System.NonSerialized] private bool _hasExecute = false;
        

        [System.NonSerialized]
        private StringBuilder _tipBuf = new StringBuilder();
        
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
            var color = selected ? Window.Setting.colorRed :
                eventStyle == EventStyle.Signal ? Window.Setting.colorEventSignal : Window.Setting.colorEventDuration;
            using (new GUIColorScope(color))
            {
                GUI.Box(rect, "", Window.Setting.customEventKey);
            }

            _tipBuf.Clear();
            if (eventProperty != null && eventProperty.EventData != null && eventProperty.EventData is EventPlayAnim epa)
            {
                if (!string.IsNullOrEmpty(epa.AnimName))
                {
                    var clip =GameAssetsMgr.Inst.LoadAnimClip(epa.AnimName);
                    duration = clip.length;
                    _tipBuf.Append(clip.name);
                    _tipBuf.Append(",");
                }
            }

            switch (eventStyle)
            {
                case EventStyle.Signal:
                    _tipBuf.Append(Window.FormatTime(start));
                    break;
                case EventStyle.Duration:
                    _tipBuf.Append(Window.FormatTime(duration));
                    break;
            }

            text.text = _tipBuf.ToString();
            
            using (new GUIColorScope(Window.Setting.colorBlack))
            {
                var rc = rect;
                var size = EditorStyles.label.CalcSize(text);
                var offset = (rc.width - size.x) / 2;
                offset = Mathf.Max(0, offset);
                rc.x += offset;
                GUI.Label(rc, text, EditorStyles.label);
            }

            if (eventStyle == EventStyle.Duration)
            {
                leftHandle.Draw();
                rightHandle.Draw();
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
                    using (new GUIColorScope(Window.Setting.colorInspectorLabel))
                    {
                        GUILayout.Label("Event");
                    }

                    GUILayout.Space(2);
                    Window.DrawData(eventProperty);

                    GUILayout.Space(5);
                    using (new GUIColorScope(Window.Setting.colorInspectorLabel))
                    {
                        GUILayout.Label("Event Data");
                    }

                    GUILayout.Space(2);
                    if (eventProperty.EventData != null)
                    {
                        switch (eventProperty.EventType)
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