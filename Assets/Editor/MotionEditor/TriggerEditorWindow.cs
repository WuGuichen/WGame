using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;

public class TriggerEditorWindow : OdinEditorWindow
{
    public string triggerName;

    public List<string> triggerList;

    [MenuItem("Tools/Trigger Editor")]
    private static void OpenWindow()
    {
        var window = GetWindow<TriggerEditorWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
    }
}
