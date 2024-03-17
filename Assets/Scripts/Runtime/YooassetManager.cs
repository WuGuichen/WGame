using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HybridCLR;
// using Motion;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace WGame.Runtime
{
    public class YooassetManager : MonoBehaviour, IAssetService
    {
        private static YooassetManager _inst;
        public static YooassetManager Inst => _inst;

        private Dictionary<string, AssetHandle> handles = new Dictionary<string, AssetHandle>();
        private Dictionary<int, string> locations = new Dictionary<int, string>();

        private ResourcePackage package;
        private ResourcePackage rawFilePackage;

        #if UNITY_EDITOR
        [SerializeField]
        private EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
        #else
        private EPlayMode PlayMode = EPlayMode.HostPlayMode;
        #endif

        public bool IsInitted { get; private set; } = false;

        private void Awake()
        {
            _inst = this;
            YooAssets.Initialize();
            StartCoroutine(LoadPackages());
        }

        IEnumerator LoadPackages()
        {
            var defaultOperation = new PatchOperation("DefaultPackage", EDefaultBuildPipeline.BuiltinBuildPipeline.ToString(), PlayMode, PatchOperation.PackageType.Default);
            var rawFileOperation = new PatchOperation("RawFilePackage", EDefaultBuildPipeline.RawFileBuildPipeline.ToString(), PlayMode, PatchOperation.PackageType.RawFile);
            YooAssets.StartOperation(rawFileOperation);
            yield return rawFileOperation;
            rawFilePackage = YooAssets.GetPackage("RawFilePackage");
            YooAssets.StartOperation(defaultOperation);
            yield return defaultOperation;
            package = YooAssets.GetPackage("DefaultPackage");
            YooAssets.SetDefaultPackage(package);
            IsInitted = true;
            var hotDllList = HotUpdateList.HotList;
            
            foreach (var dllName in hotDllList)
            {
                var hotDll = LoadBytesSync(dllName);
                Assembly.Load(hotDll);
            }

            foreach (var dllName in AOTGenericReferences.PatchedAOTAssemblyList)
            {
                LoadBytesAsync(dllName, bytes =>
                {
                    var err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(bytes, HomologousImageMode.SuperSet);
                    Debug.Log($"LoadMetadataForAOTAssembly:{dllName}. ret:{err}");
                });
            }

            EventCenter.Trigger(EventDefine.OnGameAssetsManagerInitted);
        }

        public void LoadGameObject(string path, Action<GameObject> callback)
        {
            if (handles.ContainsKey(path))
            {
                callback.Invoke(handles[path].InstantiateSync());
                return;
            }
            StartCoroutine(ProcessLoadGameObject(path, callback));
        }

        public void ReleaseAssetHandle(string path)
        {
            if (handles.ContainsKey(path))
            {
                handles[path].Release();
                handles.Remove(path);
                Debug.Log($"Release handle path: {path}");
            }
        }

        public void UnloadAsset(Object obj)
        {
            if (locations.TryGetValue(obj.GetInstanceID(), out string location))
            {
                locations.Remove(obj.GetInstanceID());
                ReleaseAssetHandle(location);
            }
        }

        public void LoadTextAssetAsync(string path, Action<TextAsset> callback)
        {
            var handle = package.LoadAssetAsync<TextAsset>(path);
            handle.Completed += operationHandle =>
            {
                var text = handle.AssetObject as TextAsset;
                callback.Invoke(text);
                handle.Release();
            };
        }

        public string LoadTextAssetSync(string path)
        {
            var handle = package.LoadAssetSync<TextAsset>(path);
            var textAsset = handle.AssetObject as TextAsset;
            var res = textAsset.text;
            handle.Release();
            return res;
        }

        public void LoadAudioClipAssetAsync(string path, Action<AudioClip> callback)
        {
            if (handles.ContainsKey(path))
            {
                callback.Invoke((AudioClip)(handles[path].AssetObject));
                return;
            }
            var handle = package.LoadAssetAsync<AudioClip>(path);
            handle.Completed += operationHandle =>
            {
                if (!handles.ContainsKey(path))
                {
                    callback.Invoke((AudioClip)(operationHandle.AssetObject));
                    handles.Add(path, handle);
                    locations.Add(handle.AssetObject.GetInstanceID(), path);
                }
                else
                {
                    callback.Invoke((AudioClip)(handles[path].AssetObject));
                    handle.Release();
                }
            };
        }

        public void LoadTextureAsync(string path, Action<Texture> callback)
        {
            if (handles.ContainsKey(path))
            {
                callback.Invoke((Texture)(handles[path].AssetObject));
                return;
            }

            var handle = package.LoadAssetAsync<Texture>(path);
            handle.Completed += operationHandle =>
            {
                if (!handles.ContainsKey(path))
                {
                    callback.Invoke((Texture)(operationHandle.AssetObject));
                    handles.Add(path, handle);
                    locations.Add(handle.AssetObject.GetInstanceID(), path);
                }
                else
                {
                    callback.Invoke((Texture)(handles[path].AssetObject));
                    handle.Release();
                }
            };
        }

        public void LoadBytesAsync(string path, Action<byte[]> callback)
        {
            var handle = package.LoadAssetAsync<TextAsset>(path);
            handle.Completed += operationHandle =>
            {
                var text = handle.AssetObject as TextAsset;
                callback.Invoke(text.bytes);
                handle.Release();
            };
        }

        public byte[] LoadBytesSync(string path)
        {
            if (handles.ContainsKey(path))
            {
                return (handles[path].AssetObject as TextAsset).bytes;
            }
            var handle = package.LoadAssetSync<TextAsset>(path);
            var text = handle.AssetObject as TextAsset;
            handles.Add(path, handle);
            locations.Add(handle.AssetObject.GetInstanceID(), path);
            return text.bytes;
        }

        public void LoadAnimationClipAsync(string path, Action<AnimationClip> callback)
        {
            if (handles.ContainsKey(path))
            {
                callback.Invoke(handles[path].AssetObject as AnimationClip);
                return;
            }
            var handle = package.LoadAssetAsync<AnimationClip>(path);
            handle.Completed += operationHandle =>
            {
                if (handles.ContainsKey(path))
                {
                    callback.Invoke(handles[path].AssetObject as AnimationClip);
                    handle.Release();
                }
                else
                {
                    callback.Invoke(operationHandle.AssetObject as AnimationClip);
                    handles.Add(path, handle);
                    locations.Add(handle.AssetObject.GetInstanceID(), path);
                }
            };
        }

        IEnumerator ProcessLoadGameObject(string path, Action<GameObject> callback)
        {
            AssetHandle handle = package.LoadAssetAsync<GameObject>(path);
            yield return handle;
            GameObject go = handle.InstantiateSync();
            callback.Invoke(go);
            if (handles.TryGetValue(path, out var oldHandle))
            {
                oldHandle.Release();
                handles.Remove(path);
            }
            handles.Add(path, handle);
        }

        public void LoadAllObjects(string location, Action<object> callback, Action onComplete)
        {
            StartCoroutine(LoadAllObjectsInternal(location, callback, onComplete));
        }

        public void LoadAvatarMask(string location, Action<AvatarMask> callback)
        {
            var handle = package.LoadAssetAsync(location);
            handle.Completed += operationHandle =>
            {
                callback.Invoke(operationHandle.AssetObject as AvatarMask);
                handle.Release();
            };
        }

        public void LoadRawFileSync(string path, Action<string> callback)
        {
            var handle = rawFilePackage.LoadRawFileSync(path);
            handle.Completed += operationHandle =>
            {
                callback.Invoke(operationHandle.GetRawFileText());
                handle.Release();
            };
        }

        public void LoadRawFileASync(string path, Action<string> callback)
        {
            var handle = rawFilePackage.LoadRawFileAsync(path);
            handle.Completed += operationHandle =>
            {
                callback.Invoke(operationHandle.GetRawFileText());
                handle.Release();
            };
        }

        public void LoadRawFileSync(string path, Action<byte[]> callback)
        {
            var handle = rawFilePackage.LoadRawFileSync(path);
            handle.Completed += operationHandle =>
            {
                callback.Invoke(operationHandle.GetRawFileData());
                handle.Release();
            };
        }

        public void LoadRawFileASync(string path, Action<byte[]> callback)
        {
            var handle = rawFilePackage.LoadRawFileAsync(path);
            handle.Completed += operationHandle =>
            {
                callback.Invoke(operationHandle.GetRawFileData());
                handle.Release();
            };
        }

        public void LoadSceneAsync(string name, Action callback)
        {
            var handle = package.LoadSceneAsync(name, LoadSceneMode.Single);
            handle.Completed += operationHandle =>
            {
                EventCenter.Trigger(EventDefine.OnSceneLoaded, WEventContext.Get(name));
                callback?.Invoke();
            };
        }

        // public void LoadMotionScriptSync(string path, Action<EventNodeScriptableObject> callback)
        // {
        //     var handle = rawFilePackage.LoadRawFileSync(path);
        //     handle.Completed += node =>
        //     {
        //         SerializationHelper.DeserializeValue<EventNodeScriptableObject>(node.GetRawFileData(), out var scriptable);
        //         callback.Invoke(scriptable);
        //         handle.Release();
        //     };
        // }

        // public void LoadMotionScriptAsync(string path, Action<EventNodeScriptableObject> callback)
        // {
        //     var handle = rawFilePackage.LoadRawFileAsync(path);
        //     handle.Completed += node =>
        //     {
        //         SerializationHelper.DeserializeValue<EventNodeScriptableObject>(node.GetRawFileData(), out var scriptable);
        //         callback.Invoke(scriptable);
        //         handle.Release();
        //     };
        // }

        IEnumerator LoadAllObjectsInternal(string location, Action<object> callback, Action onComplete)
        {
            AllAssetsHandle handle = package.LoadAllAssetsAsync<ScriptableObject>(location);
            yield return handle;
            foreach (var assetObj in handle.AllAssetObjects)
            {
                callback.Invoke(assetObj);
            }
            onComplete?.Invoke();
            handle.Release();
        }
    }
}