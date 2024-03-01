using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal class TrackItem : TreeItem
    {
        [SerializeField] private List<ActorEvent> _eventList = new List<ActorEvent>();
        public List<ActorEvent> eventList => _eventList;
        
        [SerializeField] private string _type = string.Empty;
        public string type
        {
            get => _type;
            set => _type = value;
        }

        [SerializeField] private bool _nameReadOnly = true;
        public bool nameReadOnly
        {
            get => _nameReadOnly;
            set => _nameReadOnly = value;
        }
        
        [SerializeField] private Color mColorSwatch;
        public Color colorSwatch
        {
            get => mColorSwatch;
            set => mColorSwatch = value;
        }

        [SerializeField] private GUIContent mIcon;
        public GUIContent icon
        {
            get => mIcon;
            set => mIcon = value;
        }
        
        public override void Init(TreeItem parent)
        {
            base.Init(parent);

            parent.AddChild(this);
            nameReadOnly = false;
        }
        
        //CreateEventSignal
        //CreateEventDuration
        public void AddEvent(ActorEvent evt)
        {
            _eventList.Add(evt);
        }

        public void RemoveEvent(ActorEvent evt)
        {
            evt.Destroy();
            _eventList.Remove(evt);
        }

        public void RemoveAllEvent()
        {
            using (var itr = eventList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    itr.Current.Destroy();
                }
            }
            eventList.Clear();
        }

        public override string GetItemType()
        {
            return type;
        }
        
        public override void BuildEventHandles(ref List<EventHandler> list)
        {
            using (var itr = _eventList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    itr.Current.BuildEventHandles(ref list);
                }
            }
        }
        
        public override List<ActorEvent> BuildEvents()
        {
            return _eventList;
        }
        
        public override List<TreeItem> BuildRows()
        {
            List<TreeItem> list = new List<TreeItem>();
            list.Add(this);
            return list;
        }

        public override void BuildRect(float h)
        {
            rect.x = depth * indent;
            rect.y = h + padding;
            rect.width = Window.headerWidth - rect.x - 3;// headerSplitterWidth/2
            rect.height = height;
        }

        public override void Draw()
        {
            var selected = Window.HasSelect(this);

            // track color kind - swatch
            using (new GUIColorScope(colorSwatch))
            {
                var swatchRect = rect;
                swatchRect.x += indent;
                swatchRect.width = Window.Setting.trackSwatchStyle.fixedWidth;
                GUI.Label(swatchRect, GUIContent.none, Window.Setting.trackSwatchStyle);
            }

            // header background
            var backgroundColor = selected ? Window.Setting.colorSelection : Window.Setting.colorTrackHeaderBackground;
            var backgroundRect = rect;
            backgroundRect.x += (indent + Window.Setting.trackSwatchStyle.fixedWidth);
            backgroundRect.width -= (indent + Window.Setting.trackSwatchStyle.fixedWidth);
            EditorGUI.DrawRect(backgroundRect, backgroundColor);

            // track icon
            const float buttonSize = 16f;
            var iconRect = rect;
            iconRect.width = buttonSize;
            iconRect.x += (indent + Window.Setting.trackSwatchStyle.fixedWidth + padding);
            iconRect.y += (iconRect.height - iconRect.width) * 0.5f;
            GUI.Box(iconRect, icon, GUIStyle.none);

            // track name
            var labelRect = rect;
            labelRect.x += (indent + Window.Setting.trackSwatchStyle.fixedWidth + padding + buttonSize + padding);
            var size = Window.Setting.groupFont.CalcSize(new GUIContent(itemName));
            labelRect.width = Mathf.Max(50, Mathf.Min(size.x, Window.headerWidth-2*buttonSize- labelRect.x));
            if (nameReadOnly)
            {
                EditorGUI.LabelField(labelRect, itemName, Window.Setting.groupFont);
            }
            else
            {
                var textColor = selected ? Window.Setting.colorWhite : Window.Setting.groupFont.normal.textColor;
                string newName;
                EditorGUI.BeginChangeCheck();
                using (new GUIStyleScope(Window.Setting.groupFont, textColor))
                {
                    newName = EditorGUI.DelayedTextField(labelRect, itemName, Window.Setting.groupFont);
                }

                if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(newName))
                {
                    OnChangeTrackName(newName);
                }
            }

            // track context menu
            var buttonRect = rect;
            buttonRect.x = Window.headerWidth - buttonSize - padding;
            buttonRect.y += (minHeight - buttonSize) * 0.5f;
            buttonRect.width = buttonSize;
            buttonRect.height = buttonSize;
            if (GUI.Button(buttonRect, GUIContent.none, Window.Setting.trackOptions))
            {
                Window.ShowContextMenu(this, null);
            }

            // track content
            var colorContent = selected ? Window.Setting.colorTrackBackgroundSelected : Window.Setting.colorTrackBackground;
            var contentRect = rect;
            contentRect.x = Window.headerWidth;
            contentRect.width = Window.rectContent.width;
            EditorGUI.DrawRect(contentRect, colorContent);

            // child
            using (var itr = children.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    itr.Current.Draw();
                }
            }

            // event
            using (var itr = _eventList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    itr.Current.Draw();
                }
            }
        }
        
        public void OnChangeTrackName(string trackName)
        {
            itemName = trackName;
            using (var itr = _eventList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    itr.Current.OnChangeTrackName(trackName);
                }
            }
        }
    }
}