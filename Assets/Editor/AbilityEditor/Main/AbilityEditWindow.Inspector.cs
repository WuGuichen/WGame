using WGame.Editor;

namespace WGame.Ability.Editor
{
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Reflection;
    using System;
    using LitJson;
    using System.IO;
    using System.Text;
    using System.Linq;
    using System.Text.RegularExpressions;
    
    internal sealed partial class AbilityEditWindow
    {
        [System.NonSerialized] public Vector2 leftScrollPos;
        [System.NonSerialized] public Vector2 rightScrollPos;
        
        private int inspectorID = 0;
        private string[] inspectorNames = new string[] { "Ability" };

        private void DrawInspector()
        {
            int id = inspectorID;
            var toolbarRectLeft = new Rect(rectInspector.x, rectInspector.y, rectInspectorLeft.width, timeRulerHeight);
            inspectorID = GUI.Toolbar(toolbarRectLeft, inspectorID, inspectorNames, Setting.appToolbar);
            if (id != inspectorID)
            {
                Repaint();
            }
            GUILayout.BeginArea(rectInspectorLeft);
            {
                switch (inspectorID)
                {
                    case 0:
                        DrawInspectorAbility();
                        break;
                }
            }
            GUILayout.EndArea();
            
            var toolbarRectRight = new Rect(rectInspectorRight.x, toolbarRectLeft.y, rectInspectorRight.width, toolbarRectLeft.height);
            GUI.Box(toolbarRectRight, "Inspector", Setting.appToolbar);
            GUILayout.BeginArea(rectInspectorRight, Setting.propertyBackground);
            {
                var selectEvent = GetActorEvent();
                if (selectEvent != null)
                {
                    selectEvent.DrawInspector();
                }
                else
                {
                    var selectProperty = GetItemData();
                    selectProperty?.DrawInspector();
                }
            }
            GUILayout.EndArea();
        }
        
        public Vector2 MousePos2InspectorPos(Vector2 mousePosition)
        {
            return mousePosition;
        }
        
        private void DeserializeData<T>(string filename, ItemTreeData item) where T : IData, new()
        {
            if (File.Exists(GameAssetsMgr.AbilityDataPath + filename.Replace("/AbilityData/", "")))
            {
                TextAsset text = GameAssetsMgr.Inst.LoadObject(filename, typeof(TextAsset)) as TextAsset;
                if (text == null)
                {
                    WLogger.Error($"the \"{filename}\" of Json is not exist!!!");
                }
                else
                {
                    JsonData jd = JsonMapper.ToObject(text.ToString().Trim());
                    JsonData datas = jd["Data"];
                    for (int i = 0; i < datas.Count; ++i)
                    {
                        DeserializeData(item, typeof(T), datas[i]);
                    }
                }
            }
        }
        
        private ItemData DeserializeData(ItemTreeData parent, System.Type type, JsonData jd)
        {
            var ap = ScriptableObject.CreateInstance<ItemData>();
            ap.Data = System.Activator.CreateInstance(type) as IData;
            ap.Init(parent);
            ap.Deserialize(jd);

            return ap;
        }
        
        private void SerializeData<T>(string path, ItemTreeData actor) where T : IData, new()
        {
            var sb = new StringBuilder();
            var writer = new JsonWriter(sb)
            {
                PrettyPrint = true
            };
            writer.WriteObjectStart();
            writer.WritePropertyName("Data");
            writer.WriteArrayStart();
            using (var itr = actor.Children.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    var actorProperty = itr.Current as ItemData;
                    writer = actorProperty.Serialize(writer);
                }
            }
            writer.WriteArrayEnd();
            writer.WriteObjectEnd();

            // BackupsResource(path);

            var fi = new FileInfo(path);
            using (var sw = fi.CreateText())
            {
                sw.WriteLine(sb.ToString());
                sw.Close();
            }

            AssetDatabase.Refresh();
        }

        private void DeserializeAll()
        {
            InitInspectorAbility();
        }
    }
}