using System.Collections.Generic;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal class TreeItem : WindowItemState
    {
        [SerializeField] protected bool expand = false;
        [SerializeField] protected float indent = 15f;
        [SerializeField] protected float padding = 5f;
        [SerializeField] protected float itemHeight = 32f;

        [SerializeField] protected TreeItem parent;
        [SerializeField] protected List<TreeItem> _children = new List<TreeItem>();

        public List<TreeItem> children
        {
            get => _children;
            set => _children = value;
        }
        
        [SerializeField] protected string _itemName = string.Empty;
        
        public string itemName
        {
            get => _itemName;
            set => _itemName = value;
        }

        [SerializeField] protected int _depth = -1;
        public int depth
        {
            get => _depth;
            set => _depth = value;
        }

        public float minHeight => 32f;

        public float maxHeight => 128f;

        public float height
        {
            get => itemHeight;
            set => itemHeight = Mathf.Clamp(value, minHeight, maxHeight);
        }

        public float totalHeight => height + padding;

        public int childCount => children.Count;
        
        public virtual void Init(TreeItem parent)
        {
            this.parent = parent;

            depth = parent != null ? parent.depth + 1 : -1;
        }
        
        public TreeItem GetParent() => parent;
        
        public void AddChild(TreeItem item) => children.Add(item);
        
        public void RemoveChild(TreeItem item)
        {
            item.parent = null;
            children.Remove(item);
        }

        /// <summary>
        /// 填加到handler列表
        /// </summary>
        /// <param name="list">要添加的列表</param>
        public virtual void BuildEventHandles(ref List<EventHandler> list)
        {
            using (var itr = children.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    itr.Current.BuildEventHandles(ref list);
                }
            }
        }
        
        public virtual List<ActorEvent> BuildEvents()
        {
            List<ActorEvent> list = new List<ActorEvent>();
            using (var itr = children.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    var l = itr.Current.BuildEvents();
                    list.AddRange(l);
                }
            }
            return list;
        }
        
        public virtual List<TreeItem> BuildRows()
        {
            List<TreeItem> list = new List<TreeItem>();
            using (var itr = children.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    var l = itr.Current.BuildRows();
                    list.AddRange(l);
                }
            }
            return list;
        }
        
        public virtual void BuildRect(float h)
        {

        }
        
        public override void Draw()
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