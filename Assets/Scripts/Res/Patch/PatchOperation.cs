using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniFramework.Event;
using UnityEngine;
using UnityHFSM;
using WGame.Res;
using YooAsset;

public class PatchOperation : GameAsyncOperation
{
    private enum ESteps
    {
        None,
        Update,
        Done,
    }

    private const int INITIALIZE_PACKAGE = 0;
    private const int UPDATE_PACKAGE_VERSION = 1;
    private const int UPDATE_PACKAGE_MANIFEST = 2;
    private const int CREATE_PACKAGE_DOWNLOADER = 3;
    private const int DOWNLOAD_PACKAGE_FILES = 4;
    private const int DOWNLOAD_PACKAGE_OVER = 5;
    private const int CLEAR_PACKAGE_CACHE = 6;
    private const int UPDATE_DONE = 7;

    #region 文件解密类

    /// <summary>
    /// 资源文件流加载解密类
    /// </summary>
    private class FileStreamDecryption : IDecryptionServices
    {
        /// <summary>
        /// 同步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            managedStream = bundleStream;
            return AssetBundle.LoadFromStream(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
        }

        /// <summary>
        /// 异步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            managedStream = bundleStream;
            return AssetBundle.LoadFromStreamAsync(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
        }

        private static uint GetManagedReadBufferSize()
        {
            return 1024;
        }
    }
    
    /// <summary>
    /// 资源文件偏移加载解密类
    /// </summary>
    private class FileOffsetDecryption : IDecryptionServices
    {
        /// <summary>
        /// 同步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            managedStream = null;
            return AssetBundle.LoadFromFile(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
        }

        /// <summary>
        /// 异步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            managedStream = null;
            return AssetBundle.LoadFromFileAsync(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
        }

        private static ulong GetFileOffset()
        {
            return 32;
        }
    }

    #endregion
    
    private ESteps _steps = ESteps.None;

    private readonly string _packageName;
    private readonly string _buildPipeline;
    private string _packageVersion;
    private ResourceDownloaderOperation _downloader;
    private readonly EPlayMode _playMode;
    private StateMachine<int, int, int> fsm;
    private readonly EventGroup _eventGroup = new EventGroup();

    public PatchOperation(string packageName, string buildPipeline, EPlayMode playMode)
    {
        _packageName = packageName;
        _buildPipeline = buildPipeline;
        _playMode = playMode;
    }
    protected override void OnStart()
    {
        // 注册监听事件
        _eventGroup.AddListener<UserEventDefine.UserTryInitialize>(OnHandleEventMessage);
        _eventGroup.AddListener<UserEventDefine.UserBeginDownloadWebFiles>(OnHandleEventMessage);
        _eventGroup.AddListener<UserEventDefine.UserTryUpdatePackageVersion>(OnHandleEventMessage);
        _eventGroup.AddListener<UserEventDefine.UserTryUpdatePatchManifest>(OnHandleEventMessage);
        _eventGroup.AddListener<UserEventDefine.UserTryDownloadWebFiles>(OnHandleEventMessage);
        _steps = ESteps.Update;
        fsm = new StateMachine<int, int, int>();

        MonoBehaviour mono = YooassetManager.Inst;
        fsm.AddState(INITIALIZE_PACKAGE, new CoState<int>(mono, InitPackage));
        fsm.AddState(UPDATE_PACKAGE_VERSION, new CoState<int>(mono, UpdatePackageVersion));
        fsm.AddState(UPDATE_PACKAGE_MANIFEST, new CoState<int>(mono, UpdatePackageManifest));
        fsm.AddState(CREATE_PACKAGE_DOWNLOADER, new CoState<int>(mono, CreatePackageDownloader));
        fsm.AddState(DOWNLOAD_PACKAGE_FILES, new CoState<int>(mono, DownloadPackageFiles));
        fsm.AddState(DOWNLOAD_PACKAGE_OVER, new State<int>(onEnter: DownloadPackageOver));
        fsm.AddState(CLEAR_PACKAGE_CACHE, new State<int>(onEnter: ClearPackageCache));
        fsm.AddState(UPDATE_DONE, new State<int>());
        
        fsm.SetStartState(INITIALIZE_PACKAGE);
        fsm.Init();
        // 初始化资源包
        WLogger.Info("初始化资源包");
    }

    protected override void OnUpdate()
    {
        if (_steps == ESteps.None || _steps == ESteps.Done)
            return;
        if (_steps == ESteps.Update)
        {
            fsm.OnLogic();
            if(fsm.ActiveState.name == UPDATE_DONE)
            {
                _eventGroup.RemoveAllListener();
                Status = EOperationStatus.Succeed;
                _steps = ESteps.Done;
            }
        }
    }

