namespace WGame.Ability.Editor
{
    using System.Collections.Generic;
    
    internal class ItemTreeData : WindowItemState
    {
        public ItemTreeData Parent { get; protected set; } = null;
        public List<ItemTreeData> Children { get; protected set; } = new();
        public string ItemName { get; protected set; } = string.Empty;
        
        public virtual void OnSelected()
        {

        }
        
        public virtual string GetDataType()
        {
            return string.Empty;
        }
        
        public virtual void Init(ItemTreeData parent)
        {
            if (parent != null)
            {
                this.Parent = parent;
                Parent.Children.Add(this);
            }
        }
        
        public void AddChild(ItemTreeData child)
        {
            child.Parent = this;
            Children.Add(child);
        }
        
        public void RemoveChild(ItemTreeData child)
        {
            Children.Remove(child);
        }
        
        public void RemoveAll()
        {
            Children.Clear();
        }

        public override void Draw()
        {
            using (var itr = Children.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    itr.Current.Draw();
                }
            }
        }
    }
}