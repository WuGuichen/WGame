using UnityEditor;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal class ItemTreeConditionManipulator : IManipulator
    {
        private readonly float dragHeight = 3f;

        private ItemTreeData owner = null;
        private ItemTreeData selectable = null;
        private ItemTreeData dragable = null;

        public ItemTreeConditionManipulator(ItemTreeData owner)
        {
            this.owner = owner;
        }

        protected override bool MouseDown(Event evt, AbilityEditWindow window)
        {
            if (evt.alt || evt.control || evt.shift || evt.button != 0)
                return false;

            var pos = window.MousePos2InspectorPos(evt.mousePosition);
            using (var itr = owner.Children.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    if (itr.Current.manipulatorRect.Contains(pos))
                    {
                        selectable = itr.Current;
                        break;
                    }
                }
            }

            if (selectable != null)
            {
                // selectable.OnSelected();
                window.SelectCondition(selectable);
                window.AddCaptured(this);

                return true;
            }

            return false;
        }

        protected override bool MouseDrag(Event evt, AbilityEditWindow window)
        {
            if (selectable == null)
                return false;

            var pos = window.MousePos2InspectorPos(evt.mousePosition);
            using (var itr = owner.Children.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    if (itr.Current.manipulatorRect.Contains(pos))
                    {
                        dragable = itr.Current;
                        break;
                    }
                }
            }

            if (dragable == null || dragable == selectable || dragable.Parent != selectable.Parent)
            {
                dragable = null;
                return false;
            }

            return true;
        }

        protected override bool MouseUp(Event evt, AbilityEditWindow window)
        {
            if (dragable != null)
            {
                int selectIdx = owner.Children.IndexOf(selectable);
                owner.Children.Remove(selectable);

                int idx = owner.Children.IndexOf(dragable);
                if (idx != -1)
                {
                    owner.Children.Insert(idx, selectable);
                }
                else
                {
                    owner.Children.Insert(selectIdx, selectable);
                }
            }
            
            dragable = null;
            selectable = null;
            window.RemoveCaptured(this);
            
            return false;
        }

        public override void Overlay(Event evt, AbilityEditWindow window)
        {
            if (dragable != null)
            {
                var rc = dragable.manipulatorRect;
                rc.x += window.rectInspectorRight.x;
                rc.y += (window.toobarHeight + window.timeRulerHeight - window.rightScrollPos.y + 28);
                rc.y -= (dragHeight + window.propertyHeight);
                rc.height = dragHeight;

                using (new GUIColorScope(window.Setting.colorRed))
                {
                    GUI.DrawTexture(rc, EditorGUIUtility.whiteTexture);
                }
                EditorGUIUtility.AddCursorRect(owner.manipulatorRect, MouseCursor.MoveArrow);
            }
        }
    }
}