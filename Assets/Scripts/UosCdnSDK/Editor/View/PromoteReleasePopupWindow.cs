using UnityEngine;
using UnityEditor;

namespace UosCdn
{
    public class PromoteReleasePopup : PopupWindowContent
    {
        public string bucketUuid = "";
        public string notes = "";

        public override Vector2 GetWindowSize()
        {
            return new Vector2(300, 100);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            bucketUuid = EditorGUILayout.TextField("Target Bucket ID", bucketUuid);
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            notes = EditorGUILayout.TextField("Notes", notes);
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Promote"))
            {
                ReleaseController.PromoteRelease(bucketUuid, notes);
                editorWindow.Close();
            }
            GUILayout.EndHorizontal();
        }
   
        public override void OnOpen()
        {
            // Debug.Log("Popup opened: " + this);
        }

        public override void OnClose()
        {
            // Debug.Log("Popup closed: " + this);
        }
    }
}