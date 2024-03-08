using UnityEditor;
using UnityEngine;

namespace WGame.Ability.Editor
{
    [System.Serializable]
    public enum TimeFormatType
    {
        Seconds,
        Frames
    }
    
    internal sealed partial class AbilityEditWindow
    {
        public bool EnableSnap { get; set; }

        private float _length = 16f;
        public float Length
        {
            get
            {
                if (Ability != null)
                {
                    return ToSecond(Ability.TotalTime);
                }

                return _length;
            }
            set
            {
                _length = Mathf.Max(value, 0.1f);
                if (Ability != null)
                {
                    Ability.TotalTime = Window.ToMillisecond(_length);
                }
            }
        }
        
        private float _viewTimeMin = 0f;
        public float ViewTimeMin
        {
            get => _viewTimeMin;
            set => _viewTimeMin = Mathf.Min(value, ViewTimeMax - 0.25f);
        }

        private float _viewTimeMax = 16f;
        public float ViewTimeMax
        {
            get => _viewTimeMax;
            set => _viewTimeMax = Mathf.Max(value, ViewTimeMin + 0.25f, 0);
        }
        public float ViewTime => ViewTimeMax - ViewTimeMin;
        
        public float MaxTime => Mathf.Max(ViewTimeMax, Length);
        
        private float _currentTime = 0f;
        public float CurrentTime
        {
            get => _currentTime;
            set => _currentTime = Mathf.Clamp(value, 0, Length);
        }
        
        private TimeFormatType timeFormat;
        public TimeFormatType TimeFormat
        {
            get { return timeFormat; }
            set
            {
                if (timeFormat != value)
                {
                    timeFormat = value;
                    FrameRate = value == TimeFormatType.Frames ? 60 : 100;
                }
            }
        }
        
        private int _frameRate;
        public int FrameRate
        {
            get => _frameRate;
            set
            {
                if (_frameRate == value) return;
                _frameRate = value; SnapInterval = 1f / value;
            }
        }
        
        private float _snapInterval;
        public float SnapInterval
        {
            get => _snapInterval;
            set
            {
                if (_snapInterval != value)
                {
                    _snapInterval = Mathf.Max(value, 0.001f);
                }
            }
        }

        public float FrameWidth { get; set; }
        
        public float SnapTime(float time)
        {
            if (Event.current.shift || !EnableSnap)
            {
                return time;
            }
            return SnapTime2(time);
        }

        /// <summary>
        /// 对齐interval
        /// </summary>
        public float SnapTime2(float time)
        {
            return Mathf.Round(time / SnapInterval) * SnapInterval;
        }

        public float SnapTime3(float time)
        {
            return EnableSnap ? SnapTime2(time) : time;
        }
        
        public float Time2Pos(float time)
        {
            return (time - ViewTimeMin) / ViewTime * rectTimeArea.width;
        }
        public float Pos2Time(float pos)
        {
            return (pos - headerWidth - timelineOffsetX) / rectTimeArea.width * ViewTime + ViewTimeMin;
        }
        
        public Vector2 MousePos2ViewPos(Vector2 mousePosition)
        {
            Vector2 pos = mousePosition;
            pos.y -= rectClient.y;
            pos.y += scrollPosition.y;

            return pos;
        }
        
        [System.NonSerialized] private float timeInfoStart;
        [System.NonSerialized] private float timeInfoEnd;
        [System.NonSerialized] private float timeInfoInterval;

        private System.Collections.Generic.List<float> modulos = new()
        {
            0.0000001f, 0.0000005f, 0.000001f, 0.000005f, 0.00001f, 0.00005f, 0.0001f, 0.0005f,
            0.001f, 0.005f, 0.01f, 0.05f, 0.1f, 0.5f, 1, 5, 10, 50, 100, 500,
            1000, 5000, 10000, 50000, 100000, 500000, 1000000, 5000000, 10000000
        };
        
