using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal class GroupItem : TreeItem
    {
        public override void Init(TreeItem parent)
        {
            base.Init(parent);

            parent.AddChild(this);
            expand = true;
        }

        public override string GetItemType()
        {
            return this.GetType().ToString();
        }

        public override void BuildEventHandles(ref List<EventHandler> list)
        {
            if (expand)
            {
                using (var itr = children.GetEnumerator())
                {
                    while (itr.MoveNext())
                    {
                        itr.Current.BuildEventHandles(ref list);
                    }
                }
            }
        }

        public override List<ActorEvent> BuildEvents()
        {
            var list = new List<ActorEvent>();
            if (expand)
            {
                using (var itr = children.GetEnumerator())
                {
                    while (itr.MoveNext())
                    {
                        var l = itr.Current.BuildEvents();
                        list.AddRange(l);
                    }
                }
            }
            return list;
        }
        
        public override List<TreeItem> BuildRows()
        {
            List<TreeItem> list = new List<TreeItem>();
            list.Add(this);

            if (expand)
            {
                using (var itr = children.GetEnumerator())
                {
                    while (itr.MoveNext())
                    {
                        var l = itr.Current.BuildRows();
                        list.AddRange(l);
                    }
                }
            }

            return list;
        }
        
        public override void BuildRect(float h)
        {
            rect.x = depth * indent;
            rect.y = h + padding;
            rect.width = Window.headerWidth - rect.x - 3;// headerSplitterWidth/2
            rect.height = height;
            if (expand)
            {
                using (var itr = children.GetEnumerator())
                {
                    while (itr.MoveNext())
                    {
                        rect.height += itr.Current.totalHeight;
                    }
                }
            }
        }
        
        public override void Draw()
        {
            var selected = Window.HasSelect(this);
            var foldoutRect = rect;
            foldoutRect.width = indent;
            foldoutRect.height = indent;
            expand = EditorGUI.Foldout(foldoutRect, expand, GUIContent.none, EditorStyles.foldout);

            // header
            var headerRect = rect;
            headerRect.x += indent;
            headerRect.width -= indent;

            var color = selected ? Window.Setting.colorSelection : Window.Setting.colorGroup;
            using (new GUIColorScope(color))
            {
                GUI.Box(headerRect, GUIContent.none, Window.Setting.groupBackground);
            }

            // group name
            const float buttonSize = 16f;
            var labelRect = rect;
            labelRect.x += indent;
            var size = EditorStyles.whiteLargeLabel.CalcSize(new GUIContent(itemName));
            labelRect.y += (minHeight - size.y) * 0.5f;
            labelRect.width = Window.headerWidth - labelRect.x - buttonSize;
            EditorGUI.LabelField(labelRect, itemName, EditorStyles.whiteLargeLabel);

            // button create
            var buttonRect = rect;
            buttonRect.x = Window.headerWidth - buttonSize - padding;
            buttonRect.y += (minHeight - buttonSize) * 0.5f;
            buttonRect.width = buttonSize;
            buttonRect.height = buttonSize;
            if (GUI.Button(buttonRect, Window.Setting.createAddNew, Window.Setting.trackGroupAddButton))
            {
                Window.ShowContextMenu(this, null);
            }

            // content
            var contentRect = rect;
            contentRect.x = Window.headerWidth;
            contentRect.width = Window.rectContent.width;

            color = selected ? Window.Setting.colorTrackBackgroundSelected : Window.Setting.colorGroupTrackBackground;
            EditorGUI.DrawRect(contentRect, color);
            if (!expand && children != null && children.Count > 0)
            {
                contentRect.y += padding;
                contentRect.height -= padding * 2;
                EditorGUI.DrawRect(contentRect, Window.Setting.colorClipUnion);
            }

            // child
            if (expand)
            {
                using (var itr = children.GetEnumerator())
                {
                    while (itr.MoveNext())
                    {
                        itr.Current.Draw();
                    }
                }
            }
        }

    }
}