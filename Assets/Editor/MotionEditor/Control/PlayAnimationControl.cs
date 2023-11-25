using UnityEditor;
using UnityEngine;

namespace Motion
{
    class PlayAnimationControl : NodeControl<PlayAnimationNode>
    {
        protected TimeAreaItem endTime;
        private AnimationClip clip;

        private readonly string[] animLayerNames;
        private readonly int[] animLayers;
        
        public PlayAnimationControl(WindowState state) : base(state)
        {
            endTime = new TimeAreaItem(TimelineFuncHelper.timeCursor, state, null, true);
            endTime.drawHead = false;
            endTime.drawLine = true;
            endTime.lineColor = color;
            endTime.headColor = color;
            var t = typeof(AnimLayerType);
            var infos = t.GetFields();
            animLayerNames = new string[infos.Length];
            animLayers = new int[infos.Length];
            for (int i = 0; i < infos.Length; i++)
            {
                animLayerNames[i] = infos[i].Name;
                animLayers[i] = (int)(infos[i].GetRawConstantValue());
            }
        }

        public override Color color => Color.green * 0.5f;

        public override void OnLeftGUI()
        {
            GUILayout.BeginArea(leftSize);
            EditorGUILayout.BeginHorizontal();
            
            node.active = GUILayout.Toggle(node.active, "开启");
            GUILayout.Label("开始播放", GUILayout.Width(55));
            Node.playTime = GUILayout.HorizontalSlider(Node.playTime, 0, Node.duration,GUILayout.Width(80));
            GUILayout.Label(Node.playTime.ToString("f2"), GUILayout.Width(40));
            Node.playTransTime = GUILayout.HorizontalSlider(Node.playTransTime, 0, Node.duration - Node.playTime, GUILayout.Width(80));
            GUILayout.Label(Node.playTransTime.ToString("f2"), GUILayout.Width(40));
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Label(Node.Name, GUILayout.MinWidth(80));
            GUILayout.Label(Node.AnimClipID.ToString());
            var tmp = EditorGUILayout.IntPopup("", Node.AnimClipID, ResHelper.clipNameList, ResHelper.clipList, GUILayout.Width(120));
            if (tmp != Node.AnimClipID)
                clip = null;
            Node.AnimClipID = tmp;
            if (Node.AnimClipID != 0)
            {
                if(!clip)
                    clip = ResHelper.LoadAnimClip(Node.AnimClipID);
            }
            else
                clip = null;

            if (clip)
            {
                Node.duration = clip.length;
            }
            Node.playLayer = EditorGUILayout.IntPopup("", Node.playLayer, animLayerNames, animLayers, GUILayout.Width(120));
            GUILayout.Label("重置层级", GUILayout.Width(60));
            Node.isResetBaseLayer = EditorGUILayout.Toggle("",Node.isResetBaseLayer, GUILayout.Width(20));
            
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        public override void OnRightGUI()
        {
            var transWidth = state.TimeToPixel(Node.playTransTime + Node.playTime) - state.TimeToPixel(Node.playTime);
            if (transWidth <= 0.1f) transWidth = 1f;
            var clipRect = new Rect(0.0f, 0.0f, state.window.RightContentHeaderSize.width + 2000f, state.window.position.height);
            clipRect.xMin += state.window.RightContentHeaderSize.x;
            using (new GUIViewportScope(clipRect))
            {
                EditorGUI.DrawRect(new Rect(state.TimeToPixel(node.time + Node.playTime), rightSize.y, transWidth, rightSize.height), Color.red * 0.3f);
            }
            endTime.Draw(rightSize, state, node.time + Node.duration);
            DrawTimeRect(node.time, node.time + Node.duration, color);
            // GUIUtility.Dr
        }

        public override void OnEventTimeChange(double time)
        {
            if (time > Node.time + Node.playTime && time < (Node.duration + Node.time))
            {
                if (clip && state.window.player)
                {
                    var tmpPos = state.window.playerModel.transform.position;
                    clip.SampleAnimation(state.window.playerModel, ((float)time - Node.time));
                    state.window.playerModel.transform.position = tmpPos;
                }
            }
        }
    }
}