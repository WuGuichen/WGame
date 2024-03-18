using LitJson;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal class ItemCondition : ItemTreeData
    {
        public IData Data = null;
        
        public override void Draw()
        {
            var selected = Window.HasSelectCondition(this);
            GUILayout.BeginHorizontal();
            {
                var offset = selected ? 50f : 20f;
                var rc = GUILayoutUtility.GetRect(Window.rectInspectorRight.width - offset, Window.propertyHeight);
                if (rc.height == Window.propertyHeight)
                {
                    rect = rc;
                }

                if (GUI.Button(rc, Data.GetType().Name))
                {

                }
                if (selected)
                {
                    Window.DrawSelectable(Window.Setting.colorRed);
                }
            }
            GUILayout.EndHorizontal();

            if (selected)
            {
                GUILayout.Space(3);
                Window.DrawData(Data);
                GUILayout.Space(3);
            }
        }

        public override string GetDataType()
        {
            return Data.GetType().Name;
        }
        
        public void Deserialize(JsonData jd)
        {
            Data.Deserialize(jd);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            return Data.Serialize(writer);
        }
    }
    
}