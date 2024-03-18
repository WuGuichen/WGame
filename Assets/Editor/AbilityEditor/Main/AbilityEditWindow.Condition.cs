using System.Collections.Generic;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal sealed partial class AbilityEditWindow
    {
        [SerializeField] private ItemTreeData actorConditionTree = null;

        private void InitInspectorCondition()
        {
            actorConditionTree = ScriptableObject.CreateInstance<ItemTreeData>();
            actorConditionTree.Init(null);
            actorConditionTree.AddManipulator(new ItemTreeConditionManipulator(actorConditionTree));
        }

        public void BuildConditionTree<T>(List<T> list)
        {
            actorConditionTree.Children.Clear();
            using (var itr = list.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    var ac = ScriptableObject.CreateInstance<ItemCondition>();
                    ac.Data = itr.Current as IData;
                    ac.Init(actorConditionTree);
                }
            }
        }

        public void DrawCondition()
        {
            actorConditionTree.HandleManipulatorsEvents(this, Event.current);
            actorConditionTree.Draw();
        }

        public void ClearConditionTree()
        {
            actorConditionTree.RemoveAll();
        }
    }
}