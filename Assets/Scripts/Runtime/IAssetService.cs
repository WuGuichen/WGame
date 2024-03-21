using UnityEngine;
using System;
// using Motion;
using Object = UnityEngine.Object;

namespace WGame.Runtime
{
    public interface IAssetService
    {
        public void LoadGameObject(string path, Action<GameObject> callback);
        public void LoadPrefab(string path, Action<GameObject> callback);
        public void ReleaseAssetHandle(string path);
        public void UnloadAsset(Object obj);

        public void LoadTextAssetAsync(string path, Action<TextAsset> callback);
        public void LoadAudioClipAssetAsync(string path, Action<AudioClip> callback);
        public void LoadTextureAsync(string path, Action<Texture> callback);
        public void LoadBytesAsync(string path, Action<byte[]> callback);
        public byte[] LoadBytesSync(string path);
        public void LoadAnimationClipAsync(string path, Action<AnimationClip> callback);
        public void LoadAllObjects(string location ,Action<object> callback, Action onComplete);
        public void LoadAvatarMask(string location, Action<AvatarMask> callback);
        public void LoadRawFileSync(string path, Action<string> callback);
        public void LoadRawFileASync(string path, Action<string> callback);
        public void LoadRawFileSync(string path, Action<byte[]> callback);
        public void LoadRawFileASync(string path, Action<byte[]> callback);

        public void LoadSceneAsync(string name, Action callback);
        // public void LoadMotionScriptSync(string path, Action<EventNodeScriptableObject> callback);
        // public void LoadMotionScriptAsync(string path, Action<EventNodeScriptableObject> callback);
    }
}