    protected override void OnAbort()
    {
        
    }

    #region 初始化包

    private IEnumerator InitPackage()
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("初始化资源包！");
        // 创建资源包
        var package = YooAssets.TryGetPackage(_packageName);
        if (package == null)
            package = YooAssets.CreatePackage(_packageName);
        
        // 编辑器模拟模式
        InitializationOperation initializationOperation = null;
        switch (_playMode)
        {
            case EPlayMode.EditorSimulateMode:
                var simulateModeParameters = new EditorSimulateModeParameters();
                simulateModeParameters.SimulateManifestFilePath =
                    EditorSimulateModeHelper.SimulateBuild(_buildPipeline, _packageName);
                initializationOperation = package.InitializeAsync(simulateModeParameters);
                break;
            case EPlayMode.OfflinePlayMode:
                var offlineModeParameters = new OfflinePlayModeParameters();
                offlineModeParameters.DecryptionServices = new FileStreamDecryption();
                initializationOperation = package.InitializeAsync(offlineModeParameters);
                break;
            case EPlayMode.HostPlayMode:
                var defaultHostServer = GetHostServerURL();
                var fallbackHostServer = GetHostServerURL();
                var hostModeParameters = new HostPlayModeParameters();
                hostModeParameters.DecryptionServices = new FileStreamDecryption();
                hostModeParameters.BuildinQueryServices = new GameQueryServices();
                hostModeParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                initializationOperation = package.InitializeAsync(hostModeParameters);
                break;
            case EPlayMode.WebPlayMode:
                var defaultWebServer = GetHostServerURL();
                var fallbackWebServer = GetHostServerURL();
                var webModeParameters = new WebPlayModeParameters();
                webModeParameters.DecryptionServices = new FileStreamDecryption();
                webModeParameters.BuildinQueryServices = new GameQueryServices();
                webModeParameters.RemoteServices = new RemoteServices(defaultWebServer, fallbackWebServer);
                initializationOperation = package.InitializeAsync(webModeParameters);
                break;
        }

        yield return initializationOperation;

