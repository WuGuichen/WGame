using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal class EventHandlerManipulator : IManipulator
    {
        private TreeItem owner = null;
        private EventHandler dragHandle = null;

        public EventHandlerManipulator(TreeItem owner)
        {
            this.owner = owner;
        }

        protected override bool MouseDown(Event evt, AbilityEditWindow window)
        {
            if (evt.alt || evt.button != 0)
                return false;
            
            if (evt.mousePosition.x <= window.rectTimeArea.x ||
                evt.mousePosition.x >= window.rectClient.width - 2 * window.verticalScrollbarWidth ||
                evt.mousePosition.y <= window.rectClient.y ||
                evt.mousePosition.y >= window.rectWindow.height - window.horizontalScrollbarHeight)
                return false;
            
            var eventHandlerList = new List<EventHandler>();
            
            owner.BuildEventHandles(ref eventHandlerList);
            
            // 拿到handler
            using (var itr = eventHandlerList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    var pos = window.MousePos2ViewPos(evt.mousePosition);
                    if (itr.Current.manipulatorRect.Contains(pos))
                    {
                        dragHandle = itr.Current;
                        break;
                    }
                }
            }
            
            if (dragHandle != null)
            {
                dragHandle.color = window.Setting.colorEventHandleSelected;
                window.AddCaptured(this);

                return true;
            }

            return false;
        }

        protected override bool MouseDrag(Event evt, AbilityEditWindow window)
        {
            if (dragHandle == null)
                return false;
            
            dragHandle.Move(evt);
            return true;
        }

        protected override bool MouseUp(Event evt, AbilityEditWindow window)
        {
            if (dragHandle == null)
                return false;
            
            dragHandle.color = window.Setting.colorEventHandle;
            dragHandle = null;
            
            window.RemoveCaptured(this);
            
            return false;
        }

        public override void Overlay(Event evt, AbilityEditWindow window)
        {
            if (dragHandle != null)
            {
                EditorGUIUtility.AddCursorRect(window.rectTimeArea, MouseCursor.SplitResizeLeftRight);
                using (new GUIGroupScope(window.rectTimeline))
                {
                    TimeIndicator.Draw(window, dragHandle.time);
                }
            }
        }
    }
}