using UnityEngine;
using UnityEditor;

namespace UosCdn
{
    public class CreateBucketPopup : PopupWindowContent
    {
        public string bucketName = "";
        public string bucketDescription = "";

        public override Vector2 GetWindowSize()
        {
            return new Vector2(300, 100);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            bucketName = EditorGUILayout.TextField("Bucket Name", bucketName);
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            bucketDescription = EditorGUILayout.TextField("Bucket Description", bucketDescription);
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create"))
            {
                BucketController.CreateBucket(bucketName, bucketDescription);
                BucketController.LoadBuckets();
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