using System;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace UosCdn
{
    public class UploadWindow : EditorWindow
    {
        public static float perCentage = 0;
        public static long totalDisplay = 0;
        public static long alreadyDisplay = 0;
        public static int totalCounts = 20;
        public static int uploadedCounts = 10;

        public static void uploadWindow()
        {
            UploadWindow window = EditorWindow.GetWindow<UploadWindow>(false, "Upload Content");
            window.maxSize = new Vector2(215f, 110f);
            window.minSize = window.maxSize;
            window.Show();
        }
        
        void OnGUI()
        {
            EditorGUI.LabelField(new Rect(3, 3, position.width - 6, 20), "Uploaded Files: ", string.Format("{0} / {1}", ParametersUpload.alreadyUploadFiles, ParametersUpload.totalUploadFiles));
            EditorGUI.LabelField(new Rect(3, 25, position.width - 6, 20), "Uploaded Size: ", string.Format("{0} MB / {1} MB", ParametersUpload.alreadyUploadSize / 1048576.00f, ParametersUpload.totalUploadSize / 1048576.00f));
            EditorGUI.ProgressBar(new Rect(3, 45, position.width - 6, 20), ParametersUpload.totalUploadSize4Current == 0 ? 0 : (float)ParametersUpload.alreadyUploadSize4Current / ParametersUpload.totalUploadSize4Current,
                string.Format("{0} MB / {1} MB", ParametersUpload.alreadyUploadSize4Current / 1048576.00f, ParametersUpload.totalUploadSize4Current / 1048576.00f));
        }

        void OnEnable()
        {

        }

        private void Update()
        {
            
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }

   
}