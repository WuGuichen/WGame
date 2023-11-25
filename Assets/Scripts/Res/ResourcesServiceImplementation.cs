// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;
//
// public class ResourcesServiceImplementation : IResourcesService
// {
//     private Dictionary<object, int> _referenceCount = new Dictionary<object, int>();
//     private Dictionary<object, AsyncOperationHandle> _handleMap = new Dictionary<object, AsyncOperationHandle>();
//     private HashSet<AsyncOperationHandle> _loadingHandles = new HashSet<AsyncOperationHandle>();
//
//     public ResourcesServiceImplementation()
//     {
//         
//     }
//
//     // 方法首先检查资源是否在。如果是这样，它将增加引用计数并立即使用缓存的资源调用回调。
//     // 如果不是，它将异步加载资源并将句柄添加到 _ loadingHandles 集。
//     // 回调附加到句柄的 Completed 事件，并将在资源加载完成时调用。
//     public void LoadResource<T>(object key, Action<T> onComplete)
//     {
//         if (_referenceCount.ContainsKey(key))
//         {
//             _referenceCount[key]++;
//             HandleLoadedResource<T>(key, onComplete);
//             return;
//         }
//
//         AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key.ToString());
//         _loadingHandles.Add(handle);
//         handle.Completed += h => OnLoadResourceComplete(key, h, onComplete);
//     }
//
//     // 从 _ loadingHandles 集中移除句柄，并在各自的字典中缓存句柄和引用计数。
//     // 然后调用 HandleLoadedResource 实际加载资源并调用回调。
//     private void OnLoadResourceComplete<T>(object key, AsyncOperationHandle<T> handle, Action<T> onComplete)
//     {
//         if (_loadingHandles.Contains(handle))
//             _loadingHandles.Remove(handle);
//
//         if (handle.Status == AsyncOperationStatus.Succeeded)
//         {
//             _handleMap[key] = handle;
//             _referenceCount[key] = 1;
//
//             HandleLoadedResource(key, onComplete);
//         }
//         else
//         {
//             Debug.LogError($"Failed to load resource with key {key}: {handle.OperationException}");
//         }
//     }
//
//     // 检索指定键的句柄，如果句柄已经完成加载，它将使用加载的资源调用回调。
//     private void HandleLoadedResource<T>(object key, Action<T> onComplete)
//     {
//         AsyncOperationHandle handle = _handleMap[key];
//         if (handle.Status == AsyncOperationStatus.Succeeded)
//         {
//             T resource = (T)handle.Result;
//             onComplete?.Invoke(resource);
//         }
//     }
//
//     // 首先检查资源以前是否已加载。
//     // 如果是这样，它将减少引用计数，如果引用计数达到0，它将释放句柄并从字典中删除条目
//     public void UnloadResource(object key)
//     {
//         if (!_referenceCount.ContainsKey(key))
//             return;
//
//         _referenceCount[key]--;
//
//         if (_referenceCount[key] <= 0)
//         {
//             Addressables.Release(_handleMap[key]);
//             _handleMap.Remove(key);
//             _referenceCount.Remove(key);
//         }
//     }
//
//     public void LoadResourcesAsync<T>(string tagName, Action<T> onComplete, Action<AsyncOperationHandle<IList<T>>> onHandleCompleted = null)
//     {
//         var handle = Addressables.LoadAssetsAsync<T>(tagName, onComplete);
//         if(onHandleCompleted != null)
//             handle.Completed += onHandleCompleted;
//         handle.Completed += Addressables.Release;
//     }
//     
//     public void Destroy()
//     {
//         OnDestroy();
//     }
//
//     private void OnDestroy()
//     {
//         foreach (AsyncOperationHandle handle in _loadingHandles)
//         {
//             if (!handle.IsDone)
//             {
//                 handle.WaitForCompletion();
//             }
//         }
//
//         foreach (AsyncOperationHandle handle in _handleMap.Values)
//         {
//             Addressables.Release(handle);
//         }
//
//         _referenceCount.Clear();
//         _handleMap.Clear();
//         _loadingHandles.Clear();
//     }
// }
