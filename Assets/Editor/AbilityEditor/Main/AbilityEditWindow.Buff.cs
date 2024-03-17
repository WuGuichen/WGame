using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using WGame.Editor;

namespace WGame.Ability.Editor
{
    internal sealed partial class AbilityEditWindow
    {
        [FormerlySerializedAs("actorBuffTree")] [SerializeField] private ItemTreeData itemBuffTree = null;
        
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

                // if (buff.Buff is CBuffProperty)
                // {
                //     DrawBuffCondition(buff.BuffProperty as CBuffProperty);
                // }
            }
        }
    }
}