using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WGame.Editor;

namespace WGame.Ability.Editor
{
    internal sealed partial class AbilityEditWindow
    {
        [SerializeField] private ItemTreeData itemBuffTree = null;
        [System.NonSerialized] private ConditionType buffConditionType = ConditionType.None;
        
        private void InitInspectorBuff()
        {
            itemBuffTree = ScriptableObject.CreateInstance<ItemTreeData>();
            itemBuffTree.Init(null);
            itemBuffTree.AddManipulator(new ItemTreeDataManipulator(itemBuffTree));
        }

        private void DrawInspectorBuff()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Space(2);
                GUILayout.BeginHorizontal(Setting.btnToolBoxStyle, GUILayout.Width(rectInspectorLeft.width), GUILayout.Height(16));
                {
                    if (GUILayout.Button("New"))
                    {
                        CreateData(itemBuffTree, typeof(BuffFactoryData));
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        DeleteProperty();
                    }
                    if (GUILayout.Button("Save"))
                    {
                        SaveBuff();
                    }
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(2);

                leftScrollPos = GUILayout.BeginScrollView(leftScrollPos, false, true);
                {
                    itemBuffTree.HandleManipulatorsEvents(this, Event.current);
                    itemBuffTree.Draw();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }
        
        private void SaveBuff()
        {
            SerializeData<BuffFactoryData>(GameAssetsMgr.AbilityDataPath + "Buff/Buff.json", itemBuffTree);

            EditorUtility.DisplayDialog("INFO", "Succeed to save!", "OK");
        }
        
        public void DrawBuff(BuffFactoryData buff)
        {
            GUILayout.Space(5);
            using (new GUIColorScope(Setting.colorInspectorLabel))
            {
                EditorGUILayout.LabelField("BUFF数据");
            }

            GUILayout.Space(2);
            if (buff.Buff != null)
            {
                DrawData(buff.Buff);

                if (buff.Buff is CBuffData)
                {
                    DrawBuffCondition(buff.Buff as CBuffData);
                }
            }
        }
        
        private void DrawBuffCondition(CBuffData buff)
        {
            GUILayout.Space(5);
            using (new GUIColorScope(Setting.colorInspectorLabel))
            {
                EditorGUILayout.LabelField("BUFF条件编辑");
            }

            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            {
                buffConditionType = (ConditionType)EditorGUILayout.EnumPopup(buffConditionType);
                if (GUILayout.Button("New Cond"))
                {
                    NewBuffCondition(buff);
                    BuildConditionTree(buff.ConditionList);
                }
                if (GUILayout.Button("Del Cond"))
                {
                    DelBuffCondition(buff);
                    BuildConditionTree(buff.ConditionList);
                }
                if (GUILayout.Button("Del ALL"))
                {
                    DelAllBuffCondition(buff);
                    BuildConditionTree(buff.ConditionList);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            DrawCondition();
        }
        
        private void NewBuffCondition(CBuffData buff)
        {
            if (buffConditionType == ConditionType.None || buffConditionType == ConditionType.MAX)
            {
                EditorUtility.DisplayDialog("INFO", "Please select BUFF condition type.", "OK");
            }
            else
            {
                buff.Add(buffConditionType);
            }
        }

        private void DelBuffCondition(CBuffData buff)
        {
            var selectable = GetActorCondition();
            if (selectable != null)
            {
                buff.Remove(selectable.Data as ICondition);
                DeselectAllCondition();
            }
        }

        private void DelAllBuffCondition(CBuffData buff)
        {
            buff.RemoveAll();
        }

        public List<KeyValuePair<string, int>> BuffList()
        {
            var res = new List<KeyValuePair<string, int>>();
            using (var itr = itemBuffTree.Children.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    var ap = itr.Current as ItemData;
                    var ac = ap.Data as BuffData;
                    res.Add(new KeyValuePair<string, int>(ac.Name, ac.ID));
                }
            }

            return res;
        }
        
        public HashSet<int> BuffIDSet()
        {
            HashSet<int> list = new();
            using (var itr = itemBuffTree.Children.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    var ap = itr.Current as ItemData;
                    var ac = ap.Data as BuffData;
                    list.Add(ac.ID);
                }
            }

            return list;
        }
        
        public int GenEmptyBuffID()
        {
            var idSet = BuffIDSet();
            for (int i = 0; i < 10000; i++)
            {
                if (!idSet.Contains(i))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}