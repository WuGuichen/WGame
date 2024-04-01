using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WGame.Editor;

namespace WGame.Ability.Editor
{
    internal sealed partial class AbilityEditWindow
    {
        [SerializeReference] private List<IManipulator> captureList = new();
        
        [System.NonSerialized] public List<WindowItemState> selectionList = new();
        [System.NonSerialized] public List<WindowItemState> dataList = new();
        [System.NonSerialized] public List<WindowItemState> conditionList = new();
        
        private ActorEvent copyEvent = null;
        
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
        
        public void SelectData(WindowItemState item)
        {
            dataList.Clear();
            dataList.Add(item);
        }

        public void DeselectAllData()
        {
            dataList.Clear();
        }

        public ItemTreeData GetItemData()
        {
            var list = dataList.Where(x => x is ItemTreeData).ToList();
            return list.Count > 0 ? (ItemTreeData)list[0] : null;
        }
        
        public bool HasSelectData(WindowItemState item)
        {
            return dataList.Contains(item);
        }
        
        public void SelectCondition(WindowItemState item)
        {
            conditionList.Clear();
            conditionList.Add(item);
        }

        public void DeselectAllCondition()
        {
            conditionList.Clear();
        }

        public ItemCondition GetActorCondition()
        {
            var list = conditionList.Where(x => x is ItemCondition).ToList();
            return list.Count > 0 ? (ItemCondition)list[0] : null;
        }

        public bool HasSelectCondition(WindowItemState item)
        {
            return conditionList.Contains(item);
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
                else if (group.itemName == Setting.groupInterruptName)
                {
                    menu.AddItem(Setting.contextNewInterrupt, false, () =>
                    {
                        var track = CreateActorTrack(group, Setting.trackInterruptType, Setting.colorInterrupt,
                            Setting.interruptTrackIcon);
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
                else if (group.itemName == Setting.groupActionName)
                {
                    menu.AddItem(Setting.contextNewAction, false, () =>
                    {
                        var track = CreateActorTrack(group, Setting.trackActionType, Setting.colorAction,
                            Setting.actionTrackIcon);
                    });
                }
                else if (group.itemName == Setting.groupNoticeName)
                {
                    menu.AddItem(Setting.contextNewNotice, false, () =>
                    {
                        var track = CreateActorTrack(group, Setting.trackNoticeType, Setting.colorNotice,
                            Setting.otherTrackIcon);
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
                    if (actorType == Setting.trackAnimationType
                        || actorType == Setting.trackInterruptType)
                    {
                        enableSignal = false;
                        enableDuration = true;
                    }
                    else if (actorType == Setting.trackActionType)
                    {
                        enableDuration = true;
                    }

                    if (enableSignal)
                    {
                        menu.AddItem(Setting.contextAddEventSignal, false,
                            () => { CreateActorEvent(track, posX, EventStyle.Signal, actorType); });
                    }

                    if (enableDuration)
                    {
                        menu.AddItem(Setting.contextAddEventDuration, false,
                            () => { CreateActorEvent(track, posX, EventStyle.Duration, actorType); });
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
            else if (item is ActorEvent e)
            {
                if (e.eventProperty != null && e.eventProperty.EventData != null &&
                    e.eventProperty.EventData is EventPlayAnim epa)
                {
                    menu.AddItem(Setting.contextResetTotalTime, false, () =>
                    {
                        if (Ability != null)
                        {
                            //todo 重置动画播放时间
                            var clip = GameAssetsMgr.Inst.LoadAnimClip(epa.AnimName);
                            var offset = ToSecond(epa.PlayOffsetStart + epa.PlayOffsetEnd);
                            Length = ToSecond(e.eventProperty.TriggerTime) + clip.length - offset;
                        }
                    });
                }
                else
                {
                    menu.AddItem(Setting.contextDuplicateEvent, false, () =>
                    {
                        var newAE = e.Clone();
                        newAE.start += SnapInterval;
                    });
                    menu.AddItem(Setting.contextCopyEventValue, false, () => { copyEvent = e; });
                    menu.AddItem(Setting.contextPasteEventValue, false, () =>
                    {
                        if (copyEvent != null)
                        {
                            copyEvent.eventProperty.CopyTo(e.eventProperty);
                        }
                    });
                }
                
                menu.AddItem(Setting.contextDelEvent, false, () =>
                {
                    if (EditorUtility.DisplayDialog("Delete Event", "Are you sure?", "YES", "NO!"))
                    {
                        var parent = e.parent;

                        Helper.PushUndo(new Object[] { parent, e }, Setting.contextDelEvent.text);
                        Helper.PushDestroyUndo(parent, e);

                        parent.RemoveEvent(e);
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
        
        public ActorEvent CreateActorEvent(TrackItem parent, float posX, EventStyle style, string eventTag, bool forceUndo = true)
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