using System;
using UnityEditor;
using UnityEngine;

namespace WGame.Ability.Editor
{
    public enum PlayState
    {
        Stop,
        Play,
        Pause
    }
    internal sealed partial class AbilityEditWindow
    {
        [SerializeField] private float _playSpeed = 1f;
        [SerializeReference] private SpliteWindow actorSplitter;
        [SerializeReference] private SpliteWindow propertySplitter;
        [SerializeReference] private SpliteWindow inspectorSplitter;
        [NonSerialized] private PlayState _playState = PlayState.Stop;
        [NonSerialized] public Vector2 scrollPosition;
        private void OnGUI()
        {
            InitGUI();
            
            UpdateGUI();
            DrawToolBar();
            HandleManipulator();
            
            DrawSplitter();
            DrawTimeline();
            DrawTimeCursor(AreaType.Lines);
            DrawTimelineRange();
            DrawTimeCursor(AreaType.Header);
        }

        private void InitGUI()
        {
            InitLayout();
            if (actorSplitter == null)
            {
                actorSplitter = new SpliteWindow(this);
                actorSplitter.Init(width => { headerWidth = width; }, () => headerWidth);

                propertySplitter = new SpliteWindow(this);
                propertySplitter.Init((width) => { propertyWidth = inspectorWidth - (rectWindow.width - width) - 2; },
                    () => rectWindow.width - inspectorWidth + propertyWidth + 2);

                inspectorSplitter = new SpliteWindow(this);
                inspectorSplitter.Init((width) => { inspectorWidth = rectWindow.width - width; }, 
                    () => rectWindow.width - inspectorWidth);
                TimeFormat = TimeFormatType.Frames;
            }

            // if (Ability == null)
            // {
            //     Ability = new AbilityData();
            // }
        }

        private void UpdateGUI()
        {
            var height = 0f;
            
            // 此处计算item总高度

            horizontalScrollbarHeight =
                GUI.skin.horizontalScrollbar.fixedHeight + GUI.skin.horizontalScrollbar.margin.top;

            verticalScrollbarWidth = (height + 19 >= rectClient.height)
                ? GUI.skin.verticalScrollbar.fixedWidth + GUI.skin.verticalScrollbar.margin.left
                : 0;

            rectClient.height -= horizontalScrollbarHeight;
            rectClient.width -= verticalScrollbarWidth;

            rectClientView.height = height + 5;
            rectClientView.width -= verticalScrollbarWidth;

            rectTimeRuler.width -= 2 * verticalScrollbarWidth;
            
            rectTimeline.width -= 2 * verticalScrollbarWidth;
            rectTimeline.height -= horizontalScrollbarHeight;

            rectTimeArea.width -= 2 * verticalScrollbarWidth;
            rectTimeArea.height -= horizontalScrollbarHeight;
        }
        
        private void DrawTimelineRange()
        {
            var subTimelineOverlayColor = Setting.colorSubSequenceOverlay;

            if (ViewTimeMin < 0.18f)
            {
                Rect before = new Rect(headerWidth - 1, rectClient.y, 6, rectTimeArea.height);
                EditorGUI.DrawRect(before, subTimelineOverlayColor);
            }

            if (ViewTimeMax > Length)
            {
                float s = Time2Pos(Length);
                float e = Time2Pos(ViewTimeMax);
                float x = Mathf.Clamp(rectTimeArea.x + s, rectTimeArea.x, float.MaxValue);
                Rect after = new Rect(x, rectClient.y, e - Mathf.Clamp(s, 0, float.MaxValue), rectTimeArea.height);
                EditorGUI.DrawRect(after, subTimelineOverlayColor);
            }
        }
        
        private void DrawSplitter()
        {
            actorSplitter.Draw();
            propertySplitter.Draw();
            inspectorSplitter.Draw();
        }

        private void DrawToolBar()
        {

            using (new GUILayout.VerticalScope())
            {
                using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    if (GUILayout.Button(Setting.stepBack, EditorStyles.toolbarButton))
                    {
                        PreviewStepBack();
                    }
                    GUIContent playContent = (_playState == PlayState.Play) ? Setting.pause : Setting.play;
                    
                    if (GUILayout.Button(playContent, EditorStyles.toolbarButton))
                    {
                        PreviewPlay();
                    }

                    if (GUILayout.Button(Setting.stepForward, EditorStyles.toolbarButton))
                    {
                        PreviewStepForward();
                    }
                    if (GUILayout.Button(Setting.save, EditorStyles.toolbarButton))
                    {
                        Save();
                    }
                    GUILayout.Space(20);

                    _playSpeed = GUILayout.HorizontalSlider(_playSpeed, 0.1f, 2f, GUILayout.Width(80f));
                    GUILayout.Label(_playSpeed.ToString("0.00x"));
                    if (GUILayout.Button(Setting.setting, EditorStyles.toolbarButton, GUILayout.Width(20)))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("0.1x"), _playSpeed.Equals(0.1f), () => 
                        {
                            _playSpeed = 0.1f;
                        });
                        menu.AddItem(new GUIContent("0.5x"), _playSpeed.Equals(0.5f), () =>
                        {
                            _playSpeed = 0.5f;
                        });
                        menu.AddItem(new GUIContent("1x"), _playSpeed.Equals(1f), () =>
                        {
                            _playSpeed = 1f;
                        });
                        menu.AddItem(new GUIContent("1.5x"), _playSpeed.Equals(1.5f), () =>
                        {
                            _playSpeed = 1.5f;
                        });
                        menu.AddItem(new GUIContent("2x"), _playSpeed.Equals(2f), () =>
                        {
                            _playSpeed = 2f;
                        });
                        menu.ShowAsContext();
                    }
                    GUILayout.FlexibleSpace();

                    if (EditorGUILayout.DropdownButton(Setting.options, UnityEngine.FocusType.Keyboard,
                            EditorStyles.toolbarDropDown))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.ShowAsContext();
                    }
                }
            }
            
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.Width(headerWidth)))
                {
                    GUILayout.FlexibleSpace();
                    string timeStr = FormatTime(CurrentTime);
                    GUILayout.Label(timeStr);
                }
            }
        }

        private void HandleManipulator()
        {
            var curEvent = Event.current;
            actorSplitter.HandleManipulatorsEvents(this, curEvent);
            propertySplitter.HandleManipulatorsEvents(this, curEvent);
            inspectorSplitter.HandleManipulatorsEvents(this, curEvent);
        }
        
        public void PreviewPlay()
        {
            _playState = _playState != PlayState.Play ? PlayState.Play : PlayState.Pause;
        }

        public void PreviewStop()
        {
            _playState = PlayState.Stop;
            ResetPreview(Length - CurrentTime);
            CurrentTime = 0f;
        }

        public void PreviewStepBack()
        {
            Preview(-deltaTime);
            Repaint();
        }

        public void PreviewStepForward()
        {
            Preview(deltaTime);
            Repaint();
        }

        private void Save()
        {
            
        }
    }
}