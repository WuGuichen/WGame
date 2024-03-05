using UnityEditor;
using UnityEngine;
using WGame.Editor;

namespace WGame.Ability.Editor
{
    internal sealed partial class AbilityEditWindow
    {
        [SerializeField] private ItemTreeData itemAbilityTree = null;
        
        private void InitInspectorAbility()
        {
            itemAbilityTree = ScriptableObject.CreateInstance<ItemTreeData>();
            itemAbilityTree.Init(null);
            itemAbilityTree.AddManipulator(new ItemTreeDataManipulator(itemAbilityTree));
        }

        private void DrawInspectorAbility()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Space(2);
                GUILayout.BeginHorizontal(Setting.btnToolBoxStyle, GUILayout.Width(rectInspectorLeft.width),
                    GUILayout.Height(16));
                {
                    if (GUILayout.Button("New"))
                    {
                        var ap = CreateData(itemAbilityTree, typeof(AbilityData));
                        var ac = ap.Data as AbilityData;
                        ac.TotalTime = 1666;
                        ac.ID = Helper.NonceStr(25);
                    }

                    if (GUILayout.Button("Delete"))
                    {
                        DeleteProperty();
                        ClearTreeView();
                    }

                    if (GUILayout.Button("Save"))
                    {
                        SaveAbility();
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
                leftScrollPos = GUILayout.BeginScrollView(leftScrollPos, false, true);
                {
                    itemAbilityTree.HandleManipulatorsEvents(this, Event.current);
                    itemAbilityTree.Draw();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }

        private void SaveAbility()
        {
            var path = $"{GameAssetsMgr.AbilityDataPath}/TestGroup.json";
            SerializeData<AbilityData>(path, itemAbilityTree);
            
            EditorUtility.DisplayDialog("INFO", "Succeed to save!", "OK");
        }

        public void DeserializeAbility()
        {
            itemAbilityTree.RemoveAll();
            var groupName = "defaultGroup";
            var fileName = "/Res/Ability/" + groupName + ".json";
            
            DeserializeData<AbilityData>(fileName, itemAbilityTree);
        }
        
        private ItemData CreateData(ItemTreeData parent, System.Type type)
        {
            var ap = ScriptableObject.CreateInstance<ItemData>();
            ap.Data = System.Activator.CreateInstance(type) as IData;

            var operation = $"{type} {parent.Children.Count}";
            Helper.RegisterCreatedObjectUndo(ap, operation);
            Helper.PushUndo(new UnityEngine.Object[] { ap, parent }, operation);

            ap.Init(parent);

            return ap;
        }

        private void DeleteProperty()
        {
            var selectable = GetItemData();
            if (selectable != null)
            {
                string title = $"Delete {selectable.GetDataType()}";
                if (EditorUtility.DisplayDialog(title, "Are you sure?", "YES", "NO!"))
                {
                    var parent = selectable.Parent;

                    Helper.PushUndo(new UnityEngine.Object[] { parent, selectable }, Setting.contextDelProperty.text);
                    Helper.PushDestroyUndo(parent, selectable);

                    parent.RemoveChild(selectable);
                    DeselectAllData();
                }
            }
        }

    }
}