        private void DrawTimeline()
        {
            FrameWidth = rectTimeArea.width / ViewTime * SnapInterval;

            //range bar
            var _timeMin = ViewTimeMin;
            var _timeMax = ViewTimeMax;
            EditorGUI.MinMaxSlider(rectRangebar, ref _timeMin, ref _timeMax, 0, MaxTime);
            ViewTimeMin = _timeMin;
            ViewTimeMax = _timeMax;
            if (rectRangebar.Contains(Event.current.mousePosition) && Event.current.clickCount == 2)
            {
                ResetTimelineView();
            }

            //time build
            timeInfoInterval = 1000f;
            for (var i = 0; i < modulos.Count; i++)
            {
                var count = ViewTime / modulos[i];
                if (rectTimeArea.width / count > 50)
                {
                    timeInfoInterval = modulos[i];
                    break;
                }
            }
            timeInfoInterval = Mathf.RoundToInt(timeInfoInterval / SnapInterval) * SnapInterval;
            timeInfoStart = (float)Mathf.FloorToInt(ViewTimeMin / timeInfoInterval) * timeInfoInterval;
            timeInfoEnd = (float)Mathf.CeilToInt(ViewTimeMax / timeInfoInterval) * timeInfoInterval;
            timeInfoStart = Mathf.Round(timeInfoStart * 10000) / 10000;
            timeInfoEnd = Mathf.Round(timeInfoEnd * 10000) / 10000;

            using (new GUIGroupScope(rectTimeRuler))
            {
                if (FrameWidth > 10)
                {
                    for (var i = timeInfoStart; i <= timeInfoEnd; i += SnapInterval)
                    {
                        var posX = Time2Pos(i);
                        var start = new Vector3(posX, timeRulerHeight, 0);
                        var end = new Vector3(start.x, timeRulerHeight - 5, 0);

                        Handles.color = Setting.colorWhite;
                        Handles.DrawLine(start, end);
                    }
                }
            }

            using (new GUIGroupScope(rectTimeline))
            {
                var timeInterval = GetTimeInterval(timeInfoInterval);
                for (var i = 0f; i <= timeInfoEnd; i += timeInterval)
                {
                    if (i < timeInfoStart)
                    {
                        continue;
                    }

                    var posX = Time2Pos(i);
                    var rounded = Mathf.Round(i * 10000) / 10000;
                    
                    var p1 = new Vector3(posX, timeRulerHeight - 13, 0);
                    var p2 = new Vector3(p1.x, timeRulerHeight, 0);
                    var p3 = new Vector3(p1.x, rectTimeline.height, 0);
                    
                    Handles.color = Setting.colorWhite;
                    Handles.DrawLine(p1, p2);
                    Handles.color = Setting.colorTimeline;
                    Handles.DrawLine(p2, p3);
                    
                    var text = timeFormat == TimeFormatType.Frames ? (rounded * FrameRate).ToString("0") : rounded.ToString("0.00");
                    var size = Setting.labelStyle.CalcSize(new GUIContent(text));
                    var stampRect = new Rect(posX + 2, 0, size.x, size.y);
                    GUI.Box(stampRect, text, Setting.labelStyle);
                }
            }
        }
        
        public string FormatTime(float time)
        {
            var rounded = Mathf.Round(time * 10000) / 10000;
            var frame = Mathf.RoundToInt(time * FrameRate);
            return timeFormat == TimeFormatType.Frames ? frame.ToString("0") : rounded.ToString("0.00");
        }
        
        private float GetTimeInterval(float modulo)
        {
            float timeInterval = modulo;
            if (modulo <= 5 * SnapInterval)
            {
                timeInterval = 5 * SnapInterval;
            }
            else if (modulo <= 10 * SnapInterval)
            {
                timeInterval = 10 * SnapInterval;
            }
            else if (modulo <= 50 * SnapInterval)
            {
                timeInterval = 50 * SnapInterval;
            }
            else
            {
                timeInterval = 100 * SnapInterval;
            }

            return timeInterval;
        }

        public float ToSecond(int time) => time * 0.001f;
        public int ToMillisecond(float time) => Mathf.RoundToInt(time * 1000);
        
        public void ResetTimelineView()
        {
            ViewTimeMin = 0;
            ViewTimeMax = Length;
        }
        
    }
}