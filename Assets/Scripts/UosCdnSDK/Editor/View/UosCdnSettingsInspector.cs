using UnityEditor;
using UnityEngine;

namespace UosCdn
{
    [CustomEditor(typeof(UOSCdnSettings))]
    public class UosCdnSettingsInspector : Editor
    {
        private SerializedProperty useLatest;
        private SerializedProperty syncWithDelete;

        private void OnEnable()
        {
            useLatest = serializedObject.FindProperty("useLatest");
            syncWithDelete = serializedObject.FindProperty("syncWithDelete");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(useLatest, new GUIContent("use latest badge", "Always set remote load path to latest badge"));
            EditorGUILayout.PropertyField(syncWithDelete, new GUIContent("sync with delete", "When sync entries, deleting remote entries that does not exist in local folder"));

            serializedObject.ApplyModifiedProperties();
        }

    }


}
