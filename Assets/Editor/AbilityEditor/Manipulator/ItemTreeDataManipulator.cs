using UnityEngine;

namespace WGame.Ability.Editor
{
    internal class ItemTreeDataManipulator : IManipulator
    {
        private readonly float dragHeight = 3f;

        private ItemTreeData owner = null;
        private ItemTreeData selectable = null;
        private ItemTreeData dragable = null;

        public ItemTreeDataManipulator(ItemTreeData owner)
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
                selectable.OnSelected();
                window.SelectData(selectable);
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
    }
}