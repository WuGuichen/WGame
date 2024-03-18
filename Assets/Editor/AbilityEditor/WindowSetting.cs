using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace WGame.Ability.Editor
{
    [CreateAssetMenu(fileName = "AbilityWindowSetting", menuName = "Ability/Create Settings", order = 1)]
    public class WindowSetting : ScriptableObject
    {
        [SerializeField]
        private GUISkin skin;
        
        public string groupAnimationName = "Animation Group";
        public string groupEffectName = "Effect Group";
        public string groupNoticeName = "Notice Group";
        public string groupActionName = "Action Group";
        
        public string trackAnimationType = "Animation";
        public string trackEffectType = "Effect";
        public string trackNoticeType = "Notice";
        public string trackActionType = "Action";
        
        public GUIContent contextNewAnimation = new GUIContent("New Animation");
        public GUIContent contextNewEffect = new GUIContent("New Effect");
        public GUIContent contextNewNotice = new GUIContent("New Notice");
        public GUIContent contextNewAction = new GUIContent("New Action");
        public GUIContent contextAddEventSignal = new GUIContent("Add Event Signal");
        public GUIContent contextAddEventDuration = new GUIContent("Add Event Duration");
        public GUIContent contextDelTrack = new GUIContent("Del Track");
        public GUIContent contextDelEvent = new GUIContent("Del Event");
        public GUIContent contextDuplicateEvent = new GUIContent("Duplicate Event");
        public GUIContent contextCopyEventValue = new GUIContent("Copy Event Value");
        public GUIContent contextPasteEventValue = new GUIContent("Paste Event Value");
        public GUIContent contextDelProperty = new GUIContent("Del Property");
        public GUIContent contextResetTotalTime = new GUIContent("Reset TotalTime");
        
        public Color colorWhite = new Color(1.000f, 1.000f, 1.000f, 1.000f);
        public Color colorBlack = new Color(0.000f, 0.000f, 0.000f, 1.000f);
        public Color colorRed = new Color(1.000f, 0.000f, 0.000f, 1.000f);
        public Color colorGreen = new Color(0.000f, 1.000f, 0.000f, 1.000f);
        public Color colorBlue = new Color(0.000f, 0.000f, 1.000f, 1.000f);
        public Color colorSelection = new Color(0.239f, 0.369f, 0.596f, 1.000f);
        public Color colorGroup = new Color(0.153f, 0.216f, 0.224f, 1.000f);
        public Color colorGroupTrackBackground = new Color(0.080f, 0.305f, 0.328f, 0.178f);
        public Color colorAnimation = new Color(0.141f, 0.333f, 0.537f, 1.000f);
        public Color colorEffect = new Color(0.000f, 0.597f, 0.128f, 1.000f);
        public Color colorNotice = new Color(0.100f, 0.397f, 0.028f, 1.000f);
        public Color colorAction = new Color(0.900f, 0.397f, 0.328f, 1.000f);
        public Color colorAudio = new Color(1.000f, 0.635f, 0.000f, 1.000f);
        public Color colorCamera = new Color(0.702f, 0.302f, 0.302f, 1.000f);
        public Color colorInterrupt = new Color(0.3215f, 0.827f, 0.960f, 1.000f);
        public Color colorTrackBackground = new Color(0.216f, 0.216f, 0.216f, 0.628f);
        public Color colorTrackHeaderBackground = new Color(0.255f, 0.255f, 0.255f, 1.000f);
        public Color colorTrackBackgroundSelected = new Color(0.260f, 0.339f, 0.477f, 0.516f);
        public Color colorClipUnion = new Color(0.229f, 0.280f, 0.316f, 0.709f);
        public Color colorTopOutline3 = new Color(0.274f, 0.274f, 0.274f, 1.000f);
        public Color colorDurationLine = new Color(0.153f, 0.231f, 0.376f, 0.709f);
        public Color colorDuration = new Color(0.660f, 0.660f, 0.660f, 1.000f);
        public Color colorSequenceBackground = new Color(0.161f, 0.161f, 0.161f, 1.000f);
        public Color colorTooltipBackground = new Color(0.114f, 0.125f, 0.129f, 1.000f);
        public Color colorSubSequenceOverlay = new Color(0.009f, 0.080f, 0.080f, 0.286f);
        public Color colorTimeline = new Color(0.280f, 0.280f, 0.280f, 1.000f);
        public Color colorEndmarker = new Color(0.239f, 0.369f, 0.596f, 1.000f);
        public Color colorEventSignal = new Color(1.000f, 1.000f, 1.000f, 0.800f);
        public Color colorEventDuration = new Color(0.289f, 0.943f, 0.616f, 1.000f);
        public Color colorEventHandle = new Color(0.000f, 0.000f, 0.000f, 0.000f);
        public Color colorEventHandleSelected = new Color(0.6f, 0.6f, 1, 1f);
        public Color colorInspectorLabel = new Color(0, 1, 1, 1);
        public Color colorPropertySelected = new Color(1.000f, 0.004f, 0.004f, 1.000f);
        public Color colorRefreshInterrupt = new Color(0.000f, 1.000f, 1.000f, 1.000f);

        public GUIContent stepBack { get; private set; }
        public GUIContent stepForward { get; private set; }
        public GUIContent play { get; private set; }
        public GUIContent pause { get; private set; }
        public GUIContent stop { get; private set; }
        public GUIContent save { get; private set; }
        public GUIContent setting { get; private set; }
        public GUIContent help { get; private set; }
        public GUIContent options { get; private set; }

        public GUIContent createAddNew { get; private set; }

        public GUIContent animationTrackIcon { get; private set; }

        public GUIContent effectTrackIcon { get; private set; }

        public GUIContent audioTrackIcon { get; private set; }

        public GUIContent cameraTrackIcon { get; private set; }

        public GUIContent otherTrackIcon { get; private set; }

        public GUIContent attackTrackIcon { get; private set; }
        public GUIContent actionTrackIcon { get; private set; }

        public GUIContent interruptTrackIcon { get; private set; }

        public GUIStyle groupBackground { get; private set; }

        public GUIStyle trackSwatchStyle { get; private set; }

        public GUIStyle displayBackground { get; private set; }

        public GUIStyle tinyFont { get; private set; }

        public GUIStyle groupFont { get; private set; }

        public GUIStyle trackGroupAddButton { get; private set; }

        public GUIStyle trackOptions { get; private set; }

        public GUIStyle timeCursor { get; private set; }

        public GUIStyle endmarker { get; private set; }

        public GUIStyle leftArrowStyle { get; private set; }

        public GUIStyle rightArrowStyle { get; private set; }

        public GUIStyle timelineScrubberStyle { get; private set; }

        public GUIStyle horizontalScrollbar { get; private set; }

        public GUIStyle labelStyle { get; private set; }

        public GUIStyle appToolbar { get; private set; }

        public GUIStyle btnToolBoxStyle { get; private set; }

        public GUIStyle normalBoxStyle { get; private set; }

        public GUIStyle propertyBackground { get; private set; }

        public GUIStyle customHollowFrame { get; private set; }

        public GUIStyle customEventKey { get; private set; }

        public GUIStyle customClipBox { get; private set; }

        public static GUIStyle GetGUIStyle(string s)
        {
            return EditorStyles.FromUSS(s);
        }

        public void Init()
        {
            stepBack = EditorGUIUtility.IconContent("d_StepLeftButton", "Go to the previous frame");
            stepForward = EditorGUIUtility.IconContent("d_StepButton", "Go to the next frame");
            play = EditorGUIUtility.IconContent("d_PlayButton", "Play the action");
            pause = EditorGUIUtility.IconContent("d_PauseButton");
            stop = EditorGUIUtility.IconContent("d_PreMatQuad", "Stop the action");
            save = EditorGUIUtility.IconContent("d_SaveAs", "Save all");
            setting = EditorGUIUtility.IconContent("AnimationWrapModeMenu", "Setting");
            options = EditorGUIUtility.IconContent("_Popup", "Options");
            help = EditorGUIUtility.IconContent("_Help", "Help");

            createAddNew = EditorGUIUtility.IconContent("CreateAddNew");
            animationTrackIcon = EditorGUIUtility.IconContent("AnimationClip Icon");
            effectTrackIcon = EditorGUIUtility.IconContent("ParticleSystem Icon");
            audioTrackIcon = EditorGUIUtility.IconContent("AudioSource Icon");
            cameraTrackIcon = EditorGUIUtility.IconContent("Camera Icon");
            otherTrackIcon = EditorGUIUtility.IconContent("Tilemap Icon");
            attackTrackIcon = EditorGUIUtility.IconContent("VisualEffectAsset Icon");
            actionTrackIcon = EditorGUIUtility.IconContent("VisualEffectAsset Icon");
            interruptTrackIcon = EditorGUIUtility.IconContent("BlendTree Icon");

            groupBackground = GetGUIStyle("groupBackground");
            trackSwatchStyle = GetGUIStyle("Icon-TrackHeaderSwatch");
            displayBackground = GetGUIStyle("sequenceClip");
            tinyFont = GetGUIStyle("tinyFont");
            groupFont = GetGUIStyle("sequenceGroupFont");
            trackGroupAddButton = GetGUIStyle("sequenceTrackGroupAddButton");
            trackOptions = GetGUIStyle("Icon-TrackOptions");
            timeCursor = GetGUIStyle("Icon-TimeCursor");
            endmarker = endmarker = GetGUIStyle("Icon-Endmarker");
            colorEndmarker = endmarker.normal.textColor;

            leftArrowStyle = GUI.skin.GetStyle("horizontalscrollbarleftbutton");
            rightArrowStyle = GUI.skin.GetStyle("horizontalscrollbarrightbutton");
            timelineScrubberStyle = GUI.skin.GetStyle("HorizontalMinMaxScrollbarThumb");
            horizontalScrollbar = GUI.skin.GetStyle("horizontalscrollbar");
            labelStyle = GUI.skin.GetStyle("label");
            appToolbar = GUI.skin.GetStyle("AppToolbarButtonMid");
            btnToolBoxStyle = GUI.skin.GetStyle("CN Box");
            propertyBackground = GUI.skin.GetStyle("DD Background");
            normalBoxStyle = GUI.skin.GetStyle("box");

            customHollowFrame = skin.GetStyle("HollowFrame");
            customEventKey = skin.GetStyle("EventKey");
            customClipBox = skin.GetStyle("ClipBox");
        }

    }
}