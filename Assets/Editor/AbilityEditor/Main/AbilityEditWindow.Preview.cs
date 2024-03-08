using UnityEditor;
using UnityEngine;

namespace WGame.Ability.Editor
{
    internal sealed partial class AbilityEditWindow
    {
        private float lastTime = 0f;
        private float deltaTime = 1f / 60;

        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if (!Application.isPlaying && _playState == PlayState.Play)
            {
                Preview(Time.realtimeSinceStartup - lastTime);
                Repaint();
            }

            lastTime = Time.realtimeSinceStartup;
        }

        private void Preview(float fTick)
        {
            fTick *= _playSpeed;
            
            Tick(fTick);

            CurrentTime += fTick;

            if (CurrentTime >= Length)
            {
                CurrentTime = 0f;
                _playState = PlayState.Stop;
                ResetPreview(fTick);
            }
        }

        private void Tick(float fTick)
        {
            if (Ability == null)
            {
                return;
            }
        }

        private void ResetPreview(float fTick)
        {
            
        }
    }
}