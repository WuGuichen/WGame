using System.IO;
using UnityEditor;
using UnityEngine;
using WGame.Editor;

namespace WGame.Ability.Editor
{
    internal sealed partial class AbilityEditWindow : EditorWindow
    {
        private static AbilityEditWindow _window = null;

        public static AbilityEditWindow Window
        {
            get
            {
                if (_window == null)
                {
                    _window = GetWindow<AbilityEditWindow>();
                }

                return _window;
            }
        }

        [MenuItem("Tools/AbilityEditWindow")]
        private static void Main()
        {
            OnInit();
            Window.titleContent = new GUIContent("WGame Ability Editor");
            Window.Show();
            Window.Focus();
        }

        private WindowSetting _setting;
        public WindowSetting Setting
        {
            get
            {
                if (_setting == null)
                {
                    string path = Path.Combine(editorResourcePath, "AbilityWindowSetting.asset");
                    _setting = AssetDatabase.LoadAssetAtPath<WindowSetting>(path);
                    _setting.Init();
                }

                return _setting;
            }
        }
        
        public AbilityData Ability { get; private set; }
        
        private static void OnInit()
        {
            GameAssetsMgr.Inst.InitInstance();
        }
        
        private string _editorResourcePath = null;
        public string editorResourcePath
        {
            get
            {
                if (string.IsNullOrEmpty(_editorResourcePath))
                {
                    string root = Helper.GetRootDirectory();
                    _editorResourcePath = Path.Combine(root, "Resource");
                }
                return _editorResourcePath;
            }
        }
    }
}