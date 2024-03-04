using UnityEngine;

namespace WGame.Ability.Editor
{
    internal class EventManipulator : IManipulator
    {
        private TreeItem owner = null;
        private float offset = 0;
        
        public EventManipulator(TreeItem owner)
        {
            this.owner = owner;
        }
        
        protected override bool MouseDown(Event evt, AbilityEditWindow window)
        {
            if (evt.alt || evt.button != 0)
                return false;

            if (evt.mousePosition.x >= window.rectClient.width - 2 * window.verticalScrollbarWidth)
                return false;

            offset = 0;
            ActorEvent selectEvent = null;
            var eventActorList = owner.BuildEvents();
            using (var itr = eventActorList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    var rect = itr.Current.manipulatorRect;
                    var pos = window.MousePos2ViewPos(evt.mousePosition);
                    if (rect.Contains(pos))
                    {
                        selectEvent = itr.Current;
                        break;
                    }
                }
            }

            if (selectEvent != null)
            {
                var t = window.Pos2Time(evt.mousePosition.x);
                offset = t - selectEvent.start;
                selectEvent.OnSelected();
                window.Select(selectEvent);
                return true;
            }

            window.DeselectAllEvent();
            window.Repaint();

            return false;
        }

        protected override bool MouseDrag(Event evt, AbilityEditWindow window)
        {
            if (evt.button != 0)
                return false;

            var selectList = window.GetAllActorEvent();
            if (selectList.Count == 0)
                return false;

            using (var itr = selectList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    var actor = itr.Current as ActorEvent;
                    actor.Move(evt, offset);
                }
            }

            window.AddCaptured(this);

            return true;
        }

        protected override bool MouseUp(Event evt, AbilityEditWindow window)
        {
            if (evt.button != 0)
                return false;

            window.RemoveCaptured(this);

            return true;
        }

        public override void Overlay(Event evt, AbilityEditWindow window)
        {
            var selectList = window.GetAllActorEvent();
            if (selectList.Count > 0)
            {
                using (new GUIGroupScope(window.rectTimeline))
                {
                    var actor = selectList[0] as ActorEvent;
                    switch (actor.eventStyle)
                    {
                        case EventStyle.Signal:
                            TimeIndicator.Draw(window, actor.start);
                            break;
                        case EventStyle.Duration:
                            TimeIndicator.Draw(window, actor.start, actor.end);
                            break;
                    }
                }
            }
        }

        protected override bool ContextClick(Event evt, AbilityEditWindow window)
        {
            if (evt.alt)
                return false;

            ActorEvent selectEvent = null;
            var eventActorList = owner.BuildEvents();
            using (var itr = eventActorList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    var rect = itr.Current.manipulatorRect;
                    var pos = window.MousePos2ViewPos(evt.mousePosition);
                    if (rect.Contains(pos))
                    {
                        selectEvent = itr.Current;
                        break;
                    }
                }
            }

            if (selectEvent != null)
            {
                window.ShowContextMenu(selectEvent, evt);

                return true;
            }

            return false;
        }

    }
}