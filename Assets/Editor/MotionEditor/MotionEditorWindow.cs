using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using ToolKits;
using UnityEditor;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Motion
{
    class MotionEditorWindow : EditorWindow
    {
        internal TimeArea timeArea;
        public WindowState state;
        public GameObject player;
        public GameObject playerModel;
        public readonly Rect LeftTopSize = new Rect(0, 0, 480, 44);
        public readonly Rect LeftContentHeaderSize = new Rect(0, 44, 480, 40);
        public readonly Rect RightContentHeaderSize = new Rect(500, 20, 800, 24);
        public readonly Rect RightTopSize = new Rect(500, 0, 800, 20);
        readonly List<Manipulator> m_PostManipulators = new List<Manipulator>();

        private TimeAreaItem playHead;
        private int findCfgID = 0;
        private int findCfgIndex = 0;
        private string[] findCfgList;

        private GameObject entityObj;
        private GameEntity entity;
        
        private Contexts _contexts;

        private Contexts contexts
        {
            get
            {
                if(_contexts == null)
                    _contexts = Contexts.sharedInstance;
                return _contexts;
            }
        }

        private static MotionEditorWindow window;

        [MenuItem("Tools/Motion Editor")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            window =
                (MotionEditorWindow)EditorWindow.GetWindow(typeof(MotionEditorWindow), false, "MotionEditor", true);
            window.minSize = new Vector2(800, 600);
            window.Show();
            ResHelper.Init();
        }

        private async void LoadLayout()
        {
            await Task.Delay(100);
            LayoutUtility.DockEditorWindow(window, MotionInspector.Instance);
            await Task.Delay(100);
            LayoutUtility.DockEditorWindow(window, TriggerInspector.Instance);
        }

        private void Init()
        {
            if (state == null)
            {
                state = new WindowState(this);
            }
            if(playHead == null)
            {
                playHead = new TimeAreaItem(TimelineFuncHelper.timeCursor, state, OnTrackHeadDrag, false);
                playHead.drawHead = true;
                playHead.drawLine = true;
            }
            if (timeArea == null)
            {
                timeArea = new TimeArea(false)
                {
                    hRangeLocked = false,
                    vRangeLocked = true,
                    margin = 1,
                    scaleWithWindow = true,
                    hSlider = true,
                    vSlider = true,
                    hBaseRangeMin = 0,
                    hBaseRangeMax = 2,
                    hRangeMin = 0,
                    hRangeMax = 18,
                    // hScaleMax = 10000f,
                    // hScaleMin = 0.1f,
                    rect = TimeContent,
                };
                timeArea.SetShownHRange(0,2.3f);
                m_PostManipulators.Add(new NewEventMenuManipulator(state));
            }

            if (findCfgID < 2000)
            {
                findCfgID = PlayerPrefs.GetInt(LAST_EDIT_KEY, 2000);
            }
        }
        
        public Rect TimeHeaderRect => new Rect(RightContentHeaderSize.x, RightContentHeaderSize.y, position.width - RightContentHeaderSize.x, RightContentHeaderSize.height);

        public Rect TimeTickRect => new Rect(RightContentHeaderSize.x, RightContentHeaderSize.y + RightContentHeaderSize.height, position.width - RightContentHeaderSize.x, position.height - RightContentHeaderSize.height);


        public Rect TimeContent => new Rect(RightContentHeaderSize.x, RightContentHeaderSize.y, position.width - LeftContentHeaderSize.width, position.height - RightContentHeaderSize.y);

        public Rect TimeContentWithOffset => new Rect(RightContentHeaderSize.x - 5, RightContentHeaderSize.y, position.width - LeftContentHeaderSize.width, position.height - RightContentHeaderSize.y);


        public Rect TimeContentWithOutTile => new Rect(RightContentHeaderSize.x, RightContentHeaderSize.y + RightContentHeaderSize.y, position.width - LeftContentHeaderSize.width, position.height - RightContentHeaderSize.y - RightContentHeaderSize.height);

        public int frame = 60;

        void OnGUILeftTop()
        {
            GUILayout.BeginHorizontal();
            // if (GUILayout.Button("加入角色"))
            // {
                if (!player && Application.isPlaying == false)
                {
                    player = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Character/BaseCharacter/BaseCharacter.prefab");
                    player = Instantiate(player);
                    var pos = Vector3.zero;
                    if (Camera.main != null)
                    {
                        var cam = Camera.main.transform;
                        pos = cam.position + cam.forward * 3;
                    }
                    player.transform.position = new Vector3(pos.x, -1.5f, pos.z);
                    player.transform.rotation = UnityEngine.Quaternion.Euler(new Vector3(0, 90, 0));
                    playerModel = player.transform.GetChild(1).gameObject;
                    // Debug.Log("加入Player");
                }
            // }

            if (GUILayout.Button("详细设置"))
            {
                if (PrefabModel.currentConfig)
                {
                    MotionInspector.OpenEditor();
                }
                // LayoutUtility.LoadLayoutFromAsset(LAYOUT_PATH);
            }
            GUILayout.EndHorizontal();
        }

        void OnGUIRightTop()
        {
            GUILayout.BeginHorizontal();
            findCfgList = PrefabModel.GetMotionList();
            findCfgID = (int)EditorGUILayout.IntField(findCfgID, GUILayout.Width(100));
            
            if (GUILayout.Button("用ID加载配置") || PrefabModel.currentConfig == null)
            {
                FindConfig();
            }
            
            var configIndex = EditorGUILayout.Popup(findCfgIndex, findCfgList);
            if (findCfgList.Length > 0)
            {
                var cfgName = findCfgList[configIndex];
                if (cfgName != PrefabModel.currentConfigName)
                {
                    FindConfig(cfgName);
                }
            }
            

            if (PrefabModel.currentConfig)
            {
                if (GUILayout.Button("Save"))
                {
                    Save();
                }
            }
            else
            {
                if (GUILayout.Button("New"))
                {
                    NewConfig();
                }
            }

            if (PrefabModel.currentConfig)
            {
                // GUILayout.Label("当前MotionID: "+PrefabModel.currentConfig.UID.ToString());
            }
            else
            {
                using (new GUIColorOverride(Color.red))
                {
                    GUILayout.Label("请添加或选择一个配置文件");
                }
            }
            GUILayout.EndHorizontal();
        }

        void OnGUILeftContent()
        {
            float offsetHeight = 0;
            state.nodeControls.ForEach(k =>
            {
                k.Draw(new Vector2(10, LeftContentHeaderSize.y + offsetHeight + 10));
                offsetHeight += k.Height + 20;
            });
        }


        private void OnGUI()
        {
            Init();
            GUILayout.BeginArea(LeftTopSize);
            OnGUILeftTop();
            GUILayout.EndArea();
            
            GUILayout.BeginArea(RightTopSize);
            OnGUIRightTop();
            GUILayout.EndArea();
            
            GUILayout.BeginHorizontal();
            timeArea.BeginViewGUI();
            timeArea.TimeRuler(TimeHeaderRect, frame, true, false, 1.0f, TimeArea.TimeFormat.TimeFrame);
            timeArea.DrawMajorTicks(TimeTickRect, frame);
            timeArea.EndViewGUI();
            GUILayout.EndHorizontal();
            
            OnGUILeftContent();
            playHead.Draw(TimeContent, state, state.time);
            OnHandlePlayHead();
            
            var clipRect = new Rect(0.0f, 0.0f, position.width, position.height);
            clipRect.xMin += state.window.TimeHeaderRect.width;
            
            if (state.captured.Count > 0)
            {
                using (new GUIViewportScope(clipRect))
                {
                    foreach (var o in state.captured)
                    {
                        o.Overlay(Event.current, state);
                    }
                    Repaint();
                }
            }

            if (PrefabModel.currentConfig)
            {
                PrefabModel.currentConfig.maxTime = state.maxTime;
            }
        }

        void OnHandlePlayHead()
        {
            playHead.HandleManipulatorsEvents(state);

            for (int i = 0; i < state.nodeControls.Count; i++)
            {
                state.nodeControls[i].HandleManipulatorsEvents(state);
            }

            HandleManipulatorsPostEvents(state);
        }

        private bool HandleManipulatorsPostEvents(WindowState state)
        {
            var isHandled = false;

            foreach (var manipulator in m_PostManipulators)
            {
                isHandled = manipulator.HandleEvent(state);
                if (isHandled)
                    break;
            }

            return isHandled;
        }

        void OnTrackHeadDrag(double newTime)
        {
            state.time = Math.Max(0.0, newTime);
            playHead.showTooltip = true;
            OnEventTimeChange(newTime);
        }

        void OnEventTimeChange(double time)
        {
            state.nodeControls.ForEach(r =>
            {
                if(r.node.active)
                    r.OnEventTimeChange(time);
            });
        }

        void SetEvent(SerializedProperty events, int index, AnimationEvent animationEvent)
        {
            if (events != null && events.isArray)
            {
                if (index < events.arraySize)
                {
                    events.GetArrayElementAtIndex(index).FindPropertyRelative("floatParameter").floatValue = animationEvent.floatParameter;
                    events.GetArrayElementAtIndex(index).FindPropertyRelative("functionName").stringValue = animationEvent.functionName;
                    events.GetArrayElementAtIndex(index).FindPropertyRelative("intParameter").intValue = animationEvent.intParameter;
                    events.GetArrayElementAtIndex(index).FindPropertyRelative("objectReferenceParameter").objectReferenceValue = animationEvent.objectReferenceParameter;
                    events.GetArrayElementAtIndex(index).FindPropertyRelative("data").stringValue = animationEvent.stringParameter;
                    events.GetArrayElementAtIndex(index).FindPropertyRelative("time").floatValue = animationEvent.time;
                }
                else
                {
                    Debug.LogWarning("Invalid Event Index");
                }
            }
        }

        public ModelImporterClipAnimation AssignEvent(ModelImporterClipAnimation animation, IList<AnimationEvent> list)
        {
            animation.events = list.ToArray();
            return animation;
        }

        void SerilizeEvents(ModelImporter modelImporter, IList<AnimationEvent> events, int id)
        {
            SerializedObject serializedObject = new SerializedObject(modelImporter);

            if (modelImporter.clipAnimations == null || modelImporter.clipAnimations.Length == 0)
            {
                modelImporter.clipAnimations = modelImporter.defaultClipAnimations;
            }

            SerializedProperty clipAnimations = serializedObject.FindProperty("m_ClipAnimations");
            if (clipAnimations == null || clipAnimations.arraySize == 0)
            {
                MessageBox(string.Format($" m_ClipAnimations = null，clip:{modelImporter.assetPath} skillID: {id}"));
            }
            else
            {
                SerializedProperty m_clip = clipAnimations.GetArrayElementAtIndex(0);
                SerializedProperty m_events = m_clip.FindPropertyRelative("events");
                if (modelImporter.clipAnimations.Length == 1)
                {
                    m_events.ClearArray();
                    foreach (AnimationEvent evt in events)
                    {
                        m_events.InsertArrayElementAtIndex(m_events.arraySize);
                        SetEvent(m_events, m_events.arraySize - 1, evt);
                    }

                    serializedObject.ApplyModifiedProperties();
                    modelImporter.SaveAndReimport();
                    AssetDatabase.Refresh();
                }
                else
                {
                    MessageBox(string.Format($" clipAnimations != 1，技能ID:{id}"));

                }
            }
        }
        
        public void MessageBox(string msg)
        { 
            EditorUtility.DisplayDialog("Motion Editor", msg, "确认");
            Debug.LogWarning(msg);
        }

        void FindConfig(string findName = null)
        {
            if (!findName.IsNullOrWhitespace())
            {
                var cfg = PrefabModel.GetMotionAssetByName(findName);
                PrefabModel.currentConfigName = findName;
                SwitchConfig(cfg);
                return;
            }
            if (findCfgID < 20000)
            {
                var cfgNames = findCfgList;
                if (cfgNames.Length > 0)
                {
                    var cfgName = cfgNames[0];
                    var cfg = PrefabModel.GetMotionAssetByName(cfgName);
                    PrefabModel.currentConfigName = cfgName;
                    SwitchConfig(cfg);
                }
            }
            else
            {
                for(int i = 0 ; i< findCfgList.Length; i++)
                {
                    var cfgName = findCfgList[i];
                    var cfg = PrefabModel.GetMotionAssetByName(cfgName);
                    if (cfg.UID == findCfgID)
                    {
                        PrefabModel.currentConfigName = cfgName;
                        SwitchConfig(cfg);
                        return;
                    }
                }

                Debug.LogWarning("没有找到配置的ID：" + findCfgID);
                if (findCfgID > 20000 && findCfgID < 30000)
                {
                    NewConfig(findCfgID);
                }
                else
                {
                    findCfgID = PrefabModel.GenerateMotionID();
                    NewConfig(findCfgID);
                    Debug.LogWarning("ID范围仅限：20000 ~ 30000, \n已自动生成可用ID: " + findCfgID);
                }
            }
        }

        void SwitchConfig(EventNodeScriptableObject cfg)
        {
            MotionInspector.Instance.ClearAllData();
            VisualizeHelper.ClearTriggerDescCache();
            if (!cfg)
            {
                if (PrefabModel.currentConfig)
                {
                    PrefabModel.currentConfig = null;
                    RefreshConfig();
                }
            }
            else
            {
                cfg = Instantiate(cfg);
                if (!PrefabModel.currentConfig || PrefabModel.currentConfig.UID != cfg.UID)
                {
                    PrefabModel.currentConfig = cfg;
                    RefreshConfig();
                }
            }

            findCfgID = cfg.UID;
            for (int i = 0; i < findCfgList.Length; i++)
            {
                if (PrefabModel.currentConfigName == findCfgList[i])
                {
                    findCfgIndex = i;
                    break;
                }
            }
            MotionInspector.OpenEditor();
        }

        void RefreshConfig()
        {
            state.nodeControls?.Clear();
            if (!PrefabModel.currentConfig) return;

            PrefabModel.currentConfig.animationNodes?.ForEach(n => state.AddNodeControl(n));
            PrefabModel.currentConfig.triggerAnimationNodes?.ForEach(n => state.AddNodeControl(n));
            PrefabModel.currentConfig.eventNodes?.ForEach(n => state.AddNodeControl(n));
            PrefabModel.currentConfig.eventTriggerNodes?.ForEach(n => state.AddNodeControl(n));
            PrefabModel.currentConfig.conditionTriggerNodes?.ForEach(n => state.AddNodeControl(n));
            PrefabModel.currentConfig.byteCodeCommandNodes?.ForEach(n => state.AddNodeControl(n));
        }

        private string filePath = string.Empty;
        private string fileName = string.Empty;
        private void Save()
        {
            fileName = PrefabModel.currentConfigName;
            // var path = EditorUtility.SaveFilePanel("Save Motion", PrefabModel.motionPath, fileName, "asset");
            // if (string.IsNullOrEmpty(path))
            //     return;
            var nodes = state.nodeControls.Select(arg => arg.node).ToList();
            var obj = CreateInstance<EventNodeScriptableObject>();
            state.SetData(nodes, obj);
            var path = PrefabModel.GetCurrentMotionPath();
            obj.UID = PrefabModel.currentConfig.UID;
            // AssetDatabase.DeleteAsset(p);
            AssetDatabase.CreateAsset(GameObject.Instantiate(obj), path);
            AssetDatabase.Refresh();
            Debug.Log("保存成功：" + path);
            SerializationHelper.SerializeData(SerializationHelper.MotionDataPath, ref obj, PrefabModel.currentConfigName);
        }

        private void NewConfig(int cfgId = 0)
        {
            var newId = 0;
            if (cfgId > 20000 && cfgId < 30000)
                newId = cfgId;
            else
                newId = PrefabModel.GenerateMotionID();
            fileName = "Motion_" + newId.ToString();
            var path = EditorUtility.SaveFilePanel("New Motion", PrefabModel.motionPath, fileName, "asset");
            if (string.IsNullOrEmpty(path))
                return;
            filePath = path;
            var nodes = state.nodeControls.Select(arg => arg.node).ToList();
            var obj = CreateInstance<EventNodeScriptableObject>();
            state.SetData(nodes, obj);
            var p = filePath.Substring(filePath.IndexOf("Assets"));
            obj.UID = newId;
            obj = Instantiate(obj);
            AssetDatabase.CreateAsset(obj, p);
            AssetDatabase.Refresh();
            SwitchConfig(obj);
        }

        private string LAST_EDIT_KEY = "LAST_EDIT_KEY";
        private void OnDestroy()
        {
            PlayerPrefs.SetInt(LAST_EDIT_KEY, PrefabModel.currentConfig.UID);
            PrefabModel.currentConfig = null;
            if (player)
            {
                GameObject.DestroyImmediate(player);
                playerModel = null;
            }

            MotionInspector.Instance.ClearAllData();
            VisualizeHelper.Reset();
        }
    }
}