        // 如果初始化失败弹出提示界面
        if (initializationOperation.Status != EOperationStatus.Succeed)
        {
            WLogger.Warning($"{initializationOperation.Error}");
            PatchEventDefine.InitializeFailed.SendEventMessage();
            // WLogger.Error("初始化失败");
        }
        else
        {
            var version = initializationOperation.PackageVersion;
            WLogger.Info($"Init resource package version : {version}");
            fsm.RequestStateChange(UPDATE_PACKAGE_VERSION);
        }
    }
    
    /// <summary>
    /// 获取资源服务器地址
    /// </summary>
    private string GetHostServerURL()
    {
        //string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
        string hostServerIP = "https://a.unity.cn/client_api/v1/buckets/557bcfe9-a674-40ab-bcbf-cdf3aa22a905/entry_by_path/content/?path=";
        string appVersion = "v1.0";

#if UNITY_EDITOR
        if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
            return $"{hostServerIP}/UOS CDN/Android/{appVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
            return $"{hostServerIP}/UOS CDN/IPhone/{appVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
            return $"{hostServerIP}/UOS CDN/WebGL/{appVersion}";
        else
            return $"{hostServerIP}/UOS CDN/PC/{appVersion}";
#else
        if (Application.platform == RuntimePlatform.Android)
            return $"{hostServerIP}/CDN/Android/{appVersion}";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return $"{hostServerIP}/CDN/IPhone/{appVersion}";
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            return $"{hostServerIP}/CDN/WebGL/{appVersion}";
        else
            return $"{hostServerIP}/CDN/PC/{appVersion}";
#endif
    }

    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    private class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }
        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }


    #endregion

    #region 更新资源版本号
    
    private IEnumerator UpdatePackageVersion()
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("获取最新的资源版本 !");
        // WLogger.Info("获取最新的资源版本 !");
        yield return new WaitForSecondsRealtime(0.5f);
        var package = YooAssets.GetPackage(_packageName);
        var operation = package.UpdatePackageVersionAsync(false);
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            WLogger.Warning(operation.Error);
            PatchEventDefine.PackageVersionUpdateFailed.SendEventMessage();
            // WLogger.Error("包版本更新失败");
        }
        else
        {
            _packageVersion = operation.PackageVersion;
            fsm.RequestStateChange(UPDATE_PACKAGE_MANIFEST);
        }
    }
    
    #endregion

    #region 更新资源清单

    private IEnumerator UpdatePackageManifest()
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("更新资源清单！");
        // WLogger.Info("更新资源清单! ");
        yield return new WaitForSecondsRealtime(0.5f);

        var package = YooAssets.GetPackage(_packageName);
        bool savePackageVersion = true;
        var operation = package.UpdatePackageManifestAsync(_packageVersion, savePackageVersion);
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            WLogger.Warning(operation.Error);
            PatchEventDefine.PatchManifestUpdateFailed.SendEventMessage();
            // WLogger.Error("更新资源清单失败!");
            yield break;
        }
        else
        {
            fsm.RequestStateChange(CREATE_PACKAGE_DOWNLOADER);
        }
    }

    #endregion

    #region 创建文件下载器

    private IEnumerator CreatePackageDownloader()
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("创建补丁下载器！");
        // WLogger.Info("创建补丁下载器! ");
        yield return new WaitForSecondsRealtime(0.5f);

        var package = YooAssets.GetPackage(_packageName);
        var downloadingMaxNum = 10;
        var failedTryAgain = 3;
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
        _downloader = downloader;

        if (downloader.TotalDownloadCount == 0)
        {
            WLogger.Info("没有下载任何文件！");
            fsm.RequestStateChange(UPDATE_DONE);
        }
        else
        {
            // 发现新更新文件后，挂起流程系统
            // 注意：开发者需要在下载前检测磁盘空间不足
            int totalDownloadCount = downloader.TotalDownloadCount;
            long totalDownloadBytes = downloader.TotalDownloadBytes;
            PatchEventDefine.FoundUpdateFiles.SendEventMessage(totalDownloadCount, totalDownloadBytes);
            // WLogger.Info("下载完毕， 文件总数：" + totalDownloadCount + ", 总大小：" + totalDownloadBytes);
        }
    }

    #endregion

    #region 下载更新文件

    private IEnumerator DownloadPackageFiles()
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("开始下载补丁文件！");
        _downloader.OnDownloadErrorCallback = PatchEventDefine.WebFileDownloadFailed.SendEventMessage;
        _downloader.OnDownloadProgressCallback = PatchEventDefine.DownloadProgressUpdate.SendEventMessage;
        _downloader.BeginDownload();
        yield return _downloader;
        
        // 检测下载结果
        if (_downloader.Status != EOperationStatus.Succeed)
            yield break;
        fsm.RequestStateChange(DOWNLOAD_PACKAGE_OVER);
    }

    #endregion

    #region 下载完毕

    private void DownloadPackageOver(State<int, string> state)
    {
        fsm.RequestStateChange(CLEAR_PACKAGE_CACHE);
    }

    #endregion

    #region 清理未使用的缓存文件

    private void ClearPackageCache(State<int, string> state)
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("清理未使用的缓存文件！");
        var package = YooAssets.GetPackage(_packageName);
        var operation = package.ClearUnusedCacheFilesAsync();
        operation.Completed += @base =>
        {
            fsm.RequestStateChange(UPDATE_DONE);
        };
    }
    
    #endregion
    
    /// <summary>
    /// 接收事件
    /// </summary>
    private void OnHandleEventMessage(IEventMessage message)
    {
        if (message is UserEventDefine.UserTryInitialize)
        {
            fsm.RequestStateChange(INITIALIZE_PACKAGE);
        }
        else if (message is UserEventDefine.UserBeginDownloadWebFiles)
        {
            // _machine.ChangeState<FsmDownloadPackageFiles>();
            fsm.RequestStateChange(DOWNLOAD_PACKAGE_FILES);
        }
        else if (message is UserEventDefine.UserTryUpdatePackageVersion)
        {
            // _machine.ChangeState<FsmUpdatePackageVersion>();
            fsm.RequestStateChange(UPDATE_PACKAGE_VERSION);
        }
        else if (message is UserEventDefine.UserTryUpdatePatchManifest)
        {
            // _machine.ChangeState<FsmUpdatePackageManifest>();
            fsm.RequestStateChange(UPDATE_PACKAGE_MANIFEST);
        }
        else if (message is UserEventDefine.UserTryDownloadWebFiles)
        {
            // _machine.ChangeState<FsmCreatePackageDownloader>();
            fsm.RequestStateChange(CREATE_PACKAGE_DOWNLOADER);
        }
        else
        {
            throw new System.NotImplementedException($"{message.GetType()}");
        }
    }
}
