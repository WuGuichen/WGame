using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
                    _window._state = new WindowItemState(_window);
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

        private WindowItemState _state;

        public WindowItemState State => _state;

        private WindowSetting _setting;
        public WindowSetting Setting
        {
            get
            {
                if (_setting == null)
                {
                    _setting = new WindowSetting();
                }

                return _setting;
            }
        }
        
        public AbilityData Ability { get; set; }
        
        private static void OnInit()
        {
            
        }
    }
}