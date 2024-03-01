using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal sealed partial class AbilityEditWindow
    {
        [SerializeReference] private List<IManipulator> captureList = new();
        
        [System.NonSerialized] public List<WindowItemState> selectionList = new();
        
        public void AddCaptured(IManipulator manipulator)
        {
            if (!captureList.Contains(manipulator))
            {
                captureList.Add(manipulator);
            }
        }

        public void RemoveCaptured(IManipulator manipulator)
        {
            captureList.Remove(manipulator);
        }
        
        public void Select(WindowItemState item)
        {
            selectionList.Clear();
            selectionList.Add(item);
        }

        public void SelectMore(WindowItemState item)
        {
            if (!selectionList.Contains(item))
                selectionList.Add(item);
        }

        public void Deselect(WindowItemState item)
        {
            selectionList.Remove(item);
        }

        public void DeselectAll()
        {
            selectionList.Clear();
        }

        public void DeselectAllTrack()
        {
            selectionList = selectionList.Where(x => x is ActorEvent).ToList();
        }

        public void DeselectAllEvent()
        {
            selectionList = selectionList.Where(x => x is TreeItem).ToList();
        }
        
        public bool HasSelect(WindowItemState item)
        {
            return selectionList.Contains(item);
        }
        
        public WindowItemState GetActorTrack()
        {
            return selectionList.Count > 0 ? selectionList[0] : null;
        }

        public List<WindowItemState> GetAllActorEvent()
        {
            return selectionList.Where(x => x is ActorEvent).ToList();
        }

        public ActorEvent GetActorEvent()
        {
            var list = selectionList.Where(x => x is ActorEvent).ToList();
            return list.Count == 1 ? (ActorEvent)list[0] : null;
        }
        
        public void DrawOverlay()
        {
            if (captureList.Count > 0)
            {
                using (var itr = captureList.GetEnumerator())
                {
                    while (itr.MoveNext())
                    {
                        itr.Current.Overlay(Event.current, this);
                    }
                }
                Repaint();
            }
        }
        
        private void HandleManipulator()
        {
            var curEvent = Event.current;
            actorSplitter.HandleManipulatorsEvents(this, curEvent);
            propertySplitter.HandleManipulatorsEvents(this, curEvent);
            inspectorSplitter.HandleManipulatorsEvents(this, curEvent);
            
            if (itemTreeView != null)
            {
                itemTreeView.HandleManipulatorsEvents(this, Event.current);
            }
        }

        public void ShowContextMenu(WindowItemState item, Event evt)
        {
            var menu = new GenericMenu();
            if (item is GroupItem group)
            {
                if (group.itemName == Setting.groupAnimationName)
                {
                    menu.AddItem(Setting.contextNewAnimation, false, () =>
                    {
                        var track = CreateActorTrack(group, Setting.trackAnimationType, Setting.colorAnimation,
                            Setting.animationTrackIcon);
                    });
                }
                else if (group.itemName == Setting.groupEffectName)
                {
                    menu.AddItem(Setting.contextNewEffect, false, () =>
                    {
                        var track = CreateActorTrack(group, Setting.trackEffectType, Setting.colorEffect,
                            Setting.effectTrackIcon);
                    });
                }
            }
            else if (item is TrackItem track)
            {
                if (evt != null)
                {
                    var posX = evt.mousePosition.x;
                    var enableSignal = true;
                    var enableDuration = false;
                    var actorType = track.GetItemType();
                    if (actorType == Setting.trackAnimationType)
                    {
                        enableSignal = false;
                        enableDuration = true;
                    }

                    if (enableSignal)
                    {
                        menu.AddItem(Setting.contextAddEventSignal, false,
                            () => { CreateActorEvent(track, posX, EEventStyle.Signal, actorType); });
                    }

                    if (enableDuration)
                    {
                        menu.AddItem(Setting.contextAddEventDuration, false,
                            () => { CreateActorEvent(track, posX, EEventStyle.Duration, actorType); });
                    }

                }

                menu.AddItem(Setting.contextDelTrack, false, () =>
                {
                    if (EditorUtility.DisplayDialog("Delete Track", "Are you sure?", "YES", "NO!"))
                    {
                        var parent = track.GetParent();

                        Helper.PushUndo(new Object[] { parent, track }, track.itemName);
                        Helper.PushDestroyUndo(parent, track);

                        track.RemoveAllEvent();
                        parent.RemoveChild(track);

                        BuildTrackIndex(parent);
                    }
                });
            }

            menu.ShowAsContext();
        }
        
        public GroupItem CreateActorGroup(TreeItem parent, string name)
        {
            var group = ScriptableObject.CreateInstance<GroupItem>();
            group.Init(parent);
            group.itemName = name;

            return group;
        }

        public TrackItem CreateActorTrack(GroupItem group, string type, Color color, GUIContent icon, bool forceUndo = true)
        {
            var track = ScriptableObject.CreateInstance<TrackItem>();
            var operation = $"{type} {group.childCount}";
            if (forceUndo)
            {
                // todo Undo原理
                Helper.RegisterCreatedObjectUndo(track, operation);
                Helper.PushUndo(new Object[] { group, track }, operation);
            }

            track.Init(group);
            track.type = type;
            track.itemName = operation;
            track.colorSwatch = color;
            track.icon = icon;

            return track;
        }
        
        public ActorEvent CreateActorEvent(TrackItem parent, float posX, EEventStyle style, string eventTag, bool forceUndo = true)
        {
            var evt = ScriptableObject.CreateInstance<ActorEvent>();

            if (forceUndo)
            {
                string operation = string.Format("{0} Event {1}", parent.GetItemType(), parent.eventList.Count);
                Helper.RegisterCreatedObjectUndo(evt, operation);
                Helper.PushUndo(new Object[] { parent, evt }, operation);
            }

            evt.Init(parent, posX, style, eventTag, forceUndo);
            RefreshTrackIndex(parent);

            return evt;
        }
        
        public void RefreshTrackIndex(TrackItem track)
        {
            var group = track.GetParent();
            var index = group.children.IndexOf(track);

            using (var itr = track.eventList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    itr.Current.eventProperty.TrackIndex = index;
                }
            }
        }
        
        public void BuildTrackIndex(TreeItem group)
        {
            using (var itr = group.children.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    RefreshTrackIndex(itr.Current as TrackItem);
                }
            }
            if (Ability != null)
            {
                Ability.ForceSort();
            }
        }
    }
}