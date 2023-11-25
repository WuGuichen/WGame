using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class TriggerInspector : OdinEditorWindow
{
    private static TriggerInspector _instance;

    public static TriggerInspector Instance
    {
        get
        {
            if(!_instance)
                PopUp();
            return _instance;
        }
    }
    public static void PopUp()
    {
        _instance = EditorWindow.GetWindow<TriggerInspector>();
        _instance.Show();
    }
}
