// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;
//
// public interface IResourcesService
// {
//     void LoadResource<T>(object key, Action<T> onComplete);
//     void UnloadResource(object key);
//     void LoadResourcesAsync<T>(string tagName, Action<T> onComplete, Action<AsyncOperationHandle<IList<T>>> onHandleCompleted = null);
//     void Destroy();
// }
