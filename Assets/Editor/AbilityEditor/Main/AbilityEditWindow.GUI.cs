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
        [SerializeReference] private TreeItem itemTreeView;
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
            
            DrawTreeView();
            DrawSplitter();
            DrawTimeline();
            DrawTimeCursor(AreaType.Lines);
            DrawTimelineRange();
            DrawTimeCursor(AreaType.Header);
            DrawInspector();
            DrawOverlay();
        }

        private void InitGUI()
        {
            InitLayout();
            if (actorSplitter == null)
            {
                actorSplitter = CreateInstance<SpliteWindow>();
                actorSplitter.Init(width => { headerWidth = width; }, () => headerWidth);

                propertySplitter = CreateInstance<SpliteWindow>();
                propertySplitter.Init((width) => { propertyWidth = inspectorWidth - (rectWindow.width - width) - 2; },
                    () => rectWindow.width - inspectorWidth + propertyWidth + 2);

                inspectorSplitter = CreateInstance<SpliteWindow>();
                inspectorSplitter.Init((width) => { inspectorWidth = rectWindow.width - width; }, 
                    () => rectWindow.width - inspectorWidth);
                FrameRate = 100; 
                timeFormat = TimeFormatType.Seconds;
            }

            if (itemTreeView == null)
            {
                itemTreeView = ScriptableObject.CreateInstance<TreeItem>();
                itemTreeView.Init(null);
                itemTreeView.AddManipulator(new EventHandlerManipulator(itemTreeView));
                itemTreeView.AddManipulator(new EventManipulator(itemTreeView));
                itemTreeView.AddManipulator(new ContextMenuManipulator(itemTreeView));

                CreateActorGroup(itemTreeView, Setting.groupAnimationName);
                CreateActorGroup(itemTreeView, Setting.groupActionName);
                CreateActorGroup(itemTreeView, Setting.groupEffectName);
                CreateActorGroup(itemTreeView, Setting.groupNoticeName);
                
                DeserializeAll();
            }
        }

        private void UpdateGUI()
        {
            var height = 0f;
            
            // 此处计算item总高度
            var list = itemTreeView.BuildRows();
            for (int i = 0; i < list.Count; ++i)
            {
                var item = list[i];
                item.BuildRect(height);
                height += item.totalHeight;
            }

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
        
        private void DrawTreeView()
        {
            EditorGUI.DrawRect(rectClient, Setting.colorSequenceBackground);
            scrollPosition = GUI.BeginScrollView(rectClient, scrollPosition, rectClientView, GUIStyle.none, GUI.skin.verticalScrollbar);
            {
                if (Ability != null)
                {
                    itemTreeView.Draw();
                }
                else
                {
                    GUILayout.Label("请先选择Ability");
                }
            
            }
            GUI.EndScrollView();
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
                        menu.AddItem(new GUIContent("Frames"), timeFormat == TimeFormatType.Frames, () =>
                        {
                            FrameRate = 60; timeFormat = TimeFormatType.Frames;
                        });
                        menu.AddItem(new GUIContent("Seconds"), timeFormat == TimeFormatType.Seconds, () =>
                        {
                            FrameRate = 100; timeFormat = TimeFormatType.Seconds;
                        });
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
        
        private void BuildItemEventTree(DataEvent evt, string groupName, string trackType, Color trackColor,
            GUIContent trackIcon)
        {
            GroupItem group = null;
            using (var itr = itemTreeView.children.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    if (itr.Current.itemName == groupName)
                    {
                        group = itr.Current as GroupItem;
                        break;
                    }
                }
            }

            TrackItem track = null;
            
            using (var itr = group.children.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    if (itr.Current.ItemIndex == evt.TrackIndex)
                    {
                        track = itr.Current as TrackItem;
                        break;
                    }
                }
            }
            
            if  (track == null)
            {
                track = CreateActorTrack(group, trackType, trackColor, trackIcon, false);
                track.ItemIndex = evt.TrackIndex;
                track.itemName = evt.TrackName;
            }
            
            var posX = Time2Pos(ToSecond(evt.TriggerTime)) + rectTimeArea.x;
            var style = evt.TriggerType == ETriggerType.Signal ? EventStyle.Signal : EventStyle.Duration;
            var ae = CreateActorEvent(track, posX, style, trackType, false);
            ae.eventProperty = evt;
        }

        public void BuildTreeView(AbilityData ab)
        {
            Ability = ab;
            
            ab.ForceSort();

            // 事件列表
            using var itr = ab.EventList.GetEnumerator();
            while (itr.MoveNext())
            {
                switch (itr.Current.EventType)
                {
                    case EventDataType.PlayAnim:
                        BuildItemEventTree(itr.Current, Setting.groupAnimationName, Setting.trackAnimationType,
                            Setting.colorAnimation, Setting.animationTrackIcon);
                        break;
                    case EventDataType.PlayEffect:
                        BuildItemEventTree(itr.Current, Setting.groupEffectName, Setting.trackEffectType,
                            Setting.colorEffect, Setting.effectTrackIcon);
                        break;
                    case EventDataType.NoticeMessage:
                        BuildItemEventTree(itr.Current, Setting.groupNoticeName, Setting.trackNoticeType,
                            Setting.colorNotice, Setting.otherTrackIcon);
                        break;
                    case EventDataType.DoAction:
                        BuildItemEventTree(itr.Current, Setting.groupActionName, Setting.trackActionType,
                            Setting.colorAction, Setting.actionTrackIcon);
                        break;
                    case EventDataType.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void ClearTreeView()
        {
            Ability = null;

            using var itr = itemTreeView.children.GetEnumerator();
            while (itr.MoveNext())
            {
                itr.Current.children.Clear();
            }
        }
    }
}