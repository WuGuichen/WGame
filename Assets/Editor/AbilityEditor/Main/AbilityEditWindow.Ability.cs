using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WGame.Editor;

namespace WGame.Ability.Editor
{
    internal sealed partial class AbilityEditWindow
    {
        [SerializeField] private ItemTreeData itemAbilityTree = null;
        [SerializeField] private Searcher searcher = new Searcher();
        
        private void InitInspectorAbility()
        {
            itemAbilityTree = ScriptableObject.CreateInstance<ItemTreeData>();
            itemAbilityTree.Init(null);
            itemAbilityTree.AddManipulator(new ItemTreeDataManipulator(itemAbilityTree));
            DeserializeAbility();
            
            searcher.onInputChanged = OnInputChanged;
            searcher.onConfirm = OnConfirm;
        }
        
        private void OnInputChanged(string searchString)
        {
            searcher.ClearResults();
            if (!string.IsNullOrEmpty(searchString))
            {
                foreach (var item in itemAbilityTree.Children)
                {
                    var ap = item as ItemData;
                    var ac = ap.Data as AbilityData;
                    if (!string.IsNullOrEmpty(ac.Name) && ac.Name.Contains(searchString))
                    {
                        searcher.AddResult(ac.Name);
                    }
                }
            }
        }
        
        private void OnConfirm(string result)
        {
            foreach (var item in itemAbilityTree.Children)
            {
                var ap = item as ItemData;
                var ac = ap.Data as AbilityData;
                if (ac.Name == result)
                {
                    item.OnSelected();
                    SelectData(item);

                    leftScrollPos.y = item.manipulatorRect.y;

                    break;
                }
            }
        }

        public List<KeyValuePair<string, int>> AbilityList()
        {
            var res = new List<KeyValuePair<string, int>>();
            using (var itr = itemAbilityTree.Children.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    var ap = itr.Current as ItemData;
                    var ac = ap.Data as AbilityData;
                    res.Add(new KeyValuePair<string, int>(ac.Name, ac.ID));
                }
            }

            return res;
        }

        public HashSet<int> AbilityIDSet()
        {
            HashSet<int> list = new();
            using (var itr = itemAbilityTree.Children.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    var ap = itr.Current as ItemData;
                    var ac = ap.Data as AbilityData;
                    list.Add(ac.ID);
                }
            }

            return list;
        }

        public int GenEmptyAbilityID()
        {
            var idSet = AbilityIDSet();
            for (int i = 0; i < 10000; i++)
            {
                if (!idSet.Contains(i))
                {
                    return i;
                }
            }

            return -1;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void DrawInspectorAbility()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Space(2);
                
                searcher.OnGUI();

                if (!searcher.HasSearchbarFocused())
                {
                    GUILayout.BeginHorizontal(Setting.btnToolBoxStyle, GUILayout.Width(rectInspectorLeft.width),
                        GUILayout.Height(16));
                    {
                        if (Application.isPlaying)
                        {
                            if (GUILayout.Button("Hot Reload"))
                            {
                                WAbilityMgr.Inst.HotReloadGroup(WAbilityMgr.Inst.Loader.GetAbilityGroups()[0]);
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("New"))
                            {
                                var ap = CreateData(itemAbilityTree, typeof(AbilityData));
                                var ac = ap.Data as AbilityData;
                                ac.TotalTime = 1666;
                                ac.ID = GenEmptyAbilityID();
                                ac.Name = Helper.NonceStr();
                            }

                            if (GUILayout.Button("Delete"))
                            {
                                DeleteProperty();
                                ClearTreeView();
                            }
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
            var groupName = "TestGroup";
            var fileName = "/AbilityData/" + groupName + ".json";
            
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