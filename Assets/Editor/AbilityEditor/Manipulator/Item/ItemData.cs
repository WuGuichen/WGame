namespace WGame.Ability.Editor
{
    using LitJson;
    using UnityEngine;
    
    [System.Serializable]
    internal class ItemData : ItemTreeData
    {
        [SerializeReference] public IData Data = null;

        public override void OnSelected()
        {
            if (Data is AbilityData ability)
            {
                Window.ClearTreeView();
                Window.BuildTreeView(ability);
                
                Window.ResetTimelineView();
            }
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal();
            {
                var selected = Window.HasSelectData(this);
                var offset = selected ? 50f : 20f;
                var rc = GUILayoutUtility.GetRect(Window.rectInspectorLeft.width - offset, Window.propertyHeight);
                if (rc.height == Window.propertyHeight)
                {
                    rect = rc;
                }
                
                string btnName = string.IsNullOrEmpty(Data.DebugName) ? "anonymity" : Data.DebugName;
                if (GUI.Button(rc, btnName))
                {
                    WLogger.Print("未定义");
                }
                if (selected)
                {
                    // Window.DrawSelectable(window.editorResources.colorRed);
                }
            }
            GUILayout.EndHorizontal();
        }

        public override void DrawInspector()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Space(2);
                GUILayout.Label("未定义");
            }
            GUILayout.EndVertical();
        }

        public override string GetItemType()
        {
            return Data.GetType().ToString();
        }
        
        
        public void Deserialize(JsonData jd)
        {
            Data.Deserialize(jd);
            if (Data is AbilityData ac)
            {
                ac.ForceSort();
            }
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            return Data.Serialize(writer);
        }
    }
}