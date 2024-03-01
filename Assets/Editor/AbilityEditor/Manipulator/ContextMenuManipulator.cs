using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal class ContextMenuManipulator : IManipulator
    {
        private TreeItem owner = null;

        public ContextMenuManipulator(TreeItem owner)
        {
            this.owner = owner;
        }

        protected override bool ContextClick(Event evt, AbilityEditWindow window)
        {
            if (evt.alt)
                return false;

            if (evt.mousePosition.x < window.headerWidth)
                return false;

            var list = owner.BuildRows();
            var selectable = new List<TreeItem>();
            using (var itr = list.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    var rect = itr.Current.manipulatorRect;
                    rect.width += window.rectContent.width;
                    var pos = window.MousePos2ViewPos(evt.mousePosition);
                    if (rect.Contains(pos))
                    {
                        selectable.Add(itr.Current);
                    }
                }
            }

            var item = selectable.OrderByDescending(x => x.depth).FirstOrDefault();
            window.ShowContextMenu(item, evt);

            return true;
        }
    }
}