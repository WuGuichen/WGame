using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

            //if (window.frameWidth <= 10)
            //    return;

            BuildRect();

            var selected = Window.HasSelect(this);
            var color = selected ? Window.Setting.colorRed :
                eventStyle == EventStyle.Signal ? Window.Setting.colorEventSignal : Window.Setting.colorEventDuration;
            using (new GUIColorScope(color))
            {
                GUI.Box(rect, "", Window.Setting.customEventKey);
            }

            if (eventProperty != null && eventProperty.EventData != null && eventProperty.EventData is EventPlayAnim epa)
            {
                if (!string.IsNullOrEmpty(epa.AnimName))
                {
                    duration = 0.2f;
                }
            }

            switch (eventStyle)
            {
                case EventStyle.Signal:
                    text.text = Window.FormatTime(start);
                    break;
                case EventStyle.Duration:
                    text.text = Window.FormatTime(duration);
                    break;
            }
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
            
        }
        
        public void OnChangeTrackName(string trackName)
        {
            eventProperty.TrackName = parent.itemName;
        }
        
        // public ActorEvent Clone()
        // {
        //     var posX = Window.Time2Pos(start);
        //     var newAE = Window.CreateActorEvent(parent, posX, eventStyle, parent.GetActorType());
        //
        //     newAE.ActorIndex = this.ActorIndex;
        //     this.eventProperty.CopyTo(newAE.eventProperty);
        //
        //     return newAE;
        // }
    }
}