using UnityEngine;
using UnityEditor;

namespace UosCdn
{
    public class PreivewRemoteLoadPath : PopupWindowContent
    {
        public string badgeName = "";

        public override Vector2 GetWindowSize()
        {
            return new Vector2(300, 100);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            badgeName = EditorGUILayout.TextField("Badge Name", badgeName);
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            /*
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Preview Remote Load Path"))
            {
                string remoteLoadPath = AddressableConfigController.GetRemoteLoadPath(badgeName);
                EditorUtility.DisplayDialog("Remote Load Path", "URL: " + remoteLoadPath, "OK");
            }
            GUILayout.EndHorizontal();
            */

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set"))
            {
                // AddressableConfigController.SetRemoteLoadPath(badgeName);
                string remoteLoadPath = AddressableConfigController.GetRemoteLoadPath(badgeName);
                EditorUtility.DisplayDialog("Remote Load Path", "URL: " + remoteLoadPath, "OK